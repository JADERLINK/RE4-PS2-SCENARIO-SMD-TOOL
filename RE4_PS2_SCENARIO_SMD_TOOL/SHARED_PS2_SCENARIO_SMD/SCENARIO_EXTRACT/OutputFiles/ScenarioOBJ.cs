using SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT;
using SHARED_PS2_BIN.ALL;
using SHARED_PS2_BIN.EXTRACT;
using SHARED_TOOLS.ALL;
using SHARED_TOOLS.SCENARIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT.OutputFiles
{
    public static class ScenarioOBJ
    {
        public static void CreateOBJ(SMDLine[] smdLines, Dictionary<int, PS2GenericModelBIN> BinDic, 
            Dictionary<(MaterialPart mat, byte TplFileID), string> materialDic, string baseDirectory, string baseFileName)
        {
            StreamWriter obj = new StreamWriter(Path.Combine(baseDirectory, baseFileName + ".obj"), false);
            obj.WriteLine(SHARED_TOOLS.Shared.HeaderText());
            obj.WriteLine("");

            obj.WriteLine("mtllib " + baseFileName + ".mtl");

            uint vertexIndexCount = 0;
            uint normalIndexCount = 0;
            uint uvIndexCount = 0;

            for (int smdID = 0; smdID < smdLines.Length; smdID++)
            {
                byte BinFileID = smdLines[smdID].BinFileID;
                byte TplFileID = smdLines[smdID].TplFileID;
                if (BinDic.ContainsKey(BinFileID))
                {
                    SMDLine smdLine = smdLines[smdID];

                    StringBuilder group = new StringBuilder();
                    group.Append("g ").Append("PS2SCENARIO")
                        .Append("#SMD_").Append(smdID.ToString("D3"))
                        .Append("#SMX_").Append(smdLine.SmxID.ToString("D3"))
                        .Append("#TYPE_").Append(smdLine.ObjectStatus.ToString("X2"))
                        .Append("#BIN_").Append(smdLine.BinFileID.ToString("D3"))
                        .Append("#");

                    if (BinDic[BinFileID].binType == BinType.ScenarioWithColors)
                    {
                        group.Append("COLOR#");
                    }
                    else
                    {
                        group.Append("NORMAL#");
                    }

                    obj.WriteLine(group.ToString());

                    ObjCreatePart(obj, BinDic[BinFileID], smdLine, materialDic, TplFileID, ref vertexIndexCount, ref normalIndexCount, ref uvIndexCount);
                }

            }

            obj.Close();
        }

        private static void ObjCreatePart(StreamWriter obj, PS2GenericModelBIN bin, SMDLine smdLine, 
            Dictionary<(MaterialPart mat, byte TplFileID), string> materialDic, byte TplFileID, 
            ref uint vertexIndexCount, ref uint normalIndexCount, ref uint uvIndexCount)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < bin.Vertex_Position_Array.Length; i++)
            {
                float[] pos = new float[3];// 0 = x, 1 = y, 2 = z
                pos[0] = bin.Vertex_Position_Array[i].vx;
                pos[1] = bin.Vertex_Position_Array[i].vy;
                pos[2] = bin.Vertex_Position_Array[i].vz;

                pos = RotationUtils.RotationInX(pos, smdLine.AngleX);
                pos = RotationUtils.RotationInY(pos, smdLine.AngleY);
                pos = RotationUtils.RotationInZ(pos, smdLine.AngleZ);

                pos[0] = ((pos[0] * smdLine.ScaleX) + (smdLine.PositionX / CONSTs.GLOBAL_POSITION_SCALE));
                pos[1] = ((pos[1] * smdLine.ScaleY) + (smdLine.PositionY / CONSTs.GLOBAL_POSITION_SCALE));
                pos[2] = ((pos[2] * smdLine.ScaleZ) + (smdLine.PositionZ / CONSTs.GLOBAL_POSITION_SCALE));

                sb.Append("v ").Append(pos[0].ToFloatString())
                   .Append(" ").Append(pos[1].ToFloatString())
                   .Append(" ").Append(pos[2].ToFloatString());

                if (bin.binType == BinType.ScenarioWithColors && bin.Vertex_Color_Array.Length > i)
                {
                    float r = bin.Vertex_Color_Array[i].r;
                    float g = bin.Vertex_Color_Array[i].g;
                    float b = bin.Vertex_Color_Array[i].b;
                    float a = bin.Vertex_Color_Array[i].a;

                    sb.Append(" ").Append(r.ToFloatString())
                      .Append(" ").Append(g.ToFloatString())
                      .Append(" ").Append(b.ToFloatString())
                      .Append(" ").Append(a.ToFloatString());
                }
                sb.AppendLine();
            }

            for (int i = 0; i < bin.Vertex_Normal_Array.Length; i++) // se não tiver, vai ser 0, e não vai fazer o for
            {
                float[] normal = new float[3];// 0 = x, 1 = y, 2 = z
                normal[0] = bin.Vertex_Normal_Array[i].nx;
                normal[1] = bin.Vertex_Normal_Array[i].ny;
                normal[2] = bin.Vertex_Normal_Array[i].nz;

                normal = RotationUtils.RotationInX(normal, smdLine.AngleX);
                normal = RotationUtils.RotationInY(normal, smdLine.AngleY);
                normal = RotationUtils.RotationInZ(normal, smdLine.AngleZ);

                sb.Append("vn ").Append(normal[0].ToFloatString())
                    .Append(" ").Append(normal[1].ToFloatString())
                    .Append(" ").Append(normal[2].ToFloatString())
                    .AppendLine();
            }

            for (int i = 0; i < bin.Vertex_UV_Array.Length; i++)
            {
                float tu = bin.Vertex_UV_Array[i].tu;
                float tv = bin.Vertex_UV_Array[i].tv;
                sb.Append("vt ").Append(tu.ToFloatString()).Append(" ").Append(tv.ToFloatString()).AppendLine();
            }


            for (int g = 0; g < bin.Materials.Length; g++)
            {
                var matKey = (bin.Materials[g].material, TplFileID);
                string MaterialName = materialDic.ContainsKey(matKey) ? materialDic[matKey] : "UNKNOWN_MATERIAL";

                sb.Append("usemtl ").Append(MaterialName).AppendLine();

                for (int i = 0; i < bin.Materials[g].face_index_array.Length; i++)
                {
                    long av = (bin.Materials[g].face_index_array[i].i1.indexVertex + vertexIndexCount + 1);
                    long bv = (bin.Materials[g].face_index_array[i].i2.indexVertex + vertexIndexCount + 1);
                    long cv = (bin.Materials[g].face_index_array[i].i3.indexVertex + vertexIndexCount + 1);

                    long an = (bin.Materials[g].face_index_array[i].i1.indexNormal + normalIndexCount + 1);
                    long bn = (bin.Materials[g].face_index_array[i].i2.indexNormal + normalIndexCount + 1);
                    long cn = (bin.Materials[g].face_index_array[i].i3.indexNormal + normalIndexCount + 1);

                    long at = (bin.Materials[g].face_index_array[i].i1.indexUV + uvIndexCount + 1);
                    long bt = (bin.Materials[g].face_index_array[i].i2.indexUV + uvIndexCount + 1);
                    long ct = (bin.Materials[g].face_index_array[i].i3.indexUV + uvIndexCount + 1);

                    if (bin.binType == BinType.Default)
                    {
                        // tem as normals
                        sb.Append("f ").Append(av).Append('/').Append(at).Append('/').Append(an)
                           .Append(" ").Append(bv).Append('/').Append(bt).Append('/').Append(bn)
                           .Append(" ").Append(cv).Append('/').Append(ct).Append('/').Append(cn)
                           .AppendLine();
                    }
                    else 
                    {
                        // não tem as normals
                        sb.Append("f ").Append(av).Append('/').Append(at)
                           .Append(" ").Append(bv).Append('/').Append(bt)
                           .Append(" ").Append(cv).Append('/').Append(ct)
                           .AppendLine();
                    }

                }
            }

            obj.Write(sb.ToString());

            vertexIndexCount += (uint)bin.Vertex_Position_Array.Length;
            normalIndexCount += (uint)bin.Vertex_Normal_Array.Length;
            uvIndexCount += (uint)bin.Vertex_UV_Array.Length;
        }

    }
}
