using SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK
{
    public static class MakeSMD_WithBinFolder
    {
        public static void CreateSMD(string baseDirectory, IdxPs2Scenario idxScenario)
        {
            // validações
            string binFolderPath = Path.Combine(baseDirectory, idxScenario.BinFolder);
            string smdFilePath = Path.GetFullPath(Path.Combine(baseDirectory, idxScenario.SmdFileName));
            string tplFileFullPath = Path.GetFullPath(Path.Combine(baseDirectory, idxScenario.TplFileName));

            if (Directory.Exists(binFolderPath) == false && idxScenario.SmdLinesDic.Any())
            {
                throw new ApplicationException("The content of the 'BinFolder' property is invalid.");
            }

            if (File.Exists(tplFileFullPath) == false)
            {
                throw new ApplicationException("The TPL file does not exist: " + Path.GetFileName(tplFileFullPath));
            }

            ValidateMagic.Validate(idxScenario.Magic);

            // pre processamento

            int smdLinesCount = idxScenario.SmdLinesDic.Any() ? idxScenario.SmdLinesDic.Max(a => a.Key) + 1 : 0;
            int binFilesCount = 0;
            int tplFilesCount = 0;

            SMDLine[] SmdLines = SmdLineParcer.ParserWithPart2(smdLinesCount, idxScenario.SmdLinesDic, idxScenario.SmdLinesPart2Dic, out binFilesCount, out tplFilesCount);

            Console.WriteLine("SMD Entry Count: " + smdLinesCount);
            Console.WriteLine("BIN Files Count: " + binFilesCount);
            Console.WriteLine("TPL Files Count: " + tplFilesCount);
            Console.WriteLine("Magic: " + idxScenario.Magic.ToString("X4"));

            long BinAreaOffset;
            BinaryWriter bw = new BinaryWriter(new FileInfo(smdFilePath).Create());
            MakeSMD_Top.FillTopSmd(bw, idxScenario.Magic, SmdLines, 0, out BinAreaOffset);

            //---------------------------
            // PARTE DOS ARQUIVOS BINs

            int BinOffsetBlockCount = (((binFilesCount * 4) + 15) / 16) * 16;

            bw.Write(new byte[BinOffsetBlockCount]);

            long OffsetToOffsetBin = BinAreaOffset;
            long RealOffsetBin = BinOffsetBlockCount;

            for (int i = 0; i < binFilesCount; i++)
            {
                bw.BaseStream.Position = OffsetToOffsetBin;
                bw.Write((uint)RealOffsetBin);
                bw.BaseStream.Position = BinAreaOffset + RealOffsetBin;

                string binFilePath = Path.Combine(binFolderPath, i.ToString("D4") + ".BIN");

                long tempStart = bw.BaseStream.Position;
                long tempEnd = tempStart;

                if (File.Exists(binFilePath))
                {
                    try
                    {
                        MemoryStream ms = new MemoryStream();

                        FileInfo info = new FileInfo(binFilePath);
                        var read = info.OpenRead();
                        read.CopyTo(ms);
                        read.Close();

                        // alinhamento do bin
                        int _padding = (int)((16 - (ms.Position % 16)) % 16);
                        ms.Write(new byte[_padding], 0, _padding);

                        //verifica o magic
                        ms.Position = 0;
                        BinaryReader br = new BinaryReader(ms);
                        ushort binMagic = br.ReadUInt16();
                        ms.Position = 0x10;
                        uint binPadding1 = br.ReadUInt32();

                        if (binMagic != 0x0030 && binPadding1 != 0xCDCDCDCD)
                        {
                            throw new ApplicationException("The BIN file is from a different version of the SMD that is being repacked.");
                        }

                        // copia
                        ms.Position = 0;
                        ms.CopyTo(bw.BaseStream);
                        ms.Close();
                        tempEnd = bw.BaseStream.Position;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error in " + i.ToString("D4") + ".BIN: " + Environment.NewLine + ex.Message);
                        PutEmptyFiles.PutBin(bw.BaseStream, tempStart, out tempEnd);
                    }
                }
                else
                {
                    Console.WriteLine($"Error in " + i.ToString("D4") + ".BIN: " + Environment.NewLine + "File not Exist!");
                    PutEmptyFiles.PutBin(bw.BaseStream, tempStart, out tempEnd);
                }

                OffsetToOffsetBin += 4;
                RealOffsetBin = (bw.BaseStream.Position - BinAreaOffset);
            }

            //---------------------------
            // PARTE DOS ARQUIVOS TPLs
            long TplAreaOffset = bw.BaseStream.Position;
            MakeSMD_Fill_TPL_PS2 makeSMD_Fill_TPL_PS2 = new MakeSMD_Fill_TPL_PS2(tplFileFullPath);
            makeSMD_Fill_TPL_PS2.Fill(bw, tplFilesCount, TplAreaOffset, out _);

            //coloca os offsets no topo
            bw.BaseStream.Position = 4;
            bw.Write((uint)BinAreaOffset);
            bw.Write((uint)TplAreaOffset);

            bw.Close();
        }

    }
}
