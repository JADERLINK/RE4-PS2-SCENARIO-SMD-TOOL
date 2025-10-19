using SHARED_PS2_BIN.ALL;
using SHARED_PS2_BIN.REPACK;
using SHARED_PS2_BIN.REPACK.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK.WithOBJ
{
    public class MakeSMD_Fill_BIN_PS2
    {
        // binID, FinalStructure
        private Dictionary<int, FinalStructure> FinalBinDic = null;
        private Dictionary<int, RepackProps> RepackPropsDic = null;
        private Dictionary<int, float> ConversionFactorValueDic = null;
        private IdxMaterial material;

        private bool CreateBinFilesInFolder;
        private string BinfolderPath;

        public MakeSMD_Fill_BIN_PS2(Dictionary<int, FinalStructure> finalBinDic, Dictionary<int, RepackProps> repackPropsDic, Dictionary<int, float> conversionFactorValueDic, IdxMaterial material, bool createBinFilesInFolder, string binfolderPath)
        {
            FinalBinDic = finalBinDic;
            RepackPropsDic = repackPropsDic;
            ConversionFactorValueDic = conversionFactorValueDic;
            this.material = material;
            CreateBinFilesInFolder = createBinFilesInFolder;
            BinfolderPath = binfolderPath;
        }

        public void Fill(BinaryWriter bw, int binFilesCount, long BinAreaOffset, out long endOffset)
        {
            //---------------------------
            // PARTE DOS ARQUIVOS BINs

            int BinOffsetBlockCount = (((binFilesCount * 4) + 15) / 16) * 16;

            bw.BaseStream.Position = BinAreaOffset;
            bw.Write(new byte[BinOffsetBlockCount]);

            long OffsetToOffsetBin = BinAreaOffset;
            long RealOffsetBin = BinOffsetBlockCount;

            for (int i = 0; i < binFilesCount; i++)
            {
                bw.BaseStream.Position = OffsetToOffsetBin;
                bw.Write((uint)RealOffsetBin);
                long startBinOffset = BinAreaOffset + RealOffsetBin;
                long endBinOffset;

                PutBin(bw, i, startBinOffset, out endBinOffset);

                OffsetToOffsetBin += 4;
                RealOffsetBin = (endBinOffset - BinAreaOffset);
                bw.BaseStream.Position = endBinOffset;
            }

            endOffset = bw.BaseStream.Position;
        }

        private void PutBin(BinaryWriter bw, int binId, long startBinOffset, out long endBinOffset)
        {
            long outOffset = startBinOffset;

            if (FinalBinDic.ContainsKey(binId))
            {
                //boneLine
                FinalBoneLine[] boneLineArray = new FinalBoneLine[1];
                boneLineArray[0] = new FinalBoneLine(0, 0xFF, 0, 0, 0);

                Console.WriteLine("BIN ID: " + binId.ToString("D3"));
                BINmakeFile.MakeFinalBinFile(bw.BaseStream, startBinOffset, out outOffset, FinalBinDic[binId], RepackPropsDic[binId], boneLineArray, ConversionFactorValueDic[binId], material);
            }
            else
            {
                PutEmptyFiles.PutBin(bw.BaseStream, startBinOffset, out outOffset);
            }

            if (CreateBinFilesInFolder)
            {
                try
                {
                    Directory.CreateDirectory(BinfolderPath);

                    //--salva em um arquivo
                    Stream stream = bw.BaseStream;
                    stream.Position = startBinOffset;
                    int lenght = (int)(outOffset - startBinOffset);
                    byte[] bin = new byte[lenght];
                    stream.Read(bin, 0, lenght);
                    File.WriteAllBytes(Path.Combine(BinfolderPath, binId.ToString("D4") + ".BIN"), bin);
                    stream.Position = outOffset;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error on write in file: " + binId.ToString("D3") + ".BIN" + Environment.NewLine + ex.ToString());
                }

            }

            endBinOffset = outOffset;
        }
    }
}
