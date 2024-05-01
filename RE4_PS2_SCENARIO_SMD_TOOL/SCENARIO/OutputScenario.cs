using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_PS2_BIN_TOOL.EXTRACT;
using RE4_PS2_BIN_TOOL.ALL;
using System.IO;

namespace RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO
{
    public static class OutputScenario
    {
        public static void CreateObjScenario(SMDLine[] SMDLines, Dictionary<int, PS2BIN> bins, Dictionary<MaterialPart, string> materialList, string baseDiretory, string baseFileName)
        {
            TextWriter text = new FileInfo(baseDiretory + baseFileName + ".obj").CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("mtllib " + baseFileName + ".mtl");

            uint indexCount = 1;

            for (int i = 0; i < SMDLines.Length; i++)
            {
                if (bins.ContainsKey(SMDLines[i].BinID))
                {
                    DrawScenarioPart(ref text, SMDLines[i], bins[SMDLines[i].BinID], materialList, ref indexCount, i);
                }

            }

            text.Close();

        }

        private static void DrawScenarioPart(ref TextWriter text, SMDLine SMDLine, PS2BIN bin, Dictionary<MaterialPart, string> materialList, ref uint indexCount, int smdID)
        {

            string g = "g PS2SCENARIO#SMD_" + smdID.ToString("D3") + "#SMX_" + SMDLine.SmxID.ToString("D3") + "#TYPE_" + SMDLine.objectStatus.ToString("X2") + "#BIN_" + SMDLine.BinID.ToString("D3") + "#";

            if (bin.binType == BinType.ScenarioWithColors)
            {
                g += "COLOR#";
            }
            else
            {
                g += "NORMAL#";
            }
            text.WriteLine(g);

            for (int t = 0; t < bin.Nodes.Length; t++)
            {
                text.WriteLine("usemtl " + materialList[new MaterialPart(bin.materials[t].materialLine)]);

                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    for (int l = 0; l < bin.Nodes[t].Segments[i].vertexLines.Length; l++)
                    {
                        VertexLine vertexLine = bin.Nodes[t].Segments[i].vertexLines[l];

                        float[] pos = new float[3]; // 0 = x, 1 = y, 2 = z

                        pos[0] = (float)vertexLine.VerticeX * bin.Nodes[t].Segments[i].ConversionFactorValue;
                        pos[1] = (float)vertexLine.VerticeY * bin.Nodes[t].Segments[i].ConversionFactorValue;
                        pos[2] = (float)vertexLine.VerticeZ * bin.Nodes[t].Segments[i].ConversionFactorValue;

                        //XYZ

                        pos = RotationUtils.RotationInX(pos, SMDLine.angleX);
                        pos = RotationUtils.RotationInY(pos, SMDLine.angleY);
                        pos = RotationUtils.RotationInZ(pos, SMDLine.angleZ);

                        pos[0] = ((pos[0] * SMDLine.scaleX) + SMDLine.positionX) / CONSTs.GLOBAL_SCALE;
                        pos[1] = ((pos[1] * SMDLine.scaleY) + SMDLine.positionY) / CONSTs.GLOBAL_SCALE;
                        pos[2] = ((pos[2] * SMDLine.scaleZ) + SMDLine.positionZ) / CONSTs.GLOBAL_SCALE;

                        string v = "v " + pos[0].ToFloatString()
                         + " " + pos[1].ToFloatString()
                         + " " + pos[2].ToFloatString();

                        if (bin.binType == BinType.ScenarioWithColors)
                        {
                            // nesse caso o valor no campo normal, na verdade são cores
                            v += " " + (vertexLine.NormalX / 128f).ToFloatString() + 
                                 " " + (vertexLine.NormalY / 128f).ToFloatString() + 
                                 " " + (vertexLine.NormalZ / 128f).ToFloatString() + 
                                 " " + (vertexLine.UnknownB / 128f).ToFloatString();
                        }

                        text.WriteLine(v);

                        text.WriteLine("vt " + ((float)vertexLine.TextureU / 255f).ToFloatString() + " " +
                        ((float)vertexLine.TextureV / 255f).ToFloatString());

                        if (bin.binType != BinType.ScenarioWithColors)
                        {
                            float nx = vertexLine.NormalX;
                            float ny = vertexLine.NormalY;
                            float nz = vertexLine.NormalZ;

                            float NORMAL_FIX = (float)Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));
                            NORMAL_FIX = (NORMAL_FIX == 0) ? 1 : NORMAL_FIX;
                            nx /= NORMAL_FIX;
                            ny /= NORMAL_FIX;
                            nz /= NORMAL_FIX;

                            float[] normal = new float[3]; // 0 = x, 1 = y, 2 = z
                            normal[0] = nx;
                            normal[1] = ny;
                            normal[2] = nz;

                            normal = RotationUtils.RotationInX(normal, SMDLine.angleX);
                            normal = RotationUtils.RotationInY(normal, SMDLine.angleY);
                            normal = RotationUtils.RotationInZ(normal, SMDLine.angleZ);

                            text.WriteLine("vn " + normal[0].ToFloatString() + " " + normal[1].ToFloatString() + " " + normal[2].ToFloatString());
                        }
                        else
                        {
                            text.WriteLine("vn 0 0 0");
                        }
                    }


