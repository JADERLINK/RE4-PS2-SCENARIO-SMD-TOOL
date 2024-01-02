using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.EXTRACT;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO
{
    public static class Ps2ScenarioExtract
    {
        public static SMDLine[] Extract(FileInfo SMDinfo, out Dictionary<int, BIN> BinDic, out Dictionary<int, BinRenderBox> Boxes, out int BinRealCount) 
        {
            string FileBaseName = SMDinfo.Name.Substring(0, SMDinfo.Name.Length - SMDinfo.Extension.Length);
            string BaseDirectory = SMDinfo.DirectoryName + "\\";
            string BaseBinDirectory = BaseDirectory + FileBaseName + "_BIN\\";

            Directory.CreateDirectory(BaseBinDirectory);

            BinaryReader br = new BinaryReader(SMDinfo.OpenRead());

            ushort Magic = br.ReadUInt16();
            ushort SmdLength = br.ReadUInt16();
            uint offsetBIN = br.ReadUInt32();
            uint offsetTPL = br.ReadUInt32();
            uint none1 = br.ReadUInt32();

            if (! (Magic == 0x0040 || Magic == 0x0031))
            {
                throw new ArgumentException("Invalid SMD file");
            }

            SMDLine[] SMDLines = new SMDLine[SmdLength];

            //64 bytes de tamanho

            for (int i = 0; i < SmdLength; i++)
            {
                SMDLine smdLine = new SMDLine();

                byte[] line = new byte[64];
                br.BaseStream.Read(line, 0, 64);

                float positionX = BitConverter.ToSingle(line, 0);
                float positionY = BitConverter.ToSingle(line, 4);
                float positionZ = BitConverter.ToSingle(line, 8);
                float positionW = BitConverter.ToSingle(line, 12);
                float angleX = BitConverter.ToSingle(line, 16);
                float angleY = BitConverter.ToSingle(line, 20);
                float angleZ = BitConverter.ToSingle(line, 24);
                float angleW = BitConverter.ToSingle(line, 28);
                float scaleX = BitConverter.ToSingle(line, 32);
                float scaleY = BitConverter.ToSingle(line, 36);
                float scaleZ = BitConverter.ToSingle(line, 40);
                float scaleW = BitConverter.ToSingle(line, 44);
                ushort BinID = BitConverter.ToUInt16(line, 48);
                byte FixedFF = line[50];
                byte SmxID = line[51];
                uint unused1 = BitConverter.ToUInt32(line, 52);
                uint objectStatus = BitConverter.ToUInt32(line, 56);
                uint unused2 = BitConverter.ToUInt32(line, 60);

                smdLine.positionX = positionX;
                smdLine.positionY = positionY;
                smdLine.positionZ = positionZ;
                smdLine.positionW = positionW;
                smdLine.angleX = angleX;
                smdLine.angleY = angleY;
                smdLine.angleZ = angleZ;
                smdLine.angleW = angleW;
                smdLine.scaleX = scaleX;
                smdLine.scaleY = scaleY;
                smdLine.scaleZ = scaleZ;
                smdLine.scaleW = scaleW;
                smdLine.BinID = BinID;
                smdLine.FixedFF = FixedFF;
                smdLine.SmxID = SmxID;
                smdLine.unused1 = unused1;
                smdLine.objectStatus = objectStatus;
                smdLine.unused2 = unused2;
                SMDLines[i] = smdLine;
            }

            //-------------------

            br.BaseStream.Position = offsetBIN;

            List<uint> BinOffsetList = new List<uint>();

            uint uBinFirt = br.ReadUInt32();
            BinOffsetList.Add(uBinFirt);

            int quantityOfTheNext = (int)(uBinFirt - 4) / 4;

            for (int i = 0; i < quantityOfTheNext; i++)
            {
                uint tempOffset = br.ReadUInt32();
                BinOffsetList.Add(tempOffset);
            }

            BinDic = new Dictionary<int, BIN>();
            Boxes = new Dictionary<int, BinRenderBox>();
            BinRealCount = 0;

            for (int i = 0; i < BinOffsetList.Count; i++)
            {
                if (BinOffsetList[i] != 0)
                {
                    BinRealCount = i+1;

                    long startOffset = offsetBIN + BinOffsetList[i];
                    long endOffset = startOffset;

                    try
                    {
                        var bin = BINdecoder.Decode(br.BaseStream, startOffset, out endOffset, new AltTextWriter("", false));
                        BinDic.Add(i, bin);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error on Read Bin in Smd: " + i.ToString("D3") + Environment.NewLine + ex.ToString());
                    }

                    //-------

                    //le os bytes do bin
                    br.BaseStream.Position = startOffset;
                    int lenght = (int)(endOffset - startOffset);

                    byte[] binArray = new byte[lenght];
                    br.BaseStream.Read(binArray, 0, (int)lenght);
                    //---

                    BinRenderBox box = new BinRenderBox();
                    box.DrawDistanceNegativeX = BitConverter.ToSingle(binArray, 0x30);
                    box.DrawDistanceNegativeY = BitConverter.ToSingle(binArray, 0x34);
                    box.DrawDistanceNegativeZ = BitConverter.ToSingle(binArray, 0x38);

                    box.DrawDistancePositiveX = BitConverter.ToSingle(binArray, 0x40);
                    box.DrawDistancePositiveY = BitConverter.ToSingle(binArray, 0x44);
                    box.DrawDistancePositiveZ = BitConverter.ToSingle(binArray, 0x48);

                    Boxes.Add(i, box);

                    //-----
                    try
                    {
                        // e grava em um arquivo
                        string binPath = BaseBinDirectory + i.ToString("D4") + ".BIN";
                        File.WriteAllBytes(binPath, binArray);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error on write in file: " + i.ToString("D3") + ".BIN" + Environment.NewLine + ex.ToString());
                    }



                }
                
            }

            //------------------------------
            //TPL offset

            br.BaseStream.Position = offsetTPL;

            uint tplOffset = br.ReadUInt32();
            tplOffset = tplOffset + offsetTPL;
            
            byte[] tplArray = new byte[0];

            try
            {
                //le os bytes do tpl e grava em um arquivo
                br.BaseStream.Position = tplOffset;
                int tplLenght = (int)(br.BaseStream.Length - tplOffset);

                tplArray = new byte[tplLenght];
                br.BaseStream.Read(tplArray, 0, tplLenght);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on Read: Tpl in Smd" + Environment.NewLine + ex.ToString());
            }

            try
            {
                string tplPath = BaseDirectory + FileBaseName + ".TPL";
                File.WriteAllBytes(tplPath, tplArray);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error on write in file: TPL.TPL" + Environment.NewLine + ex.ToString());
            }

            br.Close();
            return SMDLines;
        }



        public static IdxMaterial IdxMaterialMultParser(Dictionary<int, BIN> BinDic, out Dictionary<MaterialPart, string> invDic)
        {
            IdxMaterial idx = new IdxMaterial();
            idx.MaterialDic = new Dictionary<string, MaterialPart>();
            invDic = new Dictionary<MaterialPart, string>();

            int counter = 0;

            foreach (var item in BinDic)
            {
                if (item.Value != null)
                {
                    for (int i = 0; i < item.Value.materials.Length; i++)
                    {
                        var mat = new MaterialPart(item.Value.materials[i].materialLine);

                        if (!invDic.ContainsKey(mat))
                        {
                            string matKey = CONSTs.SCENARIO_MATERIAL + counter.ToString("D3");
                            invDic.Add(mat, matKey);
                            idx.MaterialDic.Add(matKey, mat);
                            counter++;
                        }

                    }
                }

            }
            return idx;
        }
    }




    public class SMDLine
    {
        public float positionX;
        public float positionY;
        public float positionZ;
        public float positionW;
        public float angleX;
        public float angleY;
        public float angleZ;
        public float angleW;
        public float scaleX;
        public float scaleY;
        public float scaleZ;
        public float scaleW;

        public ushort BinID;
        public byte FixedFF;
        public byte SmxID;
        public uint unused1;
        public uint objectStatus;
        public uint unused2;
    }


    public class BinRenderBox
    {
        public float DrawDistanceNegativeX;
        public float DrawDistanceNegativeY;
        public float DrawDistanceNegativeZ;

        public float DrawDistancePositiveX;
        public float DrawDistancePositiveY;
        public float DrawDistancePositiveZ;
    }


}
