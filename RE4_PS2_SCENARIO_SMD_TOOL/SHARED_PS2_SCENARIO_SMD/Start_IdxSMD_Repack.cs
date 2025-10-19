using SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD
{
    public static class Start_IdxSMD_Repack
    {
        public static void IdxSMD_Repack(FileInfo fileInfo)
        {
            Stream idxFile = fileInfo.OpenRead();
            IdxPs2Scenario idxScenario = IdxPs2ScenarioLoader.Loader(idxFile);
            MakeSMD_WithBinFolder.CreateSMD(fileInfo.DirectoryName, idxScenario);
        }
    }
}