                    bool invFace = false;
                    int counter = 0;
                    while (counter < bin.Nodes[t].Segments[i].vertexLines.Length)
                    {

                        string a = (indexCount - 2).ToString();
                        string b = (indexCount - 1).ToString();
                        string c = (indexCount).ToString();


                        if ((counter - 2) > -1
                            &&
                           (bin.Nodes[t].Segments[i].vertexLines[counter].IndexComplement == 0)
                           )
                        {

                            if (invFace)
                            {
                                if (bin.binType != BinType.ScenarioWithColors)
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
                                if (bin.binType != BinType.ScenarioWithColors)
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


                        counter++;
                        indexCount++;
                    }



                }

            }
        }

        public static void CreateSMDmodelReference(SMDLine[] SMDLines, string baseDiretory, string smdFileName)
        {
            var inv = System.Globalization.CultureInfo.InvariantCulture;

            TextWriter text = new FileInfo(baseDiretory + smdFileName + ".reference.smd").CreateText();
            text.WriteLine("version 1");
            text.WriteLine("nodes");

            for (int i = 0; i < SMDLines.Length; i++)
            {
                text.WriteLine(i + " \"SMD_" + i.ToString("D3") + "_BIN_" + SMDLines[i].BinID.ToString("D3") + "\" -1");
            }
            text.WriteLine(SMDLines.Length + " \"Center\" -1");
            text.WriteLine("end");

            text.WriteLine("skeleton");
            text.WriteLine("time 0");

            for (int i = 0; i < SMDLines.Length; i++)
            {
                text.WriteLine(i +
                   " " + (SMDLines[i].positionX / CONSTs.GLOBAL_SCALE).ToString("F9", inv) +
                   " " + (SMDLines[i].positionZ / CONSTs.GLOBAL_SCALE * -1).ToString("F9", inv) +
                   " " + (SMDLines[i].positionY / CONSTs.GLOBAL_SCALE).ToString("F9", inv) +
                   " " + SMDLines[i].angleX.ToString("F9", inv) +
                   " " + SMDLines[i].angleZ.ToString("F9", inv) +
                   " " + SMDLines[i].angleY.ToString("F9", inv)
                   );
            }
            text.WriteLine(SMDLines.Length + " 0.0000000 0.000000 0.000000 0.0000000 0.000000 0.000000"); //center

            text.WriteLine("end");

            text.WriteLine("triangles");

            for (int i = 0; i < SMDLines.Length; i++)
            {
                float[] pos1 = new float[3]; // 0 = x, 1 = y, 2 = z
                //XYZ
                pos1[0] = SMDLines[i].positionX / CONSTs.GLOBAL_SCALE;
                pos1[1] = SMDLines[i].positionY / CONSTs.GLOBAL_SCALE;
                pos1[2] = SMDLines[i].positionZ / CONSTs.GLOBAL_SCALE;

                //--------
                float[] pos2 = new float[3]; // 0 = x, 1 = y, 2 = z

                pos2[0] = 0;
                pos2[1] = -1000;
                pos2[2] = 1000;

                //XYZ

                pos2 = RotationUtils.RotationInX(pos2, SMDLines[i].angleX);
                pos2 = RotationUtils.RotationInY(pos2, SMDLines[i].angleY);
                pos2 = RotationUtils.RotationInZ(pos2, SMDLines[i].angleZ);

                pos2[0] = ((pos2[0] * SMDLines[i].scaleX) + SMDLines[i].positionX) / CONSTs.GLOBAL_SCALE;
                pos2[1] = ((pos2[1] * SMDLines[i].scaleY) + SMDLines[i].positionY) / CONSTs.GLOBAL_SCALE;
                pos2[2] = ((pos2[2] * SMDLines[i].scaleZ) + SMDLines[i].positionZ) / CONSTs.GLOBAL_SCALE;

                //----
                float[] pos3 = new float[3]; // 0 = x, 1 = y, 2 = z

                pos3[0] = 0;
                pos3[1] = -1000;
                pos3[2] = -1000;

                //XYZ

                pos3 = RotationUtils.RotationInX(pos3, SMDLines[i].angleX);
                pos3 = RotationUtils.RotationInY(pos3, SMDLines[i].angleY);
                pos3 = RotationUtils.RotationInZ(pos3, SMDLines[i].angleZ);

                pos3[0] = ((pos3[0] * SMDLines[i].scaleX) + SMDLines[i].positionX) / CONSTs.GLOBAL_SCALE;
                pos3[1] = ((pos3[1] * SMDLines[i].scaleY) + SMDLines[i].positionY) / CONSTs.GLOBAL_SCALE;
                pos3[2] = ((pos3[2] * SMDLines[i].scaleZ) + SMDLines[i].positionZ) / CONSTs.GLOBAL_SCALE;

                //----------

                text.WriteLine("NOMATERIAL");
                text.WriteLine(i.ToString() + " " + (pos1[0]).ToString("F9", inv) + " " + (pos1[2] * -1).ToString("F9", inv) + " " + (pos1[1]).ToString("F9", inv) + " 0 0 0 0 0 0");
                text.WriteLine(i.ToString() + " " + (pos2[0]).ToString("F9", inv) + " " + (pos2[2] * -1).ToString("F9", inv) + " " + (pos2[1]).ToString("F9", inv) + " 0 0 0 0 0 0");
                text.WriteLine(i.ToString() + " " + (pos3[0]).ToString("F9", inv) + " " + (pos3[2] * -1).ToString("F9", inv) + " " + (pos3[1]).ToString("F9", inv) + " 0 0 0 0 0 0");
            }

            // center
            text.WriteLine("NOMATERIAL");
            text.WriteLine(SMDLines.Length + " 0 0 0 0 0 0 0 0 0");
            text.WriteLine(SMDLines.Length + " 0 10 -10 0 0 0 0 0 0");
            text.WriteLine(SMDLines.Length + " 0 -10 -10 0 0 0 0 0 0");

            text.WriteLine("end");
            text.WriteLine("// RE4_PS2_SCENARIO_SMD_TOOL");
            text.WriteLine("// By JADERLINK");
            text.WriteLine($"// Version {Program.VERSION}");

            text.Close();
        }


