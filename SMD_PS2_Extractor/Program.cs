using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SMD_PS2_Extractor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("start");

            Console.WriteLine("SMD_PS2_Extractor Version A.1.0.0.0");

            if (args.Length >= 1 && File.Exists(args[0]))
            {
                bool EnableDebugInfo = false;

                if (args.Length >= 2 && args[1].ToUpper() == "TRUE")
                {
                    EnableDebugInfo = true;
                }


                FileInfo fileInfo = new FileInfo(args[0]);
                Console.WriteLine(fileInfo.Name);

                if (fileInfo.Extension.ToUpper() == ".SMD")
                {
                    try
                    {
                        SMDPS2Extractor.Extrator(fileInfo.FullName, EnableDebugInfo);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                    }
                 
                 
                }
                else 
                {
                    Console.WriteLine("it is not an SMD file");
                }
            }
            else
            {
                Console.WriteLine("no arguments or invalid file");
            }

            Console.WriteLine("end");
        }

    }

}
