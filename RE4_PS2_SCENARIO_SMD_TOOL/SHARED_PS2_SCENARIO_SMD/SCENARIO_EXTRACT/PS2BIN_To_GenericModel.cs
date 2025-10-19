using SHARED_PS2_BIN.ALL;
using SHARED_PS2_BIN.EXTRACT;
using SHARED_TOOLS.ALL;
using System;
using System.Collections.Generic;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT
{
    public static class PS2BIN_To_GenericModel
    {
        public static PS2GenericModelBIN Converter(PS2BIN bin)
        {
            PS2GenericModelBIN gmb = new PS2GenericModelBIN();
            gmb.binType = bin.binType;

            List<GenericMaterialBIN> materials = new List<GenericMaterialBIN>();

            List<(float vx, float vy, float vz)> vertexs = new List<(float vx, float vy, float vz)>();
            List<(float nx, float ny, float nz)> normals = new List<(float nx, float ny, float nz)>();
            List<(float a, float r, float g, float b)> colors = new List<(float a, float r, float g, float b)>();
            List<(float tu, float tv)> texUVs = new List<(float tu, float tv)>();

            int indexCount = 0;

            for (int t = 0; t < bin.Nodes.Length; t++)
            {
                GenericMaterialBIN gmaterial = new GenericMaterialBIN();
                gmaterial.material = new MaterialPart(bin.materials[t].materialLine);

                List<(GenericFaceIndex i1, GenericFaceIndex i2, GenericFaceIndex i3)> Faces = new List<(GenericFaceIndex i1, GenericFaceIndex i2, GenericFaceIndex i3)>();

                for (int i = 0; i < bin.Nodes[t].Segments.Length; i++)
                {
                    for (int l = 0; l < bin.Nodes[t].Segments[i].vertexLines.Length; l++)
                    {
                        VertexLine vertexLine = bin.Nodes[t].Segments[i].vertexLines[l];

                        vertexs.Add((
                        (float)vertexLine.VerticeX * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_POSITION_SCALE, //X
                        (float)vertexLine.VerticeY * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_POSITION_SCALE, //Y
                        (float)vertexLine.VerticeZ * bin.Nodes[t].Segments[i].ConversionFactorValue / CONSTs.GLOBAL_POSITION_SCALE  //Z
                        ));

                        if (bin.binType == BinType.ScenarioWithColors)
                        {
                            // nesse caso o valor no campo normal, na verdade são cores
                            colors.Add((
                            (vertexLine.UnknownB / 128f),// A
                            (vertexLine.NormalX / 128f), // R
                            (vertexLine.NormalY / 128f), // G
                            (vertexLine.NormalZ / 128f)  // B
                            ));
                        }

                        texUVs.Add((
                            ((float)vertexLine.TextureU / 255f),
                            ((float)vertexLine.TextureV / 255f)
                            ));

                        if (bin.binType == BinType.Default)
                        {
                            float nx = vertexLine.NormalX;
                            float ny = vertexLine.NormalY;
                            float nz = vertexLine.NormalZ;

                            float NORMAL_FIX = (float)Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));
                            NORMAL_FIX = (NORMAL_FIX == 0) ? 1 : NORMAL_FIX;
                            nx /= NORMAL_FIX;
                            ny /= NORMAL_FIX;
                            nz /= NORMAL_FIX;

                            normals.Add((
                            nx,
                            ny,
                            nz
                            ));
                        }

                    }

                    bool invFace = false;
                    int counter = 0;
                    while (counter < bin.Nodes[t].Segments[i].vertexLines.Length)
                    {

                        if ((counter - 2) > -1
                            &&
                           (bin.Nodes[t].Segments[i].vertexLines[counter].IndexComplement == 0)
                           )
                        {
                            int a = (indexCount - 2);
                            int b = (indexCount - 1);
                            int c = (indexCount);

                            GenericFaceIndex i1 = new GenericFaceIndex();
                            i1.indexVertex = a;
                            i1.indexNormal = a;
                            i1.indexUV = a;

                            GenericFaceIndex i2 = new GenericFaceIndex();
                            i2.indexVertex = b;
                            i2.indexNormal = b;
                            i2.indexUV = b;

                            GenericFaceIndex i3 = new GenericFaceIndex();
                            i3.indexVertex = c;
                            i3.indexNormal = c;
                            i3.indexUV = c;

                            if (bin.binType == BinType.ScenarioWithColors)
                            {
                                i1.indexNormal = -1;
                                i2.indexNormal = -1;
                                i3.indexNormal = -1;
                            }

                            if (invFace)
                            {
                                Faces.Add((i3, i2, i1));
          
                                invFace = false;
                            }
                            else
                            {
                                Faces.Add((i1, i2, i3));

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

                gmaterial.face_index_array = Faces.ToArray();
                materials.Add(gmaterial);
            }

            gmb.Materials = materials.ToArray();

            gmb.Vertex_Position_Array = vertexs.ToArray();
            gmb.Vertex_Normal_Array = normals.ToArray();
            gmb.Vertex_Color_Array = colors.ToArray();
            gmb.Vertex_UV_Array = texUVs.ToArray();

            return gmb;
        }

    }
}
