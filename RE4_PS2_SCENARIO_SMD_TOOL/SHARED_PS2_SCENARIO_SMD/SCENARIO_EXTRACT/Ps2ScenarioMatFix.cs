using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHARED_PS2_BIN.EXTRACT;
using SHARED_PS2_BIN.ALL;
using SHARED_TOOLS.ALL;
using SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT;
using SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT;

namespace RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO_EXTRACT
{
    public static class Ps2ScenarioMatFix
    {
        public static IdxMaterial IdxMaterialMultiParser(SMDLine[] smdLines, Dictionary<int, PS2GenericModelBIN> BinDic,
            out Dictionary<(MaterialPart mat, byte TplFileID), string> invDic,
            out Dictionary<(string MaterialName, byte TplFileID), MaterialPart> materialWithTplFileIdDic)
        {
            IdxMaterial idx = new IdxMaterial();
            idx.MaterialDic = new Dictionary<string, MaterialPart>();
            invDic = new Dictionary<(MaterialPart mat, byte TplFileID), string>();
            materialWithTplFileIdDic = new Dictionary<(string MaterialName, byte TplFileID), MaterialPart>();

            int counter = 0;

            foreach (var smdLine in smdLines)
            {
                if (BinDic.ContainsKey(smdLine.BinFileID))
                {
                    for (int i = 0; i < BinDic[smdLine.BinFileID].Materials.Length; i++)
                    {
                        MaterialPart mat = BinDic[smdLine.BinFileID].Materials[i].material;
                        byte TplFileID = smdLine.TplFileID;

                        if (!invDic.ContainsKey((mat, TplFileID)))
                        {
                            string matKey = CONSTs.SCENARIO_MATERIAL + counter.ToString("D3");
                            invDic.Add((mat, TplFileID), matKey);
                            idx.MaterialDic.Add(matKey, mat);
                            materialWithTplFileIdDic.Add((matKey, TplFileID), mat);
                            counter++;
                        }
                    }

                }

            }

            return idx;
        }

    }
}
