using RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO_EXTRACT;
using SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT;
using SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT.OutputFiles;
using SHARED_PS2_BIN.ALL;
using SHARED_PS2_BIN.EXTRACT;
using SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT;
using SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT.OutputFiles;
using SHARED_SCENARIO_SMD.SCENARIO_EXTRACT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD
{
    public static class Start_SMD_Extract
    {
        public static void SMD_Extract(FileInfo fileInfo1)
        {
            string baseDirectory = fileInfo1.DirectoryName;
            string baseFileName = Path.GetFileNameWithoutExtension(fileInfo1.Name);

            string baseNameScenario = baseFileName + ".scenario";
            string baseNameBinFolder = baseFileName + "_BIN";
            string baseNameBinFolderRepack = baseFileName + "_REPACK";
            string baseSubDirectory = Path.Combine(baseDirectory, baseNameBinFolder);
            Stream smdfile = fileInfo1.OpenRead();

            ushort smdMagic;
            uint OffsetBinArr = 0;
            uint OffsetTplArr = 0;
            SMDLine[] smdLines = SmdExtract2007PS2.Extract(smdfile, out smdMagic, out OffsetBinArr, out OffsetTplArr);

            Extract_BIN_Inside_SMD_PS2 extract_BIN_Inside_SMD = new Extract_BIN_Inside_SMD_PS2();

            Extract_BIN_Content_PS2 extract_BIN_Content = new Extract_BIN_Content_PS2();
            extract_BIN_Inside_SMD.ToExtractBin = extract_BIN_Content.ToExtractBin;

            ToFileMethod_BIN toFileMethod_BIN = new ToFileMethod_BIN(baseSubDirectory, true);
            extract_BIN_Inside_SMD.ToFileBin = toFileMethod_BIN.ToFileBin;

            Extract_TPL_Inside_SMD_PS2 extract_TPL_Inside_SMD = new Extract_TPL_Inside_SMD_PS2();

            ToFileMethod_TPL_PS2 toFileMethod_TPL = new ToFileMethod_TPL_PS2(true, baseDirectory, baseFileName);
            extract_TPL_Inside_SMD.ToFileTpl = toFileMethod_TPL.ToFileTpl;


            int BinFilesCount = 0;
            int TplFilesCount = 0;
            CounterBinTpl.Calc(smdLines, out BinFilesCount, out TplFilesCount);

            Console.WriteLine("Extracting BIN and TPL files");
            extract_BIN_Inside_SMD.ExtractBINs(smdfile, OffsetBinArr, BinFilesCount);
            extract_TPL_Inside_SMD.ExtractTPLs(smdfile, OffsetTplArr, TplFilesCount);

            smdfile.Close();

            //---------------

            Dictionary<(MaterialPart mat, byte TplFileID), string> materialDic;
            Dictionary<(string MaterialName, byte TplFileID), MaterialPart> materialWithTplFileIdDic;
            var idxMaterial = Ps2ScenarioMatFix.IdxMaterialMultiParser(smdLines, extract_BIN_Content.BIN_DIC, out materialDic, out materialWithTplFileIdDic);


            Console.WriteLine("Creating File: " + baseNameScenario + ".obj");
            ScenarioOBJ.CreateOBJ(smdLines, extract_BIN_Content.BIN_DIC, materialDic, baseDirectory, baseNameScenario);

            string tplFileName = baseFileName + ".TPL";

            var TplFileCount = materialWithTplFileIdDic.Any() ? materialWithTplFileIdDic.Max(a => a.Key.TplFileID) + 1 : 1;
            IdxMtl _idxMtl = new IdxMtl() { MtlDic = new Dictionary<string, MtlObj>() };

            for (int i = 0; i < TplFileCount; i++)
            {
                string newTplName = baseFileName;
                if (i != 0)
                {
                    newTplName += "." + i.ToString("D");
                }

                var temp_idxMaterial = new IdxMaterial()
                {
                    MaterialDic = materialWithTplFileIdDic.Where(a => a.Key.TplFileID == i).ToDictionary(k => k.Key.MaterialName, v => v.Value)
                };

                var temp_mtl = IdxMtlParser.Parser(temp_idxMaterial, newTplName);
                foreach (var mtlObj in temp_mtl.MtlDic)
                {
                    _idxMtl.MtlDic.Add(mtlObj.Key, mtlObj.Value);
                }
            }
            _idxMtl.MtlDic = _idxMtl.MtlDic.OrderBy(a => a.Key).ToDictionary(K => K.Key, v => v.Value);

            Console.WriteLine("Creating File: " + baseNameScenario + ".idxmaterial");
            OutputMaterial.CreateIdxMaterial(idxMaterial, baseDirectory, baseNameScenario);

            Console.WriteLine("Creating File: " + baseNameScenario + ".mtl");
            OutputMaterial.CreateMTL(_idxMtl, baseDirectory, baseNameScenario);

            Console.WriteLine("Creating File: " + baseNameScenario + ".idx_ps2_scenario");
            string idxScenarioPath = Path.Combine(baseDirectory, baseNameScenario + ".idx_ps2_scenario");
            ScenarioIdx.CreateIdxScenario(idxScenarioPath, smdLines, extract_BIN_Content.Boxes, smdMagic, baseNameBinFolderRepack, fileInfo1.Name, tplFileName);

            Console.WriteLine("Creating File: " + baseNameScenario + ".idx_ps2_smd");
            string idxSmdPath = Path.Combine(baseDirectory, baseNameScenario + ".idx_ps2_smd");
            ScenarioIdx.CreateIdxSmd(idxSmdPath, smdLines, smdMagic, baseNameBinFolder, fileInfo1.Name, tplFileName);

            // debugs files
            if (File.Exists(Path.Combine(AppContext.BaseDirectory, "CreateReferenceSMD.txt")))
            {
                Console.WriteLine("Creating File: " + baseNameScenario + ".Debug.Reference.smd");
                string fullFilePath = Path.Combine(baseDirectory, baseFileName + ".Debug.Reference.smd");
                DebugFiles.CreateSmdModelReference(fullFilePath, smdLines);
            }

            if (File.Exists(Path.Combine(AppContext.BaseDirectory, "CreateBoundingBoxOBJ.txt")))
            {
                Console.WriteLine("Creating File: " + baseNameScenario + ".Debug.BoundingBox.obj");
                string fullFilePath = Path.Combine(baseDirectory, baseFileName + ".Debug.BoundingBox.obj");
                DebugFiles.CreateBoundingBoxOBJ(fullFilePath, smdLines, extract_BIN_Content.Boxes);
            }

        }
    }
}
