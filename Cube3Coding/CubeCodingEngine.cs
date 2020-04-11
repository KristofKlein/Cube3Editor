using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using BitForByteSupport;
using FileHelper;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;

namespace Cube3Coding
{
    public static class CubeCodingEngine
    {
        private const string key = "221BBakerMycroft";
        public static CubeEngine Engine { get; private set; }
        private static CubeExtractor extractor;
        private static byte[] inputCube3File;
        private static Byte[] dataModel;
        public static void EncodeCube3FromBFBFile(String FileName, String cube3FileName)
        {
            if (FileName != null || FileName.Length > 0)
            {
                try
                {
                    using var inFile = File.OpenRead(FileName);
                    using var streamReader = new StreamReader(inFile);
                    var inputBFBFile = streamReader.ReadToEnd();

                    try
                    {
                        Encoding encoding = Encoding.ASCII;
                        Engine = new CubeEngine();
                        PaddedBufferedBlockCipher cipher;
                        ZeroBytePadding padding = new ZeroBytePadding();
                        cipher = new PaddedBufferedBlockCipher(Engine, padding);

                        // create the key parameter
                        Byte[] keyBytes = encoding.GetBytes(key);
                        KeyParameter param = new KeyParameter(encoding.GetBytes(key));

                        // initalize the cipher
                        cipher.Init(true, new KeyParameter(keyBytes));

                        Byte[] newDataModel = encoding.GetBytes(inputBFBFile);

                        Byte[] encodedBytes = new Byte[cipher.GetOutputSize(newDataModel.Length)];

                        int encodedLength = cipher.ProcessBytes(newDataModel, 0, newDataModel.Length, encodedBytes, 0);
                        cipher.DoFinal(encodedBytes, encodedLength);

                        FileBackup.MakeBackup(cube3FileName, 5);

                        using var outFile = File.OpenWrite(cube3FileName);
                        using var binaryWriter = new BinaryWriter(outFile);
                        binaryWriter.Write(encodedBytes);
                        outFile.Close();
                    }
                    catch (IOException)
                    {
                        Console.WriteLine($"Unable to process BFB File [{FileName}] or unable to open output file [{cube3FileName}]");
                        Environment.Exit(-1);
                    }
                }
                catch (SecurityException ex)
                {
                    Console.WriteLine($"Security error\n\nError message: {ex.Message}\n\n" +
                        $"Details:\n\n{ex.StackTrace}");
                    Environment.Exit(ex.HResult);
                }
            }
            else
            {
                throw new SecurityException("No File specified!");
            }
        }

        public static void DecodeBFBFromCube3File(String FileName, String bfbFileName, Boolean xmlMode)
        {
            Encoding encoding = Encoding.ASCII;
            String decodedModel;
            BitFromByte bfbObject;

            Engine = new CubeEngine();
            PaddedBufferedBlockCipher cipher;

            if (FileName != null || FileName.Length > 0)
            {
                try
                {
                    using var inFile = File.OpenRead(FileName);
                    using var binaryReader = new BinaryReader(inFile);
                    inputCube3File = binaryReader.ReadBytes((int)binaryReader.BaseStream.Length);
                    bool copyInputFile = true;
                    if (!RawCubeFile())
                    {
                        extractor = new CubeExtractor(inputCube3File);

                        String rawCube3Filename = extractor.GetCubeFilename();
                        if (rawCube3Filename != null)
                        {
                            copyInputFile = false;
                            dataModel = new byte[extractor.ModelFiles[rawCube3Filename].Length];
                            Array.Copy(extractor.ModelFiles[rawCube3Filename], dataModel, extractor.ModelFiles[rawCube3Filename].Length);
                        }
                    }

                    if (copyInputFile)
                    {
                        dataModel = new byte[inputCube3File.Length];
                        Array.Copy(inputCube3File, dataModel, inputCube3File.Length);
                    }

                    try
                    {
                        ZeroBytePadding padding = new ZeroBytePadding();
                        cipher = new PaddedBufferedBlockCipher(Engine, padding);

                        // make sure buffer is a multiple of Blowfish Block size.
                        int leftover = dataModel.Length % cipher.GetBlockSize();
                        if (leftover != 0)
                        {
                            Array.Resize(ref dataModel, dataModel.Length + (cipher.GetBlockSize() - leftover));
                        }

                        // create the key parameter 
                        Byte[] keyBytes = encoding.GetBytes(key);
                        KeyParameter param = new KeyParameter(encoding.GetBytes(key));

                        // initalize the cipher
                        cipher.Init(false, new KeyParameter(keyBytes));

                        byte[] decodedBytes = new byte[cipher.GetOutputSize(dataModel.Length)];

                        int decodedLength = cipher.ProcessBytes(dataModel, 0, dataModel.Length, decodedBytes, 0);
                        if (decodedLength % 8 == 0)
                            cipher.DoFinal(decodedBytes, decodedLength);

                        decodedModel = encoding.GetString(decodedBytes);

                        if (xmlMode)
                        {
                            int endXmlIndex = decodedModel.LastIndexOf('>');

                            if (endXmlIndex < decodedModel.Length - 1)
                            {
                                decodedModel = decodedModel.Substring(0, endXmlIndex + 1);
                            }
                        }

                        string[] seperator = new string[] { "\r\n" };
                        string[] decodedModelArray = decodedModel.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                        List<String> bfbStringList;
                        if (xmlMode)
                        {
                            bfbStringList = decodedModelArray.ToList();
                        }
                        else
                        {
                            bfbObject = new BitFromByte(encoding, decodedBytes);
                            bfbStringList = bfbObject.BfbLines;
                        }

                        System.IO.StreamWriter file = new System.IO.StreamWriter(bfbFileName, false);

                        int numBFBLines = 0;
                        foreach (string bfbLine in bfbStringList)
                        {
                            numBFBLines++;
                            if (numBFBLines < bfbStringList.Count)
                                file.WriteLine(bfbLine);
                            else
                                file.Write(bfbLine);
                        }

                        file.Close();
                    }
                    catch (IOException)
                    {
                        Console.WriteLine($"Unable to process CUBE3 File [{FileName}]\n\n" +
                            $"Was this really a CUBE3 file?");
                        Environment.Exit(-1);
                    }
                }
                catch (SecurityException ex)
                {
                    Console.WriteLine($"Security error\n\nError message: {ex.Message}\n\n" +
                        $"Details:\n\n{ex.StackTrace}");
                    Environment.Exit(ex.HResult);
                }
            }
            else
            {
                throw new SecurityException("No File specified!");
            }
        }
        private static bool RawCubeFile()
        {
            bool raw = true;

            Int32 length = BitConverter.ToInt32(inputCube3File, 4);

            if (inputCube3File.Length == length)
            {
                raw = false;
            }
            return raw;
        }
    }
}
