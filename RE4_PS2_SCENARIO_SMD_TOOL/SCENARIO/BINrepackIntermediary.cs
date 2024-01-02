using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_PS2_BIN_TOOL.REPACK;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO
{
    public static class BINrepackIntermediary
    {
        public static IntermediaryStructure MakeIntermediaryStructure(StartStructure startStructure, SMDLineIdx smdLine,
            out (float X, float Y, float Z) position, out float ConversionFactorValue)
        {
            // valor que representa a maior distancia do modelo, tanto para X, Y ou Z
            float FarthestVertex = 0;

            // passo 1: pegar valor para centralizar o modelo 3d

            Dictionary<Limits, float> limits = new Dictionary<Limits, float>();

            if (startStructure.FacesByMaterial.Count >= 1)
            {
                var pos = startStructure.FacesByMaterial.First().Value.Faces[0][0].Position;

                limits.Add(Limits.MaxX, pos.X);
                limits.Add(Limits.MinX, pos.X);

                limits.Add(Limits.MaxY, pos.Y);
                limits.Add(Limits.MinY, pos.Y);

                limits.Add(Limits.MaxZ, pos.Z);
                limits.Add(Limits.MinZ, pos.Z);
            }
            else
            {
                limits.Add(Limits.MaxX, 0);
                limits.Add(Limits.MinX, 0);

                limits.Add(Limits.MaxY, 0);
                limits.Add(Limits.MinY, 0);

                limits.Add(Limits.MaxZ, 0);
                limits.Add(Limits.MinZ, 0);
            }

            foreach (var faceGroup in startStructure.FacesByMaterial)
            {
                var Faces = faceGroup.Value.Faces;

                for (int i = 0; i < Faces.Count; i++)
                {
                    for (int t = 0; t < Faces[i].Count; t++)
                    {
                        var item = Faces[i][t].Position;

                        if (item.X < limits[Limits.MinX])
                        {
                            limits[Limits.MinX] = item.X;
                        }

                        if (item.X > limits[Limits.MaxX])
                        {
                            limits[Limits.MaxX] = item.X;
                        }

                        if (item.Y < limits[Limits.MinY])
                        {
                            limits[Limits.MinY] = item.Y;
                        }

                        if (item.Y > limits[Limits.MaxY])
                        {
                            limits[Limits.MaxY] = item.Y;
                        }

                        if (item.Z < limits[Limits.MinZ])
                        {
                            limits[Limits.MinZ] = item.Z;
                        }

                        if (item.Z > limits[Limits.MaxZ])
                        {
                            limits[Limits.MaxZ] = item.Z;
                        }

                    }
                }
            }

            float dinX = limits[Limits.MinX] - limits[Limits.MaxX];
            float dinY = limits[Limits.MinY] - limits[Limits.MaxY];
            float dinZ = limits[Limits.MinZ] - limits[Limits.MaxZ];

            float halfX = dinX / 2;
            float halfY = dinY / 2;
            float halfZ = dinZ / 2;

            float distanceX = dinX + halfX;
            float distanceY = dinY + halfY;
            float distanceZ = dinZ + halfZ;

            position = (distanceX, distanceY, distanceZ);

            // segunda e terceira etapas
            IntermediaryStructure intermediary = new IntermediaryStructure();

            foreach (var item in startStructure.FacesByMaterial)
            {
                IntermediaryGroup group = new IntermediaryGroup();
                group.MaterialName = item.Key;

                var Faces = item.Value.Faces;

                for (int i = 0; i < Faces.Count; i++)
                {
                    IntermediaryFace face = new IntermediaryFace();

                    for (int t = 0; t < Faces[i].Count; t++)
                    {
                        IntermediaryVertex vertex = new IntermediaryVertex();

                        float[] pos1 = new float[3];// 0 = x, 1 = y, 2 = z
                        pos1[0] = Faces[i][t].Position.X * 100f;
                        pos1[1] = Faces[i][t].Position.Y * 100f;
                        pos1[2] = Faces[i][t].Position.Z * 100f;

                        pos1[0] = ((pos1[0]) - (distanceX * 100f)) / smdLine.scaleX;
                        pos1[1] = ((pos1[1]) - (distanceY * 100f)) / smdLine.scaleY;
                        pos1[2] = ((pos1[2]) - (distanceZ * 100f)) / smdLine.scaleZ;

                        pos1 = RotationUtils.RotationInZ(pos1, -smdLine.angleZ);
                        pos1 = RotationUtils.RotationInY(pos1, -smdLine.angleY);
                        pos1 = RotationUtils.RotationInX(pos1, -smdLine.angleX);

                        vertex.PosX = pos1[0] / 100f;
                        vertex.PosY = pos1[1] / 100f;
                        vertex.PosZ = pos1[2] / 100f;

                        vertex.NormalX = Faces[i][t].Normal.X;
                        vertex.NormalY = Faces[i][t].Normal.Y;
                        vertex.NormalZ = Faces[i][t].Normal.Z;

                        vertex.TextureU = Faces[i][t].Texture.U;
                        vertex.TextureV = Faces[i][t].Texture.V;

                        vertex.ColorR = Faces[i][t].Color.R;
                        vertex.ColorG = Faces[i][t].Color.G;
                        vertex.ColorB = Faces[i][t].Color.B;
                        vertex.ColorA = Faces[i][t].Color.A;

                        vertex.Links = Faces[i][t].WeightMap.Links;

                        vertex.BoneID1 = Faces[i][t].WeightMap.BoneID1;
                        vertex.Weight1 = Faces[i][t].WeightMap.Weight1;
                        vertex.BoneID2 = Faces[i][t].WeightMap.BoneID2;
                        vertex.Weight2 = Faces[i][t].WeightMap.Weight2;
                        vertex.BoneID3 = Faces[i][t].WeightMap.BoneID3;
                        vertex.Weight3 = Faces[i][t].WeightMap.Weight3;

                        face.Vertexs.Add(vertex);

                        IntermediaryWeightMap weightMap = vertex.GetIntermediaryWeightMap();
                        if (!face.WeightMapOnFace.Contains(weightMap))
                        {
                            face.WeightMapOnFace.Add(weightMap);
                        }

                        //-------------
                        // --- verifica o vertice mais distante

                        float temp = vertex.PosX;
                        if (temp < 0)
                        {
                            temp *= -1;
                        }
                        if (temp > FarthestVertex)
                        {
                            FarthestVertex = temp;
                        }

                        temp = vertex.PosY;
                        if (temp < 0)
                        {
                            temp *= -1;
                        }
                        if (temp > FarthestVertex)
                        {
                            FarthestVertex = temp;
                        }

                        temp = vertex.PosZ;
                        if (temp < 0)
                        {
                            temp *= -1;
                        }
                        if (temp > FarthestVertex)
                        {
                            FarthestVertex = temp;
                        }


                    }

                    group.Faces.Add(face);

                }

                intermediary.Groups.Add(item.Key, group);
            }

            // calcula o fator de conversão
            ConversionFactorValue = FarthestVertex / short.MaxValue * CONSTs.GLOBAL_SCALE;

            return intermediary;
        }

        private enum Limits
        {
            MinX,
            MaxX,
            MinY,
            MaxY,
            MinZ,
            MaxZ
        }
    }
}
