using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SMD_PS2_Extractor
{
    /*
    Codigo feito por JADERLINK
    Pesquisas feitas por HardHain e JaderLink.
    https://www.youtube.com/@JADERLINK
    https://www.youtube.com/@HardRainModder

    Em desenvolvimento
    Para Pesquisas
    12-03-2023
    version: alfa.1.0.0.0
    */

    public static class SMDPS2Extractor
    {

        public static void Extrator(string Filename, bool EnableDebugInfo = false)
        {

            FileInfo info = new FileInfo(Filename);

            string ParentDirectory = info.Directory.FullName;
            if (ParentDirectory[ParentDirectory.Length - 1] != '\\')
            {
                ParentDirectory += "\\";
            }

            string NameSMD = info.Name.Substring(0, info.Name.Length - info.Extension.Length);
            if (NameSMD == null || NameSMD.Length == 0)
            {
                NameSMD = "null";
            }
            
            string baseid = "r000";
            var split = NameSMD.Split('_');
            if (split != null && split.Length >= 1)
            {
                baseid = split[0];
            }


            //Directory
            Directory.CreateDirectory(ParentDirectory + NameSMD);
            Directory.CreateDirectory(ParentDirectory + NameSMD + "\\Models");

            if (EnableDebugInfo)
            {
                Directory.CreateDirectory(ParentDirectory + NameSMD + "\\DebugInfo");
            }


            var res = new BINdecoderTest.AltTextWriter(ParentDirectory + NameSMD + "\\DebugInfo\\" + "_" +NameSMD + ".txt2", EnableDebugInfo);
            res.WriteLine("##SMD_PS2_Extractor##");
            res.WriteLine("##version A.1.0.0.0##");
            res.WriteLine(Filename);

            var IdxSmd = new StreamWriter(ParentDirectory + NameSMD + ".idxSmdPs2");
            IdxSmd.WriteLine(":##SMD_PS2_Extractor##");
            IdxSmd.WriteLine(":##version A.1.0.0.0##");

            var file = new FileStream(Filename, FileMode.Open);

            byte[] b0x4000 = new byte[2];
            file.Read(b0x4000, 0, 2);

            res.WriteLine("Fix4000: " + BitConverter.ToString(b0x4000).Replace("-", ""));

            byte[] listLenght = new byte[2];
            file.Read(listLenght, 0, 2);
            ushort length = BitConverter.ToUInt16(listLenght, 0);

            res.WriteLine("smdLength: " + length);
            

            byte[] offsetbin = new byte[4];
            byte[] offsettpl = new byte[4];
            byte[] none1 = new byte[4];

            file.Read(offsetbin, 0, 4);
            file.Read(offsettpl, 0, 4);
            file.Read(none1, 0, 4);

            uint Uoffsetbin = BitConverter.ToUInt32(offsetbin, 0);
            uint Uoffsettpl = BitConverter.ToUInt32(offsettpl, 0);

            offsetbin = offsetbin.Reverse().ToArray();
            offsettpl = offsettpl.Reverse().ToArray();
            none1 = none1.Reverse().ToArray();

            res.WriteLine("Offset .bin: 0x" + BitConverter.ToString(offsetbin).Replace("-", ""));
            res.WriteLine("Offset .tpl: 0x" + BitConverter.ToString(offsettpl).Replace("-", ""));
            res.WriteLine("none1: 0x" + BitConverter.ToString(none1).Replace("-", ""));


            SMDLine[] SMDLines = new SMDLine[length];

            //64 bytes de tamanho
            for (int i = 0; i < length; i++)
            {
                SMDLine smdLine = new SMDLine();

                byte[] line = new byte[64];
                file.Read(line, 0, 64);

                res.WriteLine("");
                res.WriteLine("Order: " + i);

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

                res.WriteLine("BinID: " + BinID);
                res.WriteLine("FixedFF: 0x" + FixedFF.ToString("X2"));
                res.WriteLine("SmxID: " + SmxID);
                res.WriteLine("unused1: 0x" + unused1.ToString("X8"));
                res.WriteLine("objectStatus: 0x" + objectStatus.ToString("X8"));
                res.WriteLine("unused2: 0x" + unused2.ToString("X8"));

                res.WriteLine("positionX: " + positionX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                res.WriteLine("positionY: " + positionY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                res.WriteLine("positionZ: " + positionZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                res.WriteLine("positionW: " + positionW.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

                res.WriteLine("angleX: " + angleX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                res.WriteLine("angleY: " + angleY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                res.WriteLine("angleZ: " + angleZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                res.WriteLine("angleW: " + angleW.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

                res.WriteLine("scaleX: " + scaleX.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                res.WriteLine("scaleY: " + scaleY.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                res.WriteLine("scaleZ: " + scaleZ.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                res.WriteLine("scaleW: " + scaleW.ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

                IdxSmd.WriteLine("");
                IdxSmd.WriteLine(i.ToString("D3") + "_BinID:" + BinID);
                IdxSmd.WriteLine(i.ToString("D3") + "_FixedFF:" + FixedFF.ToString("X2"));
                IdxSmd.WriteLine(i.ToString("D3") + "_SmxID:" + SmxID);
                IdxSmd.WriteLine(i.ToString("D3") + "_unused1:" + unused1.ToString("X8"));
                IdxSmd.WriteLine(i.ToString("D3") + "_objectStatus:" + objectStatus.ToString("X8"));
                IdxSmd.WriteLine(i.ToString("D3") + "_unused2:" + unused2.ToString("X8"));

                IdxSmd.WriteLine(i.ToString("D3") + "_positionX:" + positionX.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));
                IdxSmd.WriteLine(i.ToString("D3") + "_positionY:" + positionY.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));
                IdxSmd.WriteLine(i.ToString("D3") + "_positionZ:" + positionZ.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));
                IdxSmd.WriteLine(i.ToString("D3") + "_positionW:" + positionW.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));

                IdxSmd.WriteLine(i.ToString("D3") + "_angleX:" + angleX.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));
                IdxSmd.WriteLine(i.ToString("D3") + "_angleY:" + angleY.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));
                IdxSmd.WriteLine(i.ToString("D3") + "_angleZ:" + angleZ.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));
                IdxSmd.WriteLine(i.ToString("D3") + "_angleW:" + angleW.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));

                IdxSmd.WriteLine(i.ToString("D3") + "_scaleX:" + scaleX.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));
                IdxSmd.WriteLine(i.ToString("D3") + "_scaleY:" + scaleY.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));
                IdxSmd.WriteLine(i.ToString("D3") + "_scaleZ:" + scaleZ.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));
                IdxSmd.WriteLine(i.ToString("D3") + "_scaleW:" + scaleW.ToString("G9", System.Globalization.CultureInfo.InvariantCulture));


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

            res.WriteLine("");
            res.WriteLine("");
            res.WriteLine("offset: 0x" + file.Position.ToString("X8"));
            res.WriteLine("Indo para o offset dos bins: 0x" + Uoffsetbin.ToString("X8"));


            int BinRealCount = 0;

            List<KeyValuePair<int, int>> ListBinPos = new List<KeyValuePair<int, int>>();

            file.Position = Uoffsetbin;

            byte[] BinFirt = new byte[4];
            file.Read(BinFirt, 0, 4);

            uint uBinFirt = BitConverter.ToUInt32(BinFirt, 0);

            int tamanhoBloco = (int)uBinFirt - 4;
            byte[] BlocotamanhosBins = new byte[tamanhoBloco];
            file.Read(BlocotamanhosBins, 0, tamanhoBloco);

            int Quantidade = tamanhoBloco / 4;
            res.WriteLine("Quantidade: " + (Quantidade + 1));
            //res.WriteLine("0: Offset: 0x" + uBinFirt.ToString("X8"));

            ListBinPos.Add(new KeyValuePair<int, int>((int)uBinFirt, 0));

            int tempCount = 0;
            for (int i = 0; i < Quantidade; i++)
            {
                int offsetBin = BitConverter.ToInt32(BlocotamanhosBins, tempCount);
                //res.WriteLine((i+1) +": Offset: 0x" + offsetBin.ToString("X8"));
                tempCount += 4;
                ListBinPos.Add(new KeyValuePair<int, int>(offsetBin, 0));
                if (offsetBin != 0)
                {
                    ListBinPos[i] = new KeyValuePair<int, int>(ListBinPos[i].Key, (offsetBin - ListBinPos[i].Key));
                }
                else
                {
                    ListBinPos[i] = new KeyValuePair<int, int>(ListBinPos[i].Key, (int)(Uoffsettpl - Uoffsetbin - ListBinPos[i].Key));
                }
            }

            ListBinPos[Quantidade] = new KeyValuePair<int, int>(ListBinPos[Quantidade].Key, (int)(Uoffsettpl - Uoffsetbin - ListBinPos[Quantidade].Key));

            BINdecoderTest.BIN[]  bins = new BINdecoderTest.BIN[ListBinPos.Count];

            for (int i = 0; i < ListBinPos.Count; i++)
            {
                res.WriteLine(i + ": Offset: 0x" + ListBinPos[i].Key.ToString("X8") + "    Tamanho: 0x" + ListBinPos[i].Value.ToString("X8"));

                if (ListBinPos[i].Key != 0)
                {
                    BinRealCount++;

                    Console.WriteLine("Creating: " + NameSMD + "\\" + baseid + "_" + i + ".BIN");

                    file.Position = Uoffsetbin + ListBinPos[i].Key;
                    byte[] arqBin = new byte[ListBinPos[i].Value];
                    file.Read(arqBin, 0, (int)ListBinPos[i].Value);
                    File.WriteAllBytes(ParentDirectory + NameSMD + "\\" + baseid + "_" + i + ".BIN", arqBin);

                    //BIN
                    Stream stream = new MemoryStream(arqBin);

                    BINdecoderTest.BIN bin = BINdecoderTest.BINdecoder.Decode(stream, ParentDirectory + NameSMD + "\\DebugInfo\\" + baseid + "_" + i + ".BIN", false, EnableDebugInfo);
                    BINdecoderTest.BINdecoder.CreateObjMtl(bin, ParentDirectory + NameSMD + "\\Models", baseid + "_" + i, baseid);
                    BINdecoderTest.BINdecoder.CreateIdxbin(bin, ParentDirectory + NameSMD + "\\Models", baseid + "_" + i);

                    if (EnableDebugInfo)
                    {
                        BINdecoderTest.BINdecoder.CreateDrawDistanceBoxObj(bin, ParentDirectory + NameSMD + "\\DebugInfo\\", baseid + "_" + i);
                        BINdecoderTest.BINdecoder.CreateScaleLimitBoxObj(bin, ParentDirectory + NameSMD + "\\DebugInfo\\", baseid + "_" + i);
                    }
                    

                    bins[i] = bin;
                }

            }

          

            file.Position = Uoffsettpl;

            // tpl
            int tamanhoTPL = (int)(file.Length - Uoffsettpl) - 0x10;

            byte[] padding = new byte[0x10];
            file.Read(padding, 0, 0x10);
            //File.WriteAllBytes(ParentDirectory + NameSMD + "\\" + baseid + "_Padding.hex", padding);

            byte[] arqTPL = new byte[tamanhoTPL];
            file.Read(arqTPL, 0, (int)tamanhoTPL);
            File.WriteAllBytes(ParentDirectory + NameSMD + "\\" + baseid + ".TPL", arqTPL);

            CreateObjMtlScenario(SMDLines, bins, ParentDirectory + NameSMD + "\\Models", baseid);

            IdxSmd.WriteLine("");
            IdxSmd.WriteLine("SmdCount:" + length);
            IdxSmd.WriteLine("BinCount:" + BinRealCount);
            IdxSmd.WriteLine("BinBaseName:" + NameSMD + "\\" + baseid + "_");
            IdxSmd.WriteLine("TplFilePatch:" + NameSMD + "\\" + baseid + ".TPL");
          
            file.Close();
            IdxSmd.Close();
            res.Close();

        }

        private static void CreateObjMtlScenario(SMDLine[] SMDLines, BINdecoderTest.BIN[] bins, string baseDiretory, string baseFileName)
        {
            if (baseDiretory[baseDiretory.Length - 1] != '\\')
            {
                baseDiretory += "\\";
            }

            TextWriter MTLtext = new FileInfo(baseDiretory + baseFileName + "_Model.mtl").CreateText();
            MTLtext.WriteLine("##SMD_PS2_Extractor##");
            MTLtext.WriteLine("##Version A.1.0.0.0##");
            MTLtext.WriteLine("");

            for (int b = 0; b < bins.Length; b++)
            {
                BINdecoderTest.BIN bin = bins[b];

                if (bin != null)
                {
                    for (int i = 0; i < bin.materials.Length; i++)
                    {
                        MTLtext.WriteLine("");
                        MTLtext.WriteLine("newmtl " + baseFileName + "_" + b + "_" + i);
                        MTLtext.WriteLine("Ka 1.000 1.000 1.000");
                        MTLtext.WriteLine("Kd 1.000 1.000 1.000");
                        MTLtext.WriteLine("Ks 0.000 0.000 0.000");
                        MTLtext.WriteLine("Ns 0");
                        MTLtext.WriteLine("d 1");
                        MTLtext.WriteLine("Tr 1");
                        MTLtext.WriteLine("map_Kd Textures/" + baseFileName + "_" + bin.materials[i].materialLine[1].ToString() + ".tga");
                        MTLtext.WriteLine("");
                    }
                }
            }
            MTLtext.Close();


            TextWriter text = new FileInfo(baseDiretory + baseFileName + "_Model.obj").CreateText();
            text.WriteLine("##SMD_PS2_Extractor##");
            text.WriteLine("##version A.1.0.0.0##");
            text.WriteLine("mtllib " + baseFileName + "_Model.mtl");

            int indexGeral = 1;

            for (int i = 0; i < SMDLines.Length; i++)
            {
                DrawScenarioPart(ref text, SMDLines[i], bins[SMDLines[i].BinID], ref indexGeral, baseFileName, i);
            }

            text.Close();

        }

        private static void DrawScenarioPart(ref TextWriter text, SMDLine SMDLine, BINdecoderTest.BIN bin, ref int indexGeral, string baseFileName, int smdID)
        {
            text.WriteLine("o SMD_" + smdID.ToString("D3") + "_BIN_" + SMDLine.BinID.ToString("D3") +"_SMX_" + SMDLine.SmxID.ToString("D3") + "_Type_0x" + SMDLine.objectStatus.ToString("X2"));

            for (int t = 0; t < bin.Nodes.Length; t++)
            {

                text.WriteLine("g " + "SMD_" + smdID.ToString("D3") + "_BIN_" + SMDLine.BinID.ToString("D3") + "_MATERIAL_" + t.ToString("D3"));
                text.WriteLine("usemtl " + baseFileName + "_" + SMDLine.BinID + "_" + t);

                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    for (int l = 0; l < bin.Nodes[t].Segments[i].vertexLines.Length; l++)
                    {
                        BINdecoderTest.VertexLine vertexLine = bin.Nodes[t].Segments[i].vertexLines[l];

                        float[] pos = new float[3]; // 0 = x, 1 = y, 2 = z

                        //float posX = (((float)vertexLine.VerticeX * bin.Nodes[t].Segments[i].Scale) * SMDLine.scaleX) + SMDLine.posX;
                        //float posY = (((float)vertexLine.VerticeY * bin.Nodes[t].Segments[i].Scale) * SMDLine.scaleY) + SMDLine.posY;
                        //float posZ = (((float)vertexLine.VerticeZ * bin.Nodes[t].Segments[i].Scale) * SMDLine.scaleZ) + SMDLine.posZ;

                        pos[0] = (float)vertexLine.VerticeX * bin.Nodes[t].Segments[i].Scale;
                        pos[1] = (float)vertexLine.VerticeY * bin.Nodes[t].Segments[i].Scale;
                        pos[2] = (float)vertexLine.VerticeZ * bin.Nodes[t].Segments[i].Scale;

                        //XYZ
                        
                        pos = Utils.RotationInX(pos, SMDLine.angleX);
                        pos = Utils.RotationInY(pos, SMDLine.angleY);
                        pos = Utils.RotationInZ(pos, SMDLine.angleZ);
                       
                        pos[0] = (pos[0] * SMDLine.scaleX) + SMDLine.positionX;
                        pos[1] = (pos[1] * SMDLine.scaleY) + SMDLine.positionY;
                        pos[2] = (pos[2] * SMDLine.scaleZ) + SMDLine.positionZ;

                        text.WriteLine("v " + pos[0].ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             pos[1].ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             pos[2].ToString("f9", System.Globalization.CultureInfo.InvariantCulture));

                        text.WriteLine("vt " + ((float)vertexLine.TextureU / 255f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                        ((float)vertexLine.TextureV / 255f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));
                        
                        text.WriteLine("vn " + ((float)vertexLine.NormalX / 127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             ((float)vertexLine.NormalY / 127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture) + " " +
                             ((float)vertexLine.NormalZ / 127f).ToString("f9", System.Globalization.CultureInfo.InvariantCulture));


                    }


                    bool invFace = false;
                    int contagem = 0;
                    while (contagem < bin.Nodes[t].Segments[i].vertexLines.Length)
                    {

                        string a = (indexGeral - 2).ToString();
                        string b = (indexGeral - 1).ToString();
                        string c = (indexGeral).ToString();


                        if ((contagem - 2) > -1
                            &&
                           (bin.Nodes[t].Segments[i].vertexLines[contagem].IndexComplement == 0)
                           )
                        {

                            if (invFace)
                            {
                                if (bin.binType != BINdecoderTest.BinType.ScenarioWithColors)
                                {
                                    text.WriteLine("f " + c + "/" + c + "/" + c + " " +
                                                          b + "/" + b + "/" + b + " " +
                                                          a + "/" + a + "/" + a
                                                          );
                                }
                                else
                                {

                                    text.WriteLine("f " + c + "/" + c + " " +
                                                          b + "/" + b + " " +
                                                          a + "/" + a
                                                          );
                                }

                                invFace = false;
                            }
                            else
                            {
                                if (bin.binType != BINdecoderTest.BinType.ScenarioWithColors)
                                {
                                    text.WriteLine("f " + a + "/" + a + "/" + a + " " +
                                                          b + "/" + b + "/" + b + " " +
                                                          c + "/" + c + "/" + c
                                                          );
                                }
                                else 
                                {
                                    text.WriteLine("f " + a + "/" + a + " " +
                                                          b + "/" + b + " " +
                                                          c + "/" + c
                                                          );
                                }

                                invFace = true;
                            }


                        }
                        else
                        {
                            invFace = false;
                        }


                        contagem++;
                        indexGeral++;
                    }



                }

            }
        }

    }


    public static class Utils 
    {
        //pos = x, y, z

        public static float[] RotationInX(float[] pos, float angleInRadian) 
        {
            float[] res = new float[3];
            res[0] = pos[0];
            res[1] = (float)(Math.Cos(angleInRadian) * pos[1] - Math.Sin(angleInRadian) * pos[2]);
            res[2] = (float)(Math.Sin(angleInRadian) * pos[1] + Math.Cos(angleInRadian) * pos[2]);
            return res;
        }

        public static float[] RotationInY(float[] pos, float angleInRadian)
        {
            float[] res = new float[3];
            res[0] = (float)(Math.Cos(angleInRadian) * pos[0] + Math.Sin(angleInRadian) * pos[2]);
            res[1] = pos[1];
            res[2] = (float)(-1 * Math.Sin(angleInRadian) * pos[0] + Math.Cos(angleInRadian) * pos[2]);
            return res;
        }

        public static float[] RotationInZ(float[] pos, float angleInRadian)
        {
            float[] res = new float[3];
            res[0] = (float)(Math.Cos(angleInRadian) * pos[0] - Math.Sin(angleInRadian) * pos[1]);
            res[1] = (float)(Math.Sin(angleInRadian) * pos[0] + Math.Cos(angleInRadian) * pos[1]);
            res[2] = pos[2];
            return res;
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

}
