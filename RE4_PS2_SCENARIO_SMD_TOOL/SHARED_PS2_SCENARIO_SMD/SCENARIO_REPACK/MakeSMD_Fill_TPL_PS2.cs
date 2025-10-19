using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK
{
    public class MakeSMD_Fill_TPL_PS2
    {
        private string TplFileFullPath;

        public MakeSMD_Fill_TPL_PS2(string TplFileFullPath)
        {
            this.TplFileFullPath = TplFileFullPath;
        }

        public void Fill(BinaryWriter bw, int tplFilesCount, long TplAreaOffset, out long endOffset)
        {
            //---------------------------
            // PARTE DOS ARQUIVOS TPLs

            int TplOffsetBlockCount = (int)((((TplAreaOffset + (tplFilesCount * 4) + 15) / 16) * 16) - TplAreaOffset);

            bw.BaseStream.Position = TplAreaOffset;
            bw.Write(new byte[TplOffsetBlockCount]);

            long OffsetToOffsetTpl = TplAreaOffset;
            long RealOffsetTpl = TplOffsetBlockCount;

            for (int i = 0; i < tplFilesCount; i++)
            {
                bw.BaseStream.Position = OffsetToOffsetTpl;
                bw.Write((uint)RealOffsetTpl);
                long startTplOffset = TplAreaOffset + RealOffsetTpl;
                long endTplOffset;

                PutTpl(bw, i, startTplOffset, out endTplOffset);

                OffsetToOffsetTpl += 4;
                RealOffsetTpl = (endTplOffset - TplAreaOffset);
                bw.BaseStream.Position = endTplOffset;
            }

            endOffset = bw.BaseStream.Position;
        }


        private void PutTpl(BinaryWriter bw, int tplId, long startTplOffset, out long endTplOffset)
        {
            string tempTplFilePath = TplFileFullPath;
            if (tplId > 0)
            {
                tempTplFilePath = Path.ChangeExtension(tempTplFilePath, $"{tplId}.tpl");
            }

            if (File.Exists(tempTplFilePath))
            {
                try
                {
                    MemoryStream ms = new MemoryStream();
                    ms.Position = 0;

                    FileInfo info = new FileInfo(tempTplFilePath);
                    var read = info.OpenRead();
                    read.CopyTo(ms);
                    read.Close();

                    // alinhamento do tpl
                    int _padding = (int)((16 - (ms.Position % 16)) % 16);
                    ms.Write(new byte[_padding], 0, _padding);
                    long tplLength = ms.Position;

                    //verifica o magic
                    ms.Position = 0;
                    BinaryReader br = new BinaryReader(ms);
                    uint __magic = br.ReadUInt32();
                    uint __tplAmount = br.ReadUInt32();

                    if (__magic != 0x00_00_10_00 || __tplAmount > 0x00_01_00_00)
                    {
                        throw new ApplicationException("The TPL file is from a different version of the SMD that is being repacked.");
                    }

                    // copia
                    bw.BaseStream.Position = startTplOffset;
                    ms.Position = 0;
                    ms.CopyTo(bw.BaseStream);
                    ms.Close();
                    endTplOffset = bw.BaseStream.Position;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error in {Path.GetFileName(tempTplFilePath)}: " + Environment.NewLine + ex.Message);
                    PutEmptyFiles.PutTpl(bw.BaseStream, startTplOffset, out endTplOffset);
                }
            }
            else
            {
                Console.WriteLine($"Error in {Path.GetFileName(tempTplFilePath)}: " + Environment.NewLine + "File not Exist!");
                PutEmptyFiles.PutTpl(bw.BaseStream, startTplOffset, out endTplOffset);
            }
        }

    }
}