        public static void CreateIdxScenario(SMDLine[] smdLines, Dictionary<int, BinRenderBox> boxes, string binFolder, string baseDirectory, string baseFileName, string SmdFileName, string TplFile)
        {
            //
            TextWriter text = new FileInfo(baseDirectory + baseFileName + ".idxps2scenario").CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            text.WriteLine("SmdAmount:" + smdLines.Length);
            text.WriteLine("SmdFileName:" + SmdFileName);
            text.WriteLine("TplFile:" + TplFile);
            text.WriteLine("BinFolder:" + binFolder);
            text.WriteLine("UseIdxMaterial:false");

            text.WriteLine("");
            for (int i = 0; i < smdLines.Length; i++)
            {
                text.WriteLine("");
                CreateIdxScenario_parts(i, ref text, smdLines[i]);

                CreateIdxScenario_DrawDistance(i, ref text, smdLines[i], boxes[smdLines[i].BinID]);
            }

            text.Close();


        }

        private static void CreateIdxScenario_parts(int id, ref TextWriter text, SMDLine smdLine)
        {
            var inv = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.InvariantCulture.Clone();
            inv.NumberFormat.NumberDecimalDigits = 9;

            string positionX = (smdLine.positionX / CONSTs.GLOBAL_SCALE).ToString("f9", inv);
            string positionY = (smdLine.positionY / CONSTs.GLOBAL_SCALE).ToString("f9", inv);
            string positionZ = (smdLine.positionZ / CONSTs.GLOBAL_SCALE).ToString("f9", inv);
            text.WriteLine(id.ToString("D3") + "_positionX:" + positionX);
            text.WriteLine(id.ToString("D3") + "_positionY:" + positionY);
            text.WriteLine(id.ToString("D3") + "_positionZ:" + positionZ);

            string angleX = (smdLine.angleX).ToString("f9", inv);
            string angleY = (smdLine.angleY).ToString("f9", inv);
            string angleZ = (smdLine.angleZ).ToString("f9", inv);
            text.WriteLine(id.ToString("D3") + "_angleX:" + angleX);
            text.WriteLine(id.ToString("D3") + "_angleY:" + angleY);
            text.WriteLine(id.ToString("D3") + "_angleZ:" + angleZ);

            string scaleX = (smdLine.scaleX).ToString("f9", inv);
            string scaleY = (smdLine.scaleY).ToString("f9", inv);
            string scaleZ = (smdLine.scaleZ).ToString("f9", inv);
            text.WriteLine(id.ToString("D3") + "_scaleX:" + scaleX);
            text.WriteLine(id.ToString("D3") + "_scaleY:" + scaleY);
            text.WriteLine(id.ToString("D3") + "_scaleZ:" + scaleZ);
        }

