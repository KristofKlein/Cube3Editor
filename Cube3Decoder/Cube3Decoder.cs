using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using Cube3Coding;

namespace Cube3Decoder
{
    internal static class Cube3Decoder
    {
        public static void Main(string[] args)
        {
            int cube3FileIndex = 0;
            int bfbFileIndex = 1;

            bool xmlMode = false;
            string cube3File = null;
            string bfbFile = null;

            if (args.Length > 0)
            {
                if (args[0].StartsWith("-"))
                {
                    cube3FileIndex++;
                    bfbFileIndex++;

                    if (args[0].Equals("-XML", StringComparison.OrdinalIgnoreCase))
                    {
                        xmlMode = true;
                    }
                    else
                    {
                        System.Console.WriteLine($"Invalid parameter {args[0]}!");
                        System.Console.WriteLine("");
                        DisplayHelp();
                        Environment.Exit(-1);
                    }
                }
                cube3File = args[cube3FileIndex];
                if (args.Length > bfbFileIndex)
                {
                    bfbFile = args[bfbFileIndex];
                }
            }

            if (cube3File == null)
            {
                DisplayHelp();
                Environment.Exit(0);
            }

            if (bfbFile == null)
            {
                if (!xmlMode)
                {
                    bfbFile = Path.GetFileNameWithoutExtension(cube3File) + ".BFB";
                }
                else
                {
                    bfbFile = Path.GetFileNameWithoutExtension(cube3File) + ".XML";
                }
            }
            //cube3File = cube3File.Replace("/","\\");
            //bfbFile = bfbFile.Replace('/','\\');

            CubeCodingEngine.DecodeBFBFromCube3File(cube3File, bfbFile, xmlMode);
        }

        private static void DisplayHelp()
        {
            System.Console.WriteLine("usage: cube3Decoder [-xml] <cube3File> [<bfbFile>]");
            System.Console.WriteLine("");
            System.Console.WriteLine("cube3decoder reads cube3File and decodes the BFBfile.  Will generate output");
            System.Console.WriteLine("into specified bfbFile or a file with the same name as the Cube3 File,");
            System.Console.WriteLine("but with the extension BFB");
            System.Console.WriteLine("");
            System.Console.WriteLine("-xml will strip any ending bytes following the last closing angle bracket.");
        }
    }
}
