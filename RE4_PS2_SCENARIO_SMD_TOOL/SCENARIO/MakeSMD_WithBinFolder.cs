using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO
{
    public static class MakeSMD_WithBinFolder
    {
        public static void CreateSMD(string baseDirectory, IdxPs2Scenario idxScenario)
        {
            string binPath = Path.Combine(baseDirectory, idxScenario.BinFolder);
            string tplFilePath = Path.Combine(baseDirectory, idxScenario.TplFile);

            int SmdCount = idxScenario.SmdAmount;

            Stream stream = new FileInfo(Path.Combine(baseDirectory, idxScenario.SmdFileName)).Create();

            byte[] header = new byte[0x10];
            header[0] = 0x40;

            byte[] b_SmdCount = BitConverter.GetBytes(SmdCount);
            header[2] = b_SmdCount[0];
            header[3] = b_SmdCount[1];

            uint binStreamPosition = (uint)(SmdCount * 64) + 0x10;
            byte[] b_binStreamPosition = BitConverter.GetBytes(binStreamPosition);
            header[4] = b_binStreamPosition[0];
            header[5] = b_binStreamPosition[1];
            header[6] = b_binStreamPosition[2];
            header[7] = b_binStreamPosition[3];

            stream.Write(header, 0, 0x10);
            //---------


            for (int i = 0; i < SmdCount; i++)
            {
                float positionX = 0f;
                float positionY = 0f;
                float positionZ = 0f;
                float positionW = 1f;
                float angleX = 0f;
                float angleY = 0f;
                float angleZ = 0f;
                float angleW = 1f;
                float scaleX = 1f;
                float scaleY = 1f;
                float scaleZ = 1f;
                float scaleW = 1f;

                ushort BinID = 0;
                byte FixedFF = 0xFF;
                byte SmxID = 0;
                uint unused1 = 0;
                uint objectStatus = 0;
                uint unused2 = 0;

                if (idxScenario.SmdLines.Length > i)
                {
                    positionX = idxScenario.SmdLines[i].positionX * CONSTs.GLOBAL_SCALE;
                    positionY = idxScenario.SmdLines[i].positionY * CONSTs.GLOBAL_SCALE;
                    positionZ = idxScenario.SmdLines[i].positionZ * CONSTs.GLOBAL_SCALE;

                    angleX = idxScenario.SmdLines[i].angleX;
                    angleY = idxScenario.SmdLines[i].angleY;
                    angleZ = idxScenario.SmdLines[i].angleZ;

                    scaleX = idxScenario.SmdLines[i].scaleX;
                    scaleY = idxScenario.SmdLines[i].scaleY;
                    scaleZ = idxScenario.SmdLines[i].scaleZ;

                }

                if (idxScenario.SmdLinesExtras.Length > i)
                {
                    positionW = idxScenario.SmdLinesExtras[i].positionW;
                    angleW = idxScenario.SmdLinesExtras[i].angleW;
                    scaleW = idxScenario.SmdLinesExtras[i].scaleW;

                    BinID = idxScenario.SmdLinesExtras[i].BinID;
                    FixedFF = idxScenario.SmdLinesExtras[i].FixedFF;
                    SmxID = idxScenario.SmdLinesExtras[i].SmxID;
                    unused1 = idxScenario.SmdLinesExtras[i].unused1;
                    objectStatus = idxScenario.SmdLinesExtras[i].objectStatus;
                    unused2 = idxScenario.SmdLinesExtras[i].unused2;
                }


                //----

                byte[] SMDLine = new byte[64];

                BitConverter.GetBytes(positionX).CopyTo(SMDLine, 0);
                BitConverter.GetBytes(positionY).CopyTo(SMDLine, 4);
                BitConverter.GetBytes(positionZ).CopyTo(SMDLine, 8);
                BitConverter.GetBytes(positionW).CopyTo(SMDLine, 12);
                BitConverter.GetBytes(angleX).CopyTo(SMDLine, 16);
                BitConverter.GetBytes(angleY).CopyTo(SMDLine, 20);
                BitConverter.GetBytes(angleZ).CopyTo(SMDLine, 24);
                BitConverter.GetBytes(angleW).CopyTo(SMDLine, 28);
                BitConverter.GetBytes(scaleX).CopyTo(SMDLine, 32);
                BitConverter.GetBytes(scaleY).CopyTo(SMDLine, 36);
                BitConverter.GetBytes(scaleZ).CopyTo(SMDLine, 40);
                BitConverter.GetBytes(scaleW).CopyTo(SMDLine, 44);
                BitConverter.GetBytes(BinID).CopyTo(SMDLine, 48);
                SMDLine[50] = FixedFF;
                SMDLine[51] = SmxID;
                BitConverter.GetBytes(unused1).CopyTo(SMDLine, 52);
                BitConverter.GetBytes(objectStatus).CopyTo(SMDLine, 56);
                BitConverter.GetBytes(unused2).CopyTo(SMDLine, 60);

                stream.Write(SMDLine, 0, 64);
            }

            //---------------------------

            // PARTE DOS ARQUIVOS BINS

            // BLOCO DOS OFFSETS

            int BinCount = idxScenario.BinAmount;

            int offsetBlockCount = BinCount * 4;
            int CalcLines = offsetBlockCount / 0x10;
            CalcLines += 1;
            offsetBlockCount = CalcLines * 0x10;

            long StartOffset = stream.Position;

            stream.Write(new byte[offsetBlockCount], 0, offsetBlockCount);

            uint firtOffset = (uint)offsetBlockCount;

            stream.Position = StartOffset;
            stream.Write(BitConverter.GetBytes(firtOffset), 0, 4);

            stream.Position = StartOffset + firtOffset;

            //
            long tempOffset = StartOffset + firtOffset;
            uint InternalOffset = firtOffset;

            for (int i = 0; i < BinCount; i++)
            {
                string binFilePath = Path.Combine(binPath, i.ToString("D4") + ".BIN");

                uint FileLength = 0;
                byte[] bin = new byte[0];
                if (File.Exists(binFilePath))
                {
                    try
                    {
                        FileInfo info = new FileInfo(binFilePath);
                        var read = info.OpenRead();
                        bin = new byte[read.Length];
                        read.Read(bin, 0, bin.Length);
                        FileLength = (uint)read.Length;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + i.ToString("D4") + ".BIN, unable to read the file;" + Environment.NewLine + ex.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Warning: " + i.ToString("D4") + ".BIN, file does not exist;");
                }

                stream.Position = tempOffset;
                stream.Write(bin, 0, bin.Length);

                tempOffset = stream.Position;

                stream.Position = StartOffset + (i * 4);
                stream.Write(BitConverter.GetBytes(InternalOffset), 0, 4);

                stream.Position = tempOffset;

                InternalOffset += FileLength;
            }

            // tpl

            uint TplOffset = (uint)stream.Position;

            stream.Position = 8;
            stream.Write(BitConverter.GetBytes(TplOffset), 0, 4);
            stream.Position = TplOffset;

            byte[] Tpl_Padding = new byte[0x10];
            Tpl_Padding[0] = 0x10;
            stream.Write(Tpl_Padding, 0, 0x10);

            long startTplOffset = stream.Position;

            //tpl file
            byte[] tpl = new byte[0];
            if (File.Exists(tplFilePath))
            {
                try
                {
                    FileInfo info = new FileInfo(tplFilePath);
                    var read = info.OpenRead();
                    tpl = new byte[read.Length];
                    read.Read(tpl, 0, tpl.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: TPL.TPL, unable to read the file;" + Environment.NewLine + ex.ToString());
                }
            }
            else
            {
                Console.WriteLine("Warning: TPL.TPL, file does not exist;");
            }

            stream.Position = startTplOffset;
            stream.Write(tpl, 0, tpl.Length);

            stream.Close();

        }


    }
}
