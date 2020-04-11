using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileHelper
{
    public class CubeExtractor
    {
        public int ModelFileCount { get; set; }
        public List<string> ModelFileNames { get; set; } = new List<string>();
        public Dictionary<string, byte[]> ModelFiles { get; set; } = new Dictionary<string, byte[]>();
        public int ModelFileSize { get; set; }
        public int ModelFilenameSize { get; set; }
        public Int16 MaxFilenameLengthPlusSize { get; set; }

        public CubeExtractor()
        {
            ModelFileCount = 0;
            ModelFileSize = 0;
        }

        public CubeExtractor(Byte[] cubeData)
        {
            int fileSize;
            int fileIndex = 0;
            ModelFileCount = BitConverter.ToInt32(cubeData, fileIndex);
            fileIndex += sizeof(UInt32);

            ModelFileSize = BitConverter.ToInt32(cubeData, fileIndex);
            fileIndex += sizeof(UInt32);

            // fileVersion = BitConverter.ToUInt16(cubeData, fileIndex);
            fileIndex += sizeof(UInt16);

            for (int i = 0; i < ModelFileCount; i ++)
            {
                // get file size
                fileSize = BitConverter.ToInt32(cubeData, fileIndex);
                fileIndex += sizeof(UInt32);

                // get file name
                Byte[] fileNameArray = new Byte[260];
                Array.Copy(cubeData, fileIndex, fileNameArray, 0, 260);
                int fileNameLength = Array.IndexOf(fileNameArray, (byte)0, 0);
                Array.Resize(ref fileNameArray, fileNameLength);
                fileIndex += 260;

                Byte[] fileData = new Byte[fileSize];
                Array.Copy(cubeData, fileIndex, fileData, 0, fileSize);
                fileIndex += fileSize;

                string fileName = Encoding.ASCII.GetString(fileNameArray, 0, fileNameArray.Length);
                ModelFileNames.Add(fileName);
                ModelFiles.Add(fileName, fileData);
            }
        }

        public String GetCubeFilename()
        {
            String cubeFilename = null;
            foreach(String filename in ModelFileNames)
            {
                String extension = Path.GetExtension(filename);

                if (extension.Equals(".CUBE3", StringComparison.OrdinalIgnoreCase) || extension.Equals(".CUBEPRO", StringComparison.OrdinalIgnoreCase))
                {
                    cubeFilename = filename;
                    break;
                }
            }
            return cubeFilename;
        }
        public String GetXMLFilename()
        {
            String xmlFilename = null;
            foreach (String filename in ModelFileNames)
            {
                String extension = Path.GetExtension(filename);

                if (extension.Equals(".XML", StringComparison.OrdinalIgnoreCase))
                {
                    xmlFilename = filename;
                    break;
                }
            }
            return xmlFilename;
        }
    }
}
