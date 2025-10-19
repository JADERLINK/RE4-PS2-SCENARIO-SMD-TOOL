using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT
{
    public class ToFileMethod_TPL_PS2
    {
        private string DirectoryToSaveTPL = "";
        private string BaseFileNameTPL = "";
        private bool EnableExtract = false;

        public ToFileMethod_TPL_PS2(bool EnableExtract, string DirectoryToSaveTPL, string BaseFileNameTPL)
        {
            this.DirectoryToSaveTPL = DirectoryToSaveTPL;
            this.BaseFileNameTPL = BaseFileNameTPL;
            this.EnableExtract = EnableExtract;
        }

        public void ToFileTpl(Stream fileStream, long tplOffset, long endOffset, int tplID)
        {
            if (EnableExtract && tplOffset > 0)
            {
                string FileName = BaseFileNameTPL + ".TPL";
                if (tplID > 0)
                {
                    FileName = BaseFileNameTPL + $".{tplID:D}.TPL";
                }
                try
                {
                    //le os bytes do tpl e grava em um arquivo
                    fileStream.Position = tplOffset;
                    long tplLenght = endOffset - tplOffset;

                    byte[] tplArray = new byte[tplLenght];
                    fileStream.Read(tplArray, 0, (int)tplLenght);

                    string tplPath = Path.Combine(DirectoryToSaveTPL, FileName);

                    Directory.CreateDirectory(DirectoryToSaveTPL);
                    File.WriteAllBytes(tplPath, tplArray);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error on write in file: " + FileName + Environment.NewLine + ex.ToString());
                }
            }
        }

    }
}
