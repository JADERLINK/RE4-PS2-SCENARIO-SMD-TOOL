﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.REPACK;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO
{
    public static partial class Ps2ScenarioRepack
    {
        public static void RepackOBJ(Stream objFile, 
            ref IdxPs2Scenario idxScenario, 
            out Dictionary<int, SmdBaseLine> objGroupInfos, 
            out Dictionary<int, FinalStructure> FinalBinList,
            out Dictionary<int, IdxBin> IdxBinDic,
            out Dictionary<int, SMDLineIdx> SMDLineIdxDic,
            out Dictionary<int, float> ConversionFactorValueDic,
            out Dictionary<int, BinRenderBox> OutBoxes)
        {
            string patternPS2SCENARIO = "^(PS2SCENARIO#SMD_)([0]{0,})([0-9]{1,3})(#SMX_)([0]{0,})([0-9]{1,3})(#TYPE_)([0]{0,})([0-9|A-F]{1,8})(#BIN_)([0]{0,})([0-9]{1,3})(#)(COLOR|NORMAL)(#).*$";
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(patternPS2SCENARIO, System.Text.RegularExpressions.RegexOptions.CultureInvariant);

            bool LoadColorsFromObjFile = true;

            // load .obj file
            var objLoaderFactory = new ObjLoader.Loader.Loaders.ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            var streamReader = new StreamReader(objFile, Encoding.ASCII);
            ObjLoader.Loader.Loaders.LoadResult arqObj = objLoader.Load(streamReader);
            streamReader.Close();

            //lista de materiais usados no modelo
            HashSet<string> ModelMaterials = new HashSet<string>();
            HashSet<string> ModelMaterialsToUpper = new HashSet<string>();


            Vector4 color = new Vector4(1, 1, 1, 1);
            StartWeightMap weightMap = new StartWeightMap(1, 0, 1, 0, 0, 0, 0);


            //conjunto de struturas
            //id do SMD/ conteudo para o SMD/BIN
            Dictionary<int, StartStructure> ObjList = new Dictionary<int, StartStructure>();
            //id do SMD, outras informações 
            objGroupInfos = new Dictionary<int, SmdBaseLine>();
            int maxSmd = 0;
            int maxBin = 0;

            for (int iG = 0; iG < arqObj.Groups.Count; iG++)
            {
                string GroupName = arqObj.Groups[iG].GroupName.ToUpperInvariant().Trim();

                if (GroupName.StartsWith("PS2SCENARIO"))
                {
                    string materialNameInvariant = arqObj.Groups[iG].MaterialName.ToUpperInvariant().Trim();
                    string materialName = arqObj.Groups[iG].MaterialName.Trim();

                    //FIX NAME
                    GroupName = GroupName.Replace("_", "#")
                        .Replace("SMD#", "SMD_")
                        .Replace("SMX#", "SMX_")
                        .Replace("TYPE#", "TYPE_")
                        .Replace("BIN#", "BIN_")
                        ;

                    //REGEX
                    if (regex.IsMatch(GroupName))
                    {
                        Console.WriteLine("Loading in Obj: " + GroupName + " | " + materialNameInvariant);
                    }
                    else
                    {
                        Console.WriteLine("Loading in Obj: " + GroupName + " | " + materialNameInvariant + "  The group name is wrong;");
                    }


                    SmdBaseLine info = getGroupInfo(GroupName);

                    if (!objGroupInfos.ContainsKey(info.SmdId))
                    {
                        objGroupInfos.Add(info.SmdId, info);
                    }

                    if (info.SmdId >= maxSmd)
                    {
                        maxSmd = info.SmdId + 1;
                    }

                    if (info.BinId >= maxBin)
                    {
                        maxBin = info.BinId + 1;
                    }

                    List<List<StartVertex>> facesList = new List<List<StartVertex>>();

                    for (int iF = 0; iF < arqObj.Groups[iG].Faces.Count; iF++)
                    {
                        List<StartVertex> verticeListInObjFace = new List<StartVertex>();

                        for (int iI = 0; iI < arqObj.Groups[iG].Faces[iF].Count; iI++)
                        {
                            StartVertex vertice = new StartVertex();

                            if (arqObj.Groups[iG].Faces[iF][iI].VertexIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1 >= arqObj.Vertices.Count)
                            {
                                throw new ArgumentException("Vertex Position Index is invalid! Value: " + arqObj.Groups[iG].Faces[iF][iI].VertexIndex);
                            }

                            Vector3 position = new Vector3(
                                arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].X,
                                arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].Y,
                                arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].Z
                                );

                            vertice.Position = position;


                            if (arqObj.Groups[iG].Faces[iF][iI].TextureIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].TextureIndex - 1 >= arqObj.Textures.Count)
                            {
                                vertice.Texture = new Vector2(0, 0);
                            }
                            else
                            {
                                Vector2 texture = new Vector2(
                                arqObj.Textures[arqObj.Groups[iG].Faces[iF][iI].TextureIndex - 1].U,
                                arqObj.Textures[arqObj.Groups[iG].Faces[iF][iI].TextureIndex - 1].V
                                );

                                vertice.Texture = texture;
                            }


                            if (arqObj.Groups[iG].Faces[iF][iI].NormalIndex <= 0 || arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1 >= arqObj.Normals.Count)
                            {
                                vertice.Normal = new Vector3(0, 0, 0);
                            }
                            else
                            {
                                float nx = arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].X;
                                float ny = arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].Y;
                                float nz = arqObj.Normals[arqObj.Groups[iG].Faces[iF][iI].NormalIndex - 1].Z;
                                float NORMAL_FIX = (float)Math.Sqrt((nx * nx) + (ny * ny) + (nz * nz));
                                NORMAL_FIX = (NORMAL_FIX == 0) ? 1 : NORMAL_FIX;
                                nx /= NORMAL_FIX;
                                ny /= NORMAL_FIX;
                                nz /= NORMAL_FIX;

                                vertice.Normal = new Vector3(nx, ny, nz);
                            }

                            vertice.Color = color;
                            vertice.WeightMap = weightMap;

                            if (LoadColorsFromObjFile)
                            {
                                Vector4 vColor = new Vector4(
                               arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].R,
                               arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].G,
                               arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].B,
                               arqObj.Vertices[arqObj.Groups[iG].Faces[iF][iI].VertexIndex - 1].A
                               );
                                vertice.Color = vColor;
                            }

                            verticeListInObjFace.Add(vertice);

                        }

                        if (verticeListInObjFace.Count >= 3)
                        {
                            for (int i = 2; i < verticeListInObjFace.Count; i++)
                            {
                                List<StartVertex> face = new List<StartVertex>();
                                face.Add(verticeListInObjFace[0]);
                                face.Add(verticeListInObjFace[i - 1]);
                                face.Add(verticeListInObjFace[i]);
                                facesList.Add(face);
                            }
                        }

                    }


                    if (ObjList.ContainsKey(info.SmdId))
                    {
                        if (ObjList[info.SmdId].FacesByMaterial.ContainsKey(materialNameInvariant))
                        {
                            ObjList[info.SmdId].FacesByMaterial[materialNameInvariant].Faces.AddRange(facesList);
                        }
                        else
                        {
                            ModelMaterials.Add(materialName);
                            ModelMaterialsToUpper.Add(materialNameInvariant);
                            ObjList[info.SmdId].FacesByMaterial.Add(materialNameInvariant, new StartFacesGroup(facesList));
                        }
                    }
                    else
                    {
                        StartStructure startStructure = new StartStructure();
                        ModelMaterials.Add(materialName);
                        ModelMaterialsToUpper.Add(materialNameInvariant);
                        startStructure.FacesByMaterial.Add(materialNameInvariant, new StartFacesGroup(facesList));
                        ObjList.Add(info.SmdId, startStructure);
                    }

                }
                else
                {
                    Console.WriteLine("Loading in Obj: " + GroupName + "   Warning: Group not used;");
                }

            }


            if (idxScenario.SmdAmount < maxSmd)
            {
                idxScenario.SmdAmount = maxSmd;
            }

            if (idxScenario.BinAmount < maxBin)
            {
                idxScenario.BinAmount = maxBin;
            }


            for (int i = 0; i < idxScenario.SmdAmount; i++)
            {
                if (!objGroupInfos.ContainsKey(i))
                {
                    SmdBaseLine smdBaseLine = new SmdBaseLine();
                    smdBaseLine.SmdId = i;
                    smdBaseLine.SmxId = 0xFE;
                    smdBaseLine.Type = 0;
                    smdBaseLine.BinId = 0;
                    smdBaseLine.useType = UseType.Color;
                    objGroupInfos.Add(i, smdBaseLine);
                }
            }

            //----
            FinalBinList = new Dictionary<int, FinalStructure>();
            IdxBinDic = new Dictionary<int, IdxBin>();
            SMDLineIdxDic = new Dictionary<int, SMDLineIdx>();
            ConversionFactorValueDic = new Dictionary<int, float>();
            OutBoxes = new Dictionary<int, BinRenderBox>();

            foreach (var item in ObjList)
            {
                int BinID = objGroupInfos[item.Key].BinId;

                if (!FinalBinList.ContainsKey(BinID))
                {
                    // faz a compressão das vertives
                    Console.WriteLine("BIN ID: " + BinID.ToString("D3"));
                    item.Value.CompressAllFaces();
                    //-----

                    SMDLineIdx smdLineIdx = new SMDLineIdx();
                    smdLineIdx.scaleX = 1f;
                    smdLineIdx.scaleY = 1f;
                    smdLineIdx.scaleZ = 1f;

                    if (idxScenario.SmdLines.Length > item.Key)
                    {
                        smdLineIdx = idxScenario.SmdLines[item.Key];
                    }

                    (float X, float Y, float Z) position = (0,0,0);
                    float ConversionFactorValue = 0;

                    var intermediary = BINrepackIntermediary.MakeIntermediaryStructure(item.Value, smdLineIdx, out position, out ConversionFactorValue);
                    FinalStructure final = BINrepackFinal.MakeFinalStructure(intermediary, ConversionFactorValue, CONSTs.GLOBAL_SCALE);
                    FinalBinList.Add(BinID, final);
                    ConversionFactorValueDic.Add(BinID, ConversionFactorValue);

                    smdLineIdx.positionX = position.X;
                    smdLineIdx.positionY = position.Y;
                    smdLineIdx.positionZ = position.Z;
                    SMDLineIdxDic.Add(item.Key, smdLineIdx);

                    //conteudo complementar
                    IdxBin idxBin = new IdxBin();
                    idxBin.BonePairLines = new BonePairLine[0];
                    idxBin.BoneLines = new BoneLine[1];
                    BoneLine bone = new BoneLine();
                    bone.BoneId = 0;
                    bone.BoneParent = 0xFF; // -1
                    idxBin.BoneLines[0] = bone;
                    idxBin.CompressVertices = true;
                    idxBin.DrawDistanceNegativePadding = 1f;
                    idxBin.unknown1 = new byte[2];
                    idxBin.unknown2 = 0;

                    idxBin.unknown4_B = new byte[4] { 0x01, 0x08, 0x01, 0x20 }; // version
                    idxBin.unknown4_unk009 = new byte[4] { 0x00, 0x00, 0x00, 0x00 }; // tex tag

                    if (objGroupInfos[item.Key].useType == UseType.Color)
                    {
                        idxBin.IsScenarioBin = true;
                        idxBin.unknown4_unk009[3] = 0xE0;
                    }
                    else
                    {
                        idxBin.IsScenarioBin = false;
                        idxBin.unknown4_unk009[3] = 0xB0;
                    }

                    //-----------

                    if (idxScenario.BinRenderBoxes.Length > item.Key)
                    {
                        SMDLineIdx smdLine = smdLineIdx;
                        BinRenderBox box = idxScenario.BinRenderBoxes[item.Key];

                        float[] pos1 = new float[3];// 0 = x, 1 = y, 2 = z
                        pos1[0] = box.DrawDistanceNegativeX * 100f;
                        pos1[1] = box.DrawDistanceNegativeY * 100f;
                        pos1[2] = box.DrawDistanceNegativeZ * 100f;

                        pos1[0] = ((pos1[0]) - (smdLine.positionX * 100f)) / smdLine.scaleX;
                        pos1[1] = ((pos1[1]) - (smdLine.positionY * 100f)) / smdLine.scaleY;
                        pos1[2] = ((pos1[2]) - (smdLine.positionZ * 100f)) / smdLine.scaleZ;

                        pos1 = RotationUtils.RotationInZ(pos1, -smdLine.angleZ);
                        pos1 = RotationUtils.RotationInY(pos1, -smdLine.angleY);
                        pos1 = RotationUtils.RotationInX(pos1, -smdLine.angleX);

                        idxBin.DrawDistanceNegativeX = pos1[0];
                        idxBin.DrawDistanceNegativeY = pos1[1];
                        idxBin.DrawDistanceNegativeZ = pos1[2];

                        float[] pos2 = new float[3];// 0 = x, 1 = y, 2 = z
                        pos2[0] = box.DrawDistancePositiveX * 100f;
                        pos2[1] = box.DrawDistancePositiveY * 100f;
                        pos2[2] = box.DrawDistancePositiveZ * 100f;

                        pos2[0] = ((pos2[0]) - (smdLine.positionX * 100f)) / smdLine.scaleX;
                        pos2[1] = ((pos2[1]) - (smdLine.positionY * 100f)) / smdLine.scaleY;
                        pos2[2] = ((pos2[2]) - (smdLine.positionZ * 100f)) / smdLine.scaleZ;

                        pos2 = RotationUtils.RotationInZ(pos2, -smdLine.angleZ);
                        pos2 = RotationUtils.RotationInY(pos2, -smdLine.angleY);
                        pos2 = RotationUtils.RotationInX(pos2, -smdLine.angleX);

                        idxBin.DrawDistancePositiveX = pos2[0];
                        idxBin.DrawDistancePositiveY = pos2[1];
                        idxBin.DrawDistancePositiveZ = pos2[2];

                        BinRenderBox outBox = new BinRenderBox();
                        outBox.DrawDistanceNegativeX = pos1[0];
                        outBox.DrawDistanceNegativeY = pos1[1];
                        outBox.DrawDistanceNegativeZ = pos1[2];
                        outBox.DrawDistancePositiveX = pos2[0];
                        outBox.DrawDistancePositiveY = pos2[1];
                        outBox.DrawDistancePositiveZ = pos2[2];
                        OutBoxes.Add(BinID, outBox);
                    }
                    else 
                    {
                        idxBin.DrawDistanceNegativeX = -327675f;
                        idxBin.DrawDistanceNegativeY = -327675f;
                        idxBin.DrawDistanceNegativeZ = -327675f;
                        idxBin.DrawDistancePositiveX = 655350f;
                        idxBin.DrawDistancePositiveY = 655350f;
                        idxBin.DrawDistancePositiveZ = 655350f;

                        BinRenderBox outBox = new BinRenderBox();
                        outBox.DrawDistanceNegativeX = -327675f;
                        outBox.DrawDistanceNegativeY = -327675f;
                        outBox.DrawDistanceNegativeZ = -327675f;
                        outBox.DrawDistancePositiveX = 655350f;
                        outBox.DrawDistancePositiveY = 655350f;
                        outBox.DrawDistancePositiveZ = 655350f;
                        OutBoxes.Add(BinID, outBox);
                    }

                    IdxBinDic.Add(BinID, idxBin);
                }
            }

        }


        private static SmdBaseLine getGroupInfo(string GroupName)
        {
            SmdBaseLine line = new SmdBaseLine();

            var split = GroupName.Split('#');

            try
            {
                var subSplit = split[1].Split('_');
                int id = int.Parse(subSplit[1].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                line.SmdId = id;
            }
            catch (Exception)
            {
            }

            try
            {
                var subSplit = split[2].Split('_');
                int id = int.Parse(subSplit[1].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                line.SmxId = id;
            }
            catch (Exception)
            {
            }

            try
            {
                var subSplit = split[3].Split('_');
                uint type = uint.Parse(subSplit[1].Trim(), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                line.Type = type;
            }
            catch (Exception)
            {
            }

            try
            {
                var subSplit = split[4].Split('_');
                int id = int.Parse(subSplit[1].Trim(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                line.BinId = id;
            }
            catch (Exception)
            {
            }

            try
            {
                line.useType = UseType.Color;
                if (split[5].Contains("NORMAL"))
                {
                    line.useType = UseType.Normal;
                }
            }
            catch (Exception)
            {
            }


            return line;
        }


    }
}
