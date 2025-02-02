using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RE4_PS2_SCENARIO_SMD_TOOL
{
    class Program
    {
        public const string VERSION = "B.1.2.5 (2025-02-02)";

        public static string headerText()
        {
            return "# github.com/JADERLINK/RE4-PS2-SCENARIO-SMD-TOOL" + Environment.NewLine +
                   "# youtube.com/@JADERLINK" + Environment.NewLine +
                   "# RE4_PS2_SCENARIO_SMD_TOOL" + Environment.NewLine +
                   "# By JADERLINK" + Environment.NewLine +
                  $"# Version {VERSION}";
        }

        static void Main(string[] args)
        {
            System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            Console.WriteLine(headerText());

            if (args.Length == 0)
            {
                Console.WriteLine("For more information read:");
                Console.WriteLine("https://github.com/JADERLINK/RE4-PS2-SCENARIO-SMD-TOOL");
                Console.WriteLine("Press any key to close the console.");
                Console.ReadKey();
            }
            else if (args.Length >= 1 && File.Exists(args[0]))
            {
                try
                {
                    Actions(args);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                }
               
            }
            else
            {
                Console.WriteLine("The file does not exist;");
            }

            Console.WriteLine("Finished!!!");
        }

        private static void Actions(string[] args)
        {
            FileInfo fileInfo = new FileInfo(args[0]);
            Console.WriteLine("File: " + fileInfo.Name);

            string baseDirectory = fileInfo.DirectoryName;
            string baseName = Path.GetFileNameWithoutExtension(fileInfo.Name);

            string Extension = fileInfo.Extension.ToUpperInvariant();

            // modo extract
            if (Extension == ".SMD")
            {
                string scenarioName = baseName + ".scenario";
                string BinFolder = baseName + "_BIN";
                string tplFileName = baseName + ".TPL";
                string BaseBinDirectory = Path.Combine(baseDirectory, baseName + "_BIN");
                string tplPath = Path.Combine(baseDirectory, tplFileName);

                Dictionary<int, RE4_PS2_BIN_TOOL.EXTRACT.PS2BIN> BinDic = null;
                Dictionary<RE4_PS2_BIN_TOOL.ALL.MaterialPart, string> materialInvDic = null;
                Dictionary<int, SCENARIO.BinRenderBox> Boxes = null;
                int BinRealCount = 0;

                SCENARIO.Ps2ScenarioExtract extractSMD = new SCENARIO.Ps2ScenarioExtract();
                SCENARIO.ToFileMethods toFileMethods = new SCENARIO.ToFileMethods(BaseBinDirectory, tplPath, true);

                extractSMD.ToFileBin += toFileMethods.ToFileBin;
                extractSMD.ToFileTpl += toFileMethods.ToFileTpl;

                var SmdLines = extractSMD.Extract(fileInfo, out BinDic, out Boxes, out BinRealCount);
                var idxMaterial = SCENARIO.Ps2ScenarioMatFix.IdxMaterialMultParser(BinDic, out materialInvDic);

                //create obj
                SCENARIO.OutputScenario.CreateObjScenario(SmdLines, BinDic, materialInvDic, baseDirectory, scenarioName);
                SCENARIO.OutputScenario.CreateSMDmodelReference(SmdLines, baseDirectory, scenarioName);
                SCENARIO.OutputScenario.CreateDrawDistanceObj(SmdLines, Boxes, baseDirectory, scenarioName);

                //material
                RE4_PS2_BIN_TOOL.EXTRACT.OutputMaterial.CreateIdxMaterial(idxMaterial, baseDirectory, scenarioName);
                var mtl = RE4_PS2_BIN_TOOL.ALL.IdxMtlParser.Parser(idxMaterial, baseName);
                RE4_PS2_BIN_TOOL.EXTRACT.OutputMaterial.CreateMTL(mtl, baseDirectory, scenarioName);

                //idx
                SCENARIO.OutputScenario.CreateIdxScenario(SmdLines, Boxes, BinFolder, baseDirectory, scenarioName, fileInfo.Name, tplFileName);
                SCENARIO.OutputScenario.CreateIdxps2Smd(SmdLines, Boxes, BinFolder, baseDirectory, scenarioName, fileInfo.Name, tplFileName, BinRealCount);
                
            }

            //mode repack usando obj
            else if (Extension == ".IDXPS2SCENARIO")
            {
                Stream idxFile = fileInfo.OpenRead();
                SCENARIO.IdxPs2Scenario idxPs2Scenario = SCENARIO.IdxPs2ScenarioLoader.Loader(new StreamReader(idxFile, Encoding.ASCII));


                string objPath =  Path.Combine(baseDirectory, baseName + ".obj");
                string mtlPath = Path.Combine(baseDirectory, baseName + ".mtl");
                string idxmaterialPath = Path.Combine(baseDirectory, baseName + ".idxmaterial");
                string tplPath = Path.Combine(baseDirectory, idxPs2Scenario.TplFile);

                Stream objFile = null;
                Stream mtlFile = null;
                Stream idxmaterialFile = null;

                #region verifica a existencia dos arquivos
                if (File.Exists(objPath))
                {

                    Console.WriteLine("Load File: " + baseName + ".obj");
                    objFile = new FileInfo(objPath).OpenRead();
                }
                else
                {
                    Console.WriteLine("Error: .obj file not found;");
                    return;
                }

                if (idxPs2Scenario.UseIdxMaterial)
                {
                    if (File.Exists(idxmaterialPath))
                    {
                        Console.WriteLine("Load File: " + baseName + ".idxmaterial");
                        idxmaterialFile = new FileInfo(idxmaterialPath).OpenRead();
                    }
                    else
                    {
                        Console.WriteLine("Error: .idxmaterial file not found;");
                        return;
                    }

                }
                else
                {
                    if (File.Exists(mtlPath))
                    {
                        Console.WriteLine("Load File: " + baseName + ".mtl");
                        mtlFile = new FileInfo(mtlPath).OpenRead();
                    }
                    else
                    {
                        Console.WriteLine("Error: .mtl file not found.");
                        return;
                    }

                }

                if (!File.Exists(tplPath))
                {
                    Console.WriteLine("Error: .tpl file not found;");
                    return;
                }

                #endregion

                // carrega os materiais

                RE4_PS2_BIN_TOOL.ALL.IdxMaterial material = null;
                RE4_PS2_BIN_TOOL.ALL.IdxMtl idxMtl = null;

                if (idxmaterialFile != null)
                {
                    material = RE4_PS2_BIN_TOOL.ALL.IdxMaterialLoad.Load(idxmaterialFile);
                    idxmaterialFile.Close();
                }

                if (mtlFile != null) // .MTL
                {
                    RE4_PS2_BIN_TOOL.REPACK.MtlLoad.Load(mtlFile, out idxMtl);
                    mtlFile.Close();
                }

                if (idxMtl != null)
                {
                    Console.WriteLine("Converting .mtl");
                    RE4_PS2_BIN_TOOL.REPACK.MtlConverter.Convert(idxMtl, out material);
                    RE4_PS2_BIN_TOOL.EXTRACT.OutputMaterial.CreateIdxMaterial(material, baseDirectory, baseName + ".Repack");
                }

                //----
                // cria o arquivo smd e os bins

                Console.WriteLine("Reading and converting .obj");
                Dictionary<int, SCENARIO.SmdBaseLine> objGroupInfos = null;
                Dictionary<int, RE4_PS2_BIN_TOOL.REPACK.FinalStructure> finalBinList = null;
                Dictionary<int, RE4_PS2_BIN_TOOL.REPACK.IdxBin> IdxBinDic = null;
                Dictionary<int, SCENARIO.SMDLineIdx> SMDLineIdxDic = null;
                Dictionary<int, float> ConversionFactorValueDic = null;
                Dictionary<int, SCENARIO.BinRenderBox> Boxes = null;

                SCENARIO.Ps2ScenarioRepack.RepackOBJ(
                    objFile, 
                    ref idxPs2Scenario, 
                    out objGroupInfos, 
                    out finalBinList, 
                    out IdxBinDic, 
                    out SMDLineIdxDic, 
                    out ConversionFactorValueDic,
                    out Boxes);

                SCENARIO.MakeSMD_Scenario.CreateSMD(
                    baseDirectory,
                    idxPs2Scenario.SmdFileName,
                    tplPath,
                    objGroupInfos,
                    idxPs2Scenario,
                    SMDLineIdxDic,
                    ConversionFactorValueDic, 
                    finalBinList, 
                    IdxBinDic, 
                    material, 
                    true);


                //cria um novo idxps2smd
                Console.WriteLine("Creating new .idxuhdsmd");
                SCENARIO.SMDLine[] smdLines = SCENARIO.SmdLineParcer.Parser(idxPs2Scenario.SmdAmount, idxPs2Scenario.SmdLines, SMDLineIdxDic, objGroupInfos);
                SCENARIO.OutputScenario.CreateIdxps2Smd(smdLines, Boxes, idxPs2Scenario.BinFolder, baseDirectory, baseName + ".Repack", idxPs2Scenario.SmdFileName, idxPs2Scenario.TplFile, idxPs2Scenario.BinAmount);

                // cria novo SMDmodelReference
                SCENARIO.OutputScenario.CreateSMDmodelReference(smdLines, baseDirectory, baseName + ".Repack");

            }
            
            //mode repack usando BIN
            else if (Extension == ".IDXPS2SMD")
            {
                Stream idxFile = fileInfo.OpenRead();
                SCENARIO.IdxPs2Scenario idxPs2Smd = SCENARIO.IdxPs2ScenarioLoader.Loader(new StreamReader(idxFile, Encoding.ASCII));

                SCENARIO.MakeSMD_WithBinFolder.CreateSMD(baseDirectory, idxPs2Smd);
            }

            else
            {
                Console.WriteLine("Invalid file format;");
            }
        }





    }
}