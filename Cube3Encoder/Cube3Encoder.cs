using System;
using System.IO;
using Cube3Coding;

namespace Cube3Enconder
{
    internal static class Cube3Encoder
    {
        public static void Main(string[] args)
        {
            string cube3File = null;
            string bfbFile = null;

            if (args.Length > 0)
            {
                if (args[0].StartsWith("-"))
                {
                    DisplayHelp();
                    Environment.Exit(0);
                }

                bfbFile = args[0];
                if (args.Length > 1)
                {
                    cube3File = args[1];
                }
            }

            if (bfbFile == null)
            {
                DisplayHelp();
                Environment.Exit(0);
            }

            if (cube3File == null)
            {
                cube3File = Path.GetFileNameWithoutExtension(bfbFile) + ".Cube3";
            }

            CubeCodingEngine.EncodeCube3FromBFBFile(bfbFile, cube3File);
      }

        private static void DisplayHelp()
        {
            System.Console.WriteLine("usage: cube3Encoder <bfbFile> [<Cube3File>]");
            System.Console.WriteLine("");
            System.Console.WriteLine("cube3Encoder reads bfbFile and encodes into Cube3File.   Will generate");
            System.Console.WriteLine("into specified Cube3File or a file with the same name as the BFB File,");
            System.Console.WriteLine("but with the extension Cube3");
            System.Console.WriteLine("");
        }
    }
}
