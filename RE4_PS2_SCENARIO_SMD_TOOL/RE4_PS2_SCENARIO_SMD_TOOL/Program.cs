using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SHARED_PS2_SCENARIO_SMD;

namespace RE4_PS2_SCENARIO_SMD_TOOL
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine(SHARED_TOOLS.Shared.HeaderText());

            bool usingBatFile = false;
            int start = 0;
            if (args.Length > 0 && args[0].ToLowerInvariant() == "-bat")
            {
                usingBatFile = true;
                start = 1;
            }

            for (int i = start; i < args.Length; i++)
            {
                if (File.Exists(args[i]))
                {
                    try
                    {
                        FileInfo fileInfo1 = new FileInfo(args[i]);
                        string file1Extension = fileInfo1.Extension.ToUpperInvariant();
                        Console.WriteLine("File: " + fileInfo1.Name);
                        ContinueActions(fileInfo1, file1Extension);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + Environment.NewLine + ex);
                    }
                }
                else
                {
                    Console.WriteLine("File specified does not exist: " + args[i]);
                }

            }

            if (args.Length == 0)
            {
                Console.WriteLine("How to use: drag the file to the executable.");
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-PS2-SCENARIO-SMD-TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Finished!!!");
                if (!usingBatFile)
                {
                    Console.WriteLine("Press any key to close the console.");
                    Console.ReadKey();
                }
            }

        }

        private static void ContinueActions(FileInfo fileInfo, string Extension)
        {
          
            if (Extension == ".SMD")
            {
                Start_SMD_Extract.SMD_Extract(fileInfo);
            }
            else if (Extension == ".IDX_PS2_SCENARIO")
            {
                Start_ScenarioOBJ_Repack.ScenarioOBJ_Repack(fileInfo);
            }
            else if (Extension == ".IDX_PS2_SMD")
            {
                Start_IdxSMD_Repack.IdxSMD_Repack(fileInfo);
            }
            else
            {
                Console.WriteLine("Invalid file format: " + fileInfo.Name);
            }

        }

    }
}