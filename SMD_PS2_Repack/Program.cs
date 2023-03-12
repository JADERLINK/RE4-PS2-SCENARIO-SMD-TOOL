using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SMD_PS2_Repack
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start");
            Console.WriteLine("SMD_PS2_Repack Version A.1.0.0.0");

            if (args.Length >= 1 && File.Exists(args[0]) && new FileInfo(args[0]).Extension.ToUpper() == ".IDXSMDPS2")
            {
  
                Console.WriteLine(args[0]);

                try
                {
                    var fileinfo = new FileInfo(args[0]);
                    string parentDirectory = fileinfo.DirectoryName;
                    if (parentDirectory[parentDirectory.Length-1] != '\\')
                    {
                        parentDirectory += "\\";
                    }

                    string smdPath = fileinfo.FullName.Substring(0, fileinfo.FullName.Length - fileinfo.Extension.Length) + ".SMD";
                    SMDPS2Repack.Repack(fileinfo.FullName, smdPath, parentDirectory);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }


            }
            else
            {
                Console.WriteLine("no arguments or invalid file");
            }

            Console.WriteLine("End");

        }
    }
}
