﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO
{
    public class ToFileMethods
    {

        private string DirectoryToSaveBIN = "";
        private string PathToSaveTPL = "";
        private bool EnableExtract = false;

        public ToFileMethods(string DirectoryToSaveBIN, string PathToSaveTPL, bool EnableExtract)
        {
            this.DirectoryToSaveBIN = DirectoryToSaveBIN;
            this.PathToSaveTPL = PathToSaveTPL;
            this.EnableExtract = EnableExtract;
        }

        public void ToFileBin(Stream fileStream, long binOffset, long endOffset, int binID)
        {
            if (EnableExtract)
            {
                try
                {
                    if ( ! Directory.Exists(DirectoryToSaveBIN))
                    {
                        Directory.CreateDirectory(DirectoryToSaveBIN);
                    }

                    //le os bytes do bin e grava em um arquivo
                    fileStream.Position = binOffset;
                    long lenght = endOffset - binOffset;

                    byte[] binArray = new byte[lenght];
                    fileStream.Read(binArray, 0, (int)lenght);

                    string binPath = Path.Combine(DirectoryToSaveBIN, binID.ToString("D4") + ".BIN");
                    File.WriteAllBytes(binPath, binArray);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error on write in file: " + binID.ToString("D3") + ".BIN" + Environment.NewLine + ex.ToString());
                }
            }
        }

        public void ToFileTpl(Stream fileStream, long tplOffset, long endOffset)
        {
            if (EnableExtract)
            {
                try
                {
                    //le os bytes do tpl e grava em um arquivo
                    fileStream.Position = tplOffset;
                    long tplLenght = endOffset - tplOffset;

                    byte[] tplArray = new byte[tplLenght];
                    fileStream.Read(tplArray, 0, (int)tplLenght);

                    File.WriteAllBytes(PathToSaveTPL, tplArray);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error on write in file: TPL.TPL" + Environment.NewLine + ex.ToString());
                }
            }
        }



    }
}
