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

            Console.WriteLine("## SMD_PS2_Extractor ##");
            Console.WriteLine($"## Version {SMDPS2Extractor.VERSION} ##");
            Console.WriteLine("## By JADERLINK and HardRain ##");

            if (args.Length >= 1 && File.Exists(args[0]))
            {
                bool ExtractOnlyModel = false;

                if (args.Length >= 2 && args[1].ToUpper() == "TRUE")
                {
                    ExtractOnlyModel = true;
                }

                bool EnableDebugInfo = false;

                if (args.Length >= 3 && args[2].ToUpper() == "TRUE")
                {
                    EnableDebugInfo = true;
                }


                FileInfo fileInfo = new FileInfo(args[0]);
                Console.WriteLine(fileInfo.Name);

                if (fileInfo.Extension.ToUpper() == ".SMD")
                {
                    try
                    {
                        SMDPS2Extractor.Extrator(fileInfo.FullName, ExtractOnlyModel, EnableDebugInfo);
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
