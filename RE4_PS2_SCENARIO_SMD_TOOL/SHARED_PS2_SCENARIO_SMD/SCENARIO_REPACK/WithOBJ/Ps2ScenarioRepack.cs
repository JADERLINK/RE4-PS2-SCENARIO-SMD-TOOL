using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SHARED_PS2_BIN.REPACK;
using SHARED_TOOLS.ALL;
using SHARED_TOOLS.SCENARIO;
using SHARED_PS2_BIN.REPACK.Structures;
using SHARED_PS2_BIN.ALL;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK.WithOBJ
{
    public static partial class Ps2ScenarioRepack
    {
        public static void RepackOBJ(Stream objFile, IdxPs2Scenario idxScenario,
            out Dictionary<int, SmdBaseLine> ObjGroupInfosDic,
            out Dictionary<int, FinalStructure> FinalBinDic,
            out Dictionary<int, RepackProps> RepackPropsDic,
            out Dictionary<int, SMDLineIdx> SMDLineIdxDic,
            out Dictionary<int, float> ConversionFactorValueDic)
        {

            string patternPS2SCENARIO = "^(PS2SCENARIO#SMD_)([0]{0,})([0-9]{1,4})(#SMX_)([0]{0,})([0-9]{1,3})(#TYPE_)([0]{0,})([0-9|A-F]{1,8})(#BIN_)([0]{0,})([0-9]{1,3})(#)(COLOR|NORMAL)(#).*$";
            System.Text.RegularExpressions.Regex regexScenario = new System.Text.RegularExpressions.Regex(patternPS2SCENARIO, System.Text.RegularExpressions.RegexOptions.CultureInvariant);

            bool LoadColorsFromObjFile = true;

            // load .obj file
            var objLoaderFactory = new ObjLoader.Loader.Loaders.ObjLoaderFactory();
            var objLoader = objLoaderFactory.Create();
            StreamReader streamReader = null;
            ObjLoader.Loader.Loaders.LoadResult arqObj = null;

            try
            {
                streamReader = new StreamReader(objFile, Encoding.ASCII);
                arqObj = objLoader.Load(streamReader);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                streamReader?.Close();
            }

            //lista de materiais usados no modelo
            HashSet<string> ModelMaterialsToUpper = new HashSet<string>();

            Vector4 color = new Vector4(1, 1, 1, 1);
            StartWeightMap weightMap = new StartWeightMap(1, 0, 1, 0, 0, 0, 0);

            //conjunto de struturas
            //id do SMD/ conteudo para o SMD/BIN
            Dictionary<int, StartStructure> ObjList = new Dictionary<int, StartStructure>();
            //id do SMD, outras informações 
            ObjGroupInfosDic = new Dictionary<int, SmdBaseLine>();

            for (int iG = 0; iG < arqObj.Groups.Count; iG++)
            {
                string GroupName = arqObj.Groups[iG].GroupName.ToUpperInvariant().Trim();
                string materialNameInvariant = arqObj.Groups[iG].MaterialName.ToUpperInvariant().Trim();

                //FIX NAME
                GroupName = GroupName.Replace("_", "#")
                    .Replace("SMD#", "SMD_")
                    .Replace("SMX#", "SMX_")
                    .Replace("TYPE#", "TYPE_")
                    .Replace("BIN#", "BIN_")
                    ;

                if (GroupName.StartsWith("PS2SCENARIO"))
                {
                    //REGEX
                    if (regexScenario.IsMatch(GroupName))
                    {
                        Console.WriteLine("Loading in Obj: " + GroupName + " | " + materialNameInvariant);
                    }
                    else
                    {
                        Console.WriteLine("Loading in Obj: " + GroupName + " | " + materialNameInvariant + "  The group name is wrong, group not used;");
                        continue;
                    }

                    SmdBaseLine info = getGroupInfo(GroupName);

                    if (!ObjGroupInfosDic.ContainsKey(info.SmdId) && info.SmdId < LimitConsts.SmdLineLimit)
                    {
                        ObjGroupInfosDic.Add(info.SmdId, info);
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
                                throw new ApplicationException("Vertex Position Index is invalid! Value: " + arqObj.Groups[iG].Faces[iF][iI].VertexIndex);
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

                    if (info.SmdId < LimitConsts.SmdLineLimit)
                    {
                        if (ObjList.ContainsKey(info.SmdId))
                        {
                            if (ObjList[info.SmdId].FacesByMaterial.ContainsKey(materialNameInvariant))
                            {
                                ObjList[info.SmdId].FacesByMaterial[materialNameInvariant].Faces.AddRange(facesList);
                            }
                            else
                            {
                                ModelMaterialsToUpper.Add(materialNameInvariant);
                                ObjList[info.SmdId].FacesByMaterial.Add(materialNameInvariant, new StartFacesGroup(facesList));
                            }
                        }
                        else
                        {
                            StartStructure startStructure = new StartStructure();
                            ModelMaterialsToUpper.Add(materialNameInvariant);
                            startStructure.FacesByMaterial.Add(materialNameInvariant, new StartFacesGroup(facesList));
                            ObjList.Add(info.SmdId, startStructure);
                        }
                    }
                    else
                    {
                        Console.WriteLine("This part of the modeling was not added because the SMD_ID exceed the allowed limit.");
                    }
                }
                else
                {
                    Console.WriteLine("Loading in Obj: " + GroupName + " | " + materialNameInvariant + "   Warning: Group not used;");
                }

            }

            var smdCount = ObjGroupInfosDic.Any() ? ObjGroupInfosDic.Max(a => a.Key) + 1 : 0;
            for (int i = 0; i < smdCount && i < LimitConsts.SmdLineLimit; i++)
            {
                if (!ObjGroupInfosDic.ContainsKey(i))
                {
                    SmdBaseLine smdBaseLine = new SmdBaseLine();
                    smdBaseLine.SmdId = i;
                    smdBaseLine.SmxId = 0xFE;
                    smdBaseLine.Type = 0;
                    smdBaseLine.BinId = 0;
                    smdBaseLine.useType = UseType.Color;
                    ObjGroupInfosDic.Add(i, smdBaseLine);
                }
            }

            //----
            FinalBinDic = new Dictionary<int, FinalStructure>();
            RepackPropsDic = new Dictionary<int, RepackProps>();
            SMDLineIdxDic = new Dictionary<int, SMDLineIdx>();
            ConversionFactorValueDic = new Dictionary<int, float>();

            foreach (var item in ObjList.OrderBy(a => a.Key).ToArray())
            {
                int BinID = ObjGroupInfosDic[item.Key].BinId;

                if (!FinalBinDic.ContainsKey(BinID))
                {
                    // faz a compressão das vertives
                    Console.WriteLine("BIN ID: " + BinID.ToString("D3"));
                    item.Value.CompressAllFaces();
                    //-----

                    SMDLineIdx smdLineIdx = new SMDLineIdx();
                    smdLineIdx.ScaleX = 1f;
                    smdLineIdx.ScaleY = 1f;
                    smdLineIdx.ScaleZ = 1f;

                    if (idxScenario.SmdLinesDic.ContainsKey(item.Key))
                    {
                        smdLineIdx = idxScenario.SmdLinesDic[item.Key].Clone();
                    }

                    (float X, float Y, float Z) position = (0,0,0);
                    float ConversionFactorValue = 0;
                    BoundingBox calculatedBoundingBox;

                    var intermediary = BINrepackIntermediary.MakeIntermediaryStructure(item.Value, smdLineIdx, out position, out ConversionFactorValue, out calculatedBoundingBox);
                    FinalStructure final = BINrepackFinal.MakeFinalStructure(intermediary, ConversionFactorValue);
                    FinalBinDic.Add(BinID, final);
                    ConversionFactorValueDic.Add(BinID, ConversionFactorValue);

                    smdLineIdx.PositionX = position.X;
                    smdLineIdx.PositionY = position.Y;
                    smdLineIdx.PositionZ = position.Z;
                    SMDLineIdxDic.Add(item.Key, smdLineIdx);

                    //conteudo complementar
                    RepackProps repackProps = new RepackProps();
                    repackProps.BonePairs = new (ushort b1, ushort b2, ushort b3, ushort b4)[0];
                    repackProps.BoundingBox.BoundingBoxPosW = 1f;

                    repackProps.EnableUnkFlag2 = true; // flag sempre ativa, mas não tem utilidade

                    if (ObjGroupInfosDic[item.Key].useType == UseType.Color)
                    {
                        repackProps.IsScenarioBin = true;
                    }
                    else
                    {
                        repackProps.IsScenarioBin = false;
                        repackProps.EnableUnkFlag1 = true; //flag sempre ativa no tipo normal.
                    }

                    //---------------
                    BoundingBox idxBoundingBox = new BoundingBox();
                    idxBoundingBox.BoundingBoxPosW = 1f;

                    if (idxScenario.RenderLimitBoxes.ContainsKey(item.Key))
                    {
                        float scaleX = smdLineIdx.ScaleX != 0 ? smdLineIdx.ScaleX : 1f;
                        float scaleY = smdLineIdx.ScaleY != 0 ? smdLineIdx.ScaleY : 1f;
                        float scaleZ = smdLineIdx.ScaleZ != 0 ? smdLineIdx.ScaleZ : 1f;

                        RenderLimitBox box = idxScenario.RenderLimitBoxes[item.Key];

                        float[] minPos = new float[3];// 0 = x, 1 = y, 2 = z
                        minPos[0] = box.BBoxMinX * CONSTs.GLOBAL_POSITION_SCALE;
                        minPos[1] = box.BBoxMinY * CONSTs.GLOBAL_POSITION_SCALE;
                        minPos[2] = box.BBoxMinZ * CONSTs.GLOBAL_POSITION_SCALE;

                        minPos[0] = ((minPos[0]) - (smdLineIdx.PositionX * CONSTs.GLOBAL_POSITION_SCALE)) / scaleX;
                        minPos[1] = ((minPos[1]) - (smdLineIdx.PositionY * CONSTs.GLOBAL_POSITION_SCALE)) / scaleY;
                        minPos[2] = ((minPos[2]) - (smdLineIdx.PositionZ * CONSTs.GLOBAL_POSITION_SCALE)) / scaleZ;

                        minPos = RotationUtils.RotationInZ(minPos, -smdLineIdx.AngleZ);
                        minPos = RotationUtils.RotationInY(minPos, -smdLineIdx.AngleY);
                        minPos = RotationUtils.RotationInX(minPos, -smdLineIdx.AngleX);

                        float[] maxPos = new float[3];// 0 = x, 1 = y, 2 = z
                        maxPos[0] = box.BBoxMaxX * CONSTs.GLOBAL_POSITION_SCALE;
                        maxPos[1] = box.BBoxMaxY * CONSTs.GLOBAL_POSITION_SCALE;
                        maxPos[2] = box.BBoxMaxZ * CONSTs.GLOBAL_POSITION_SCALE;

                        maxPos[0] = ((maxPos[0]) - (smdLineIdx.PositionX * CONSTs.GLOBAL_POSITION_SCALE)) / scaleX;
                        maxPos[1] = ((maxPos[1]) - (smdLineIdx.PositionY * CONSTs.GLOBAL_POSITION_SCALE)) / scaleY;
                        maxPos[2] = ((maxPos[2]) - (smdLineIdx.PositionZ * CONSTs.GLOBAL_POSITION_SCALE)) / scaleZ;

                        maxPos = RotationUtils.RotationInZ(maxPos, -smdLineIdx.AngleZ);
                        maxPos = RotationUtils.RotationInY(maxPos, -smdLineIdx.AngleY);
                        maxPos = RotationUtils.RotationInX(maxPos, -smdLineIdx.AngleX);

                        float CenterX = (minPos[0] + maxPos[0]) / 2;
                        float CenterY = (minPos[1] + maxPos[1]) / 2;
                        float CenterZ = (minPos[2] + maxPos[2]) / 2;

                        float SemiDistanceX = Math.Abs(maxPos[0] - minPos[0]) / 2;
                        float SemiDistanceY = Math.Abs(maxPos[1] - minPos[1]) / 2;
                        float SemiDistanceZ = Math.Abs(maxPos[2] - minPos[2]) / 2;

                        idxBoundingBox.BoundingBoxPosX = CenterX;
                        idxBoundingBox.BoundingBoxPosY = CenterY;
                        idxBoundingBox.BoundingBoxPosZ = CenterZ;
                        idxBoundingBox.BoundingBoxPosW = 1f;
                        idxBoundingBox.BoundingBoxWidth = SemiDistanceX;
                        idxBoundingBox.BoundingBoxHeight = SemiDistanceY;
                        idxBoundingBox.BoundingBoxDepth = SemiDistanceZ;
                    }

                    repackProps.BoundingBox = idxBoundingBox;

                    if (idxScenario.AutoCalcBoundingBox)
                    {
                        repackProps.BoundingBox = calculatedBoundingBox;
                    }

                    if (idxScenario.IgnoreBoundingBox)
                    {
                        repackProps.BoundingBox = new BoundingBox();
                        repackProps.BoundingBox.BoundingBoxPosW = 1f;
                    }

                    RepackPropsDic.Add(BinID, repackProps);
                }
            }

            // adiciona SmdLineIdx faltantes

            for (int i = 0; i < smdCount && i < LimitConsts.SmdLineLimit; i++)
            {
                if (!SMDLineIdxDic.ContainsKey(i))
                {
                    if (idxScenario.SmdLinesDic.ContainsKey(i))
                    {
                        SMDLineIdxDic.Add(i, idxScenario.SmdLinesDic[i].Clone());
                    }
                    else 
                    {
                        SMDLineIdx smdLineIdx = new SMDLineIdx();
                        smdLineIdx.ScaleX = 1f;
                        smdLineIdx.ScaleY = 1f;
                        smdLineIdx.ScaleZ = 1f;
                        SMDLineIdxDic.Add(i, smdLineIdx);
                    }
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
                line.SmxId = 0xFE;
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