        private static void CreateIdxScenario_DrawDistance(int id, ref TextWriter text, SMDLine smdLine, BinRenderBox box)
        {
            float[] pos1 = new float[3];// 0 = x, 1 = y, 2 = z
            pos1[0] = box.DrawDistanceNegativeX;
            pos1[1] = box.DrawDistanceNegativeY;
            pos1[2] = box.DrawDistanceNegativeZ;

            pos1 = RotationUtils.RotationInX(pos1, smdLine.angleX);
            pos1 = RotationUtils.RotationInY(pos1, smdLine.angleY);
            pos1 = RotationUtils.RotationInZ(pos1, smdLine.angleZ);

            pos1[0] = ((pos1[0] * smdLine.scaleX) + (smdLine.positionX)) / 100f;
            pos1[1] = ((pos1[1] * smdLine.scaleY) + (smdLine.positionY)) / 100f;
            pos1[2] = ((pos1[2] * smdLine.scaleZ) + (smdLine.positionZ)) / 100f;

            float[] pos2 = new float[3];// 0 = x, 1 = y, 2 = z
            pos2[0] = box.DrawDistancePositiveX;
            pos2[1] = box.DrawDistancePositiveY;
            pos2[2] = box.DrawDistancePositiveZ;

            pos2 = RotationUtils.RotationInX(pos2, smdLine.angleX);
            pos2 = RotationUtils.RotationInY(pos2, smdLine.angleY);
            pos2 = RotationUtils.RotationInZ(pos2, smdLine.angleZ);

            pos2[0] = ((pos2[0] * smdLine.scaleX) + (smdLine.positionX)) / 100f;
            pos2[1] = ((pos2[1] * smdLine.scaleY) + (smdLine.positionY)) / 100f;
            pos2[2] = ((pos2[2] * smdLine.scaleZ) + (smdLine.positionZ)) / 100f;


            string DrawDistanceNegativeX = (pos1[0]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeY = (pos1[1]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeZ = (pos1[2]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);

            string DrawDistancePositiveX = (pos2[0]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveY = (pos2[1]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveZ = (pos2[2]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);


            text.WriteLine(id.ToString("D3") + "_DrawDistanceNegativeX:" + DrawDistanceNegativeX);
            text.WriteLine(id.ToString("D3") + "_DrawDistanceNegativeY:" + DrawDistanceNegativeY);
            text.WriteLine(id.ToString("D3") + "_DrawDistanceNegativeZ:" + DrawDistanceNegativeZ);

            text.WriteLine(id.ToString("D3") + "_DrawDistancePositiveX:" + DrawDistancePositiveX);
            text.WriteLine(id.ToString("D3") + "_DrawDistancePositiveY:" + DrawDistancePositiveY);
            text.WriteLine(id.ToString("D3") + "_DrawDistancePositiveZ:" + DrawDistancePositiveZ);


        }


        public static void CreateIdxps2Smd(SMDLine[] smdLines, Dictionary<int, BinRenderBox> boxes, string binFolder, string baseDirectory, string baseFileName, string SmdFileName, string TplFile, int binAmount)
        {
            TextWriter text = new FileInfo(baseDirectory + baseFileName + ".idxps2smd").CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");

            text.WriteLine("SmdAmount:" + smdLines.Length);
            text.WriteLine("SmdFileName:" + SmdFileName);
            text.WriteLine("TplFile:" + TplFile);
            text.WriteLine("BinFolder:" + binFolder);
            text.WriteLine("BinAmount:" + binAmount);

            text.WriteLine("");
            for (int i = 0; i < smdLines.Length; i++)
            {
                text.WriteLine("");
                CreateIdxScenario_parts(i, ref text, smdLines[i]);
                CreateIdxuhdSmd_parts(i, ref text, smdLines[i]);

                CreateIdxScenario_DrawDistance(i, ref text, smdLines[i], boxes[smdLines[i].BinID]);
            }

            text.Close();

        }

        private static void CreateIdxuhdSmd_parts(int id, ref TextWriter text, SMDLine smdLine)
        {
            var inv = (System.Globalization.CultureInfo)System.Globalization.CultureInfo.InvariantCulture.Clone();
            inv.NumberFormat.NumberDecimalDigits = 9;

            text.WriteLine(id.ToString("D3") + "_positionW:" + smdLine.positionW.ToString("f9", inv));
            text.WriteLine(id.ToString("D3") + "_angleW:" + smdLine.angleW.ToString("f9", inv));
            text.WriteLine(id.ToString("D3") + "_scaleW:" + smdLine.scaleW.ToString("f9", inv));
            text.WriteLine(id.ToString("D3") + "_BinID:" + smdLine.BinID);
            text.WriteLine(id.ToString("D3") + "_FixedFF:" + smdLine.FixedFF.ToString("X2"));
            text.WriteLine(id.ToString("D3") + "_SmxID:" + smdLine.SmxID);
            text.WriteLine(id.ToString("D3") + "_unused1:" + smdLine.unused1.ToString("X8"));
            text.WriteLine(id.ToString("D3") + "_objectStatus:" + smdLine.objectStatus.ToString("X8"));
            text.WriteLine(id.ToString("D3") + "_unused2:" + smdLine.unused2.ToString("X8"));        
        }

        public static void CreateDrawDistanceObj(SMDLine[] smdLine, Dictionary<int, BinRenderBox> boxes, string baseDiretory, string baseFileName)
        {
            //
            TextWriter text = new FileInfo(baseDiretory + baseFileName + ".DrawDistance.obj").CreateText();
            text.WriteLine(Program.headerText());
            text.WriteLine("");
            int index = 0;

            for (int i = 0; i < smdLine.Length; i++)
            {
                if (boxes.ContainsKey(smdLine[i].BinID))
                {
                    CreateDrawDistancePart(i, ref text, smdLine[i], boxes[smdLine[i].BinID], ref index);
                }
             
            }

            text.Close();
        }

        private static void CreateDrawDistancePart(int id, ref TextWriter text, SMDLine smdLine, BinRenderBox box, ref int index)
        {
            text.WriteLine("g " + "DRAWDISTANCE#PMD_" + id.ToString("D3") + "#");

            float[] pos1 = new float[3];// 0 = x, 1 = y, 2 = z
            pos1[0] = box.DrawDistanceNegativeX;
            pos1[1] = box.DrawDistanceNegativeY;
            pos1[2] = box.DrawDistanceNegativeZ;

            pos1 = RotationUtils.RotationInX(pos1, smdLine.angleX);
            pos1 = RotationUtils.RotationInY(pos1, smdLine.angleY);
            pos1 = RotationUtils.RotationInZ(pos1, smdLine.angleZ);

            pos1[0] = ((pos1[0] * smdLine.scaleX) + (smdLine.positionX)) / 100f;
            pos1[1] = ((pos1[1] * smdLine.scaleY) + (smdLine.positionY)) / 100f;
            pos1[2] = ((pos1[2] * smdLine.scaleZ) + (smdLine.positionZ)) / 100f;


            float[] pos2 = new float[3];// 0 = x, 1 = y, 2 = z
            pos2[0] = box.DrawDistanceNegativeX + box.DrawDistancePositiveX;
            pos2[1] = box.DrawDistanceNegativeY + box.DrawDistancePositiveY;
            pos2[2] = box.DrawDistanceNegativeZ + box.DrawDistancePositiveZ;

            pos2 = RotationUtils.RotationInX(pos2, smdLine.angleX);
            pos2 = RotationUtils.RotationInY(pos2, smdLine.angleY);
            pos2 = RotationUtils.RotationInZ(pos2, smdLine.angleZ);

            pos2[0] = ((pos2[0] * smdLine.scaleX) + (smdLine.positionX)) / 100f;
            pos2[1] = ((pos2[1] * smdLine.scaleY) + (smdLine.positionY)) / 100f;
            pos2[2] = ((pos2[2] * smdLine.scaleZ) + (smdLine.positionZ)) / 100f;

            string DrawDistanceNegativeX = (pos1[0]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeY = (pos1[1]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistanceNegativeZ = (pos1[2]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);

            string DrawDistancePositiveX = (pos2[0]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveY = (pos2[1]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);
            string DrawDistancePositiveZ = (pos2[2]).ToString("f9", System.Globalization.CultureInfo.InvariantCulture);


            text.WriteLine("v " + DrawDistanceNegativeX + " " + DrawDistanceNegativeY + " " + DrawDistanceNegativeZ);
            text.WriteLine("v " + DrawDistancePositiveX + " " + DrawDistancePositiveY + " " + DrawDistancePositiveZ);

            int i1 = index + 1;
            int i2 = index + 2;

            text.WriteLine($"l {i1} {i2}");

            index += 2;
        }

    }


}