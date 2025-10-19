using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHARED_TOOLS.SCENARIO;
using SHARED_TOOLS.ALL;
using SHARED_PS2_BIN.REPACK.Structures;
using SHARED_PS2_BIN.ALL;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK.WithOBJ
{
    public static class BINrepackIntermediary
    {
        public static IntermediaryStructure MakeIntermediaryStructure(StartStructure startStructure, SMDLineIdx smdLine,
            out (float X, float Y, float Z) position, out float ConversionFactorValue, out BoundingBox boundingBox)
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

            float distanceX = (limits[Limits.MinX] + limits[Limits.MaxX]) / 2;
            float distanceY = (limits[Limits.MinY] + limits[Limits.MaxY]) / 2;
            float distanceZ = (limits[Limits.MinZ] + limits[Limits.MaxZ]) / 2;

            position = (distanceX, distanceY, distanceZ);

            //---- para o calculo do BoundingBox
            float? MinX = null;
            float? MaxX = null;
            float? MinY = null;
            float? MaxY = null;
            float? MinZ = null;
            float? MaxZ = null;

            // segunda e terceira etapas
            IntermediaryStructure intermediary = new IntermediaryStructure();

            // calculo da scala:
            float scaleX = smdLine.ScaleX != 0 ? smdLine.ScaleX : 1f;
            float scaleY = smdLine.ScaleY != 0 ? smdLine.ScaleY : 1f;
            float scaleZ = smdLine.ScaleZ != 0 ? smdLine.ScaleZ : 1f;

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
                        pos1[0] = Faces[i][t].Position.X * CONSTs.GLOBAL_POSITION_SCALE;
                        pos1[1] = Faces[i][t].Position.Y * CONSTs.GLOBAL_POSITION_SCALE;
                        pos1[2] = Faces[i][t].Position.Z * CONSTs.GLOBAL_POSITION_SCALE;

                        pos1[0] = ((pos1[0]) - (distanceX * CONSTs.GLOBAL_POSITION_SCALE)) / scaleX;
                        pos1[1] = ((pos1[1]) - (distanceY * CONSTs.GLOBAL_POSITION_SCALE)) / scaleY;
                        pos1[2] = ((pos1[2]) - (distanceZ * CONSTs.GLOBAL_POSITION_SCALE)) / scaleZ;

                        pos1 = RotationUtils.RotationInZ(pos1, -smdLine.AngleZ);
                        pos1 = RotationUtils.RotationInY(pos1, -smdLine.AngleY);
                        pos1 = RotationUtils.RotationInX(pos1, -smdLine.AngleX);

                        vertex.PosX = pos1[0];
                        vertex.PosY = pos1[1];
                        vertex.PosZ = pos1[2];

                        float[] normal1 = new float[3];// 0 = x, 1 = y, 2 = z
                        normal1[0] = Faces[i][t].Normal.X;
                        normal1[1] = Faces[i][t].Normal.Y;
                        normal1[2] = Faces[i][t].Normal.Z;

                        normal1 = RotationUtils.RotationInZ(normal1, -smdLine.AngleZ);
                        normal1 = RotationUtils.RotationInY(normal1, -smdLine.AngleY);
                        normal1 = RotationUtils.RotationInX(normal1, -smdLine.AngleX);

                        vertex.NormalX = normal1[0];
                        vertex.NormalY = normal1[1];
                        vertex.NormalZ = normal1[2];

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

                        //----------------------------
                        // para calculo do BoundingBox
                        if (MinX == null) { MinX = vertex.PosX; }
                        if (MinY == null) { MinY = vertex.PosY; }
                        if (MinZ == null) { MinZ = vertex.PosZ; }
                        if (MaxX == null) { MaxX = vertex.PosX; }
                        if (MaxY == null) { MaxY = vertex.PosY; }
                        if (MaxZ == null) { MaxZ = vertex.PosZ; }

                        if (vertex.PosX < MinX) { MinX = vertex.PosX; }
                        if (vertex.PosY < MinY) { MinY = vertex.PosY; }
                        if (vertex.PosZ < MinZ) { MinZ = vertex.PosZ; }
                        if (vertex.PosX > MaxX) { MaxX = vertex.PosX; }
                        if (vertex.PosY > MaxY) { MaxY = vertex.PosY; }
                        if (vertex.PosZ > MaxZ) { MaxZ = vertex.PosZ; }
                    }

                    group.Faces.Add(face);

                }

                intermediary.Groups.Add(item.Key, group);
            }

            // calcula o fator de conversão
            ConversionFactorValue = FarthestVertex / short.MaxValue;

            // calculo BoundingBox
            if (MinX == null) { MinX = 0; }
            if (MinY == null) { MinY = 0; }
            if (MinZ == null) { MinZ = 0; }
            if (MaxX == null) { MaxX = 0; }
            if (MaxY == null) { MaxY = 0; }
            if (MaxZ == null) { MaxZ = 0; }

            float CenterX = ((float)MinX + (float)MaxX) / 2;
            float CenterY = ((float)MinY + (float)MaxY) / 2;
            float CenterZ = ((float)MinZ + (float)MaxZ) / 2;

            float SemiDistanceX = Math.Abs((float)MaxX - (float)MinX) / 2;
            float SemiDistanceY = Math.Abs((float)MaxY - (float)MinY) / 2;
            float SemiDistanceZ = Math.Abs((float)MaxZ - (float)MinZ) / 2;

            boundingBox = new BoundingBox();
            boundingBox.BoundingBoxPosX = CenterX;
            boundingBox.BoundingBoxPosY = CenterY;
            boundingBox.BoundingBoxPosZ = CenterZ;
            boundingBox.BoundingBoxPosW = 1f;
            boundingBox.BoundingBoxWidth = SemiDistanceX;
            boundingBox.BoundingBoxHeight = SemiDistanceY;
            boundingBox.BoundingBoxDepth = SemiDistanceZ;

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
