using SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT.OutputFiles;
using SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT.OutputFiles;
using SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK;
using SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK.WithOBJ;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD
{
    public static class Start_ScenarioOBJ_Repack
    {
        public static void ScenarioOBJ_Repack(FileInfo fileInfo1) 
        {
            Stream idxStream = fileInfo1.OpenRead();
            IdxPs2Scenario idxPs2Scenario = IdxPs2ScenarioLoader.Loader(idxStream);

            ValidateMagic.Validate(idxPs2Scenario.Magic);

            string baseDirectory = fileInfo1.DirectoryName;
            string baseFileName = Path.GetFileNameWithoutExtension(fileInfo1.Name);

            string objPath = Path.Combine(baseDirectory, baseFileName + ".obj");
            string mtlPath = Path.Combine(baseDirectory, baseFileName + ".mtl");
            string idxmaterialPath = Path.Combine(baseDirectory, baseFileName + ".idxmaterial");

            string TplFileName = idxPs2Scenario.TplFileName;
            string TplFileFullPath = Path.Combine(baseDirectory, TplFileName);

            Stream objFile = null;
            Stream mtlFile = null;
            Stream idxmaterialFile = null;

            Action CloseOpenedStreams = () => {
                objFile?.Close();
                mtlFile?.Close();
                idxmaterialFile?.Close();
            };


            #region verifica a existencia dos arquivos

            if (File.Exists(TplFileFullPath) == false)
            {
                throw new ApplicationException("The TPL file does not exist: " + TplFileName);
            }

            if (File.Exists(objPath))
            {
                Console.WriteLine("Load File: " + baseFileName + ".obj");
                objFile = new FileInfo(objPath).OpenRead();
            }
            else
            {
                Console.WriteLine("Error: OBJ file not found!");
                CloseOpenedStreams();
                return;
            }

            if (idxPs2Scenario.UseIdxMaterial)
            {
                if (File.Exists(idxmaterialPath))
                {
                    Console.WriteLine("Load File: " + baseFileName + ".idxmaterial");
                    idxmaterialFile = new FileInfo(idxmaterialPath).OpenRead();
                }
                else
                {
                    Console.WriteLine("Error: IDXMATERIAL file not found!");
                    CloseOpenedStreams();
                    return;
                }

            }
            else
            {
                if (File.Exists(mtlPath))
                {
                    Console.WriteLine("Load File: " + baseFileName + ".mtl");
                    mtlFile = new FileInfo(mtlPath).OpenRead();
                }
                else
                {
                    Console.WriteLine("Error: .mtl file not found.");
                    return;
                }

            }

            #endregion

            // carrega os materiais

            SHARED_PS2_BIN.ALL.IdxMaterial material = null;
            SHARED_PS2_BIN.ALL.IdxMtl idxMtl = null;

            if (idxmaterialFile != null)
            {
                Console.WriteLine("Processing IDXMATERIAL");
                material = SHARED_PS2_BIN.ALL.IdxMaterialLoad.Load(idxmaterialFile);
                idxmaterialFile.Close();
            }

            if (mtlFile != null) // .MTL
            {
                Console.WriteLine("Processing MTL");
                SHARED_PS2_BIN.REPACK.MtlLoad.Load(mtlFile, out idxMtl);
                mtlFile.Close();
            }

            if (idxMtl != null)
            {
                Console.WriteLine("Converting MTL");
                SHARED_PS2_BIN.REPACK.MtlConverter.Convert(idxMtl, out material);
            }

            //----
            // cria o arquivo smd e os bins

            Console.WriteLine("Reading and converting OBJ");
            Dictionary<int, SmdBaseLine> ObjGroupInfosDic = null;
            Dictionary<int, SHARED_PS2_BIN.REPACK.Structures.FinalStructure> FinalBinDic = null;
            Dictionary<int, SHARED_PS2_BIN.REPACK.RepackProps> RepackPropsDic = null;
            Dictionary<int, SMDLineIdx> SMDLineIdxDic = null;
            Dictionary<int, float> ConversionFactorValueDic = null;

            Ps2ScenarioRepack.RepackOBJ(
                objFile,
                idxPs2Scenario,
                out ObjGroupInfosDic,
                out FinalBinDic,
                out RepackPropsDic,
                out SMDLineIdxDic,
                out ConversionFactorValueDic);

            int smdLinesCount = ObjGroupInfosDic.Any() ? ObjGroupInfosDic.Max(a => a.Key) + 1 : 0;
            int binFilesCount = 0;
            int tplFilesCount = 1;
            var SmdLines = SmdLineParcer.Parser(smdLinesCount, SMDLineIdxDic, ObjGroupInfosDic, out binFilesCount);
            SmdLineParcer.SetTplFileIDInSmdLine(ref SmdLines, out tplFilesCount, idxPs2Scenario.SmdLinesPart2Dic);

            Console.WriteLine("FILE INFO:");
            Console.WriteLine("SMD Entry Count: " + smdLinesCount);
            Console.WriteLine("BIN Files Count: " + binFilesCount);
            Console.WriteLine("TPL Files Count: " + tplFilesCount);

            //------

            long BinAreaOffset;
            string smdFilePath = Path.GetFullPath(Path.Combine(baseDirectory, idxPs2Scenario.SmdFileName));
            BinaryWriter bw = new BinaryWriter(new FileInfo(smdFilePath).Create());
            MakeSMD_Top.FillTopSmd(bw, idxPs2Scenario.Magic, SmdLines, 0, out BinAreaOffset);

            MakeSMD_Fill_BIN_PS2 makeSMD_Fill_BIN_PS2 = new MakeSMD_Fill_BIN_PS2( FinalBinDic, RepackPropsDic, ConversionFactorValueDic, material, true, idxPs2Scenario.BinFolder);

            long TplAreaOffset;
            makeSMD_Fill_BIN_PS2.Fill(bw, binFilesCount, BinAreaOffset, out TplAreaOffset);


            MakeSMD_Fill_TPL_PS2 makeSMD_Fill_TPL_PS2 = new MakeSMD_Fill_TPL_PS2(TplFileFullPath);
            makeSMD_Fill_TPL_PS2.Fill(bw, tplFilesCount, TplAreaOffset, out _);

            //coloca os offsets no topo
            bw.BaseStream.Position = 4;
            bw.Write((uint)BinAreaOffset);
            bw.Write((uint)TplAreaOffset);

            bw.Close();

            Console.WriteLine("Creating file: " + baseFileName + ".Repack.idx_ps2_smd");
            string idxSmdPath = Path.Combine(baseDirectory, baseFileName + ".Repack.idx_ps2_smd");
            ScenarioIdx.CreateIdxSmd(idxSmdPath, SmdLines, idxPs2Scenario.Magic, idxPs2Scenario.BinFolder, idxPs2Scenario.SmdFileName, TplFileName);

            // debugs files
            if (File.Exists(Path.Combine(AppContext.BaseDirectory, "CreateReferenceSMD.txt")))
            {
                Console.WriteLine("Creating File: " + baseFileName + ".Repack.Reference.smd");
                string fullFilePath = Path.Combine(baseDirectory, baseFileName + ".Repack.Reference.smd");
                DebugFiles.CreateSmdModelReference(fullFilePath, SmdLines);
            }

            if (File.Exists(Path.Combine(AppContext.BaseDirectory, "CreateBoundingBoxOBJ.txt")))
            {
                var boxes = RepackPropsDic.Select(a => (a.Key, a.Value.BoundingBox)).ToDictionary(k => k.Key, v => v.BoundingBox);
                Console.WriteLine("Creating File: " + baseFileName + ".Repack.BoundingBox.obj");
                string fullFilePath = Path.Combine(baseDirectory, baseFileName + ".Repack.BoundingBox.obj");
                DebugFiles.CreateBoundingBoxOBJ(fullFilePath, SmdLines, boxes);
            }
        }
    }
}
