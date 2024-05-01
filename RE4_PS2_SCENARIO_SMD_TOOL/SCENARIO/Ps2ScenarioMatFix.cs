using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.EXTRACT;
using RE4_PS2_BIN_TOOL.ALL;


namespace RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO
{
    public static class Ps2ScenarioMatFix
    {
        public static IdxMaterial IdxMaterialMultParser(Dictionary<int, PS2BIN> BinDic, out Dictionary<MaterialPart, string> invDic)
        {
            IdxMaterial idx = new IdxMaterial();
            idx.MaterialDic = new Dictionary<string, MaterialPart>();
            invDic = new Dictionary<MaterialPart, string>();

            int counter = 0;

            foreach (var item in BinDic)
            {
                if (item.Value != null)
                {
                    for (int i = 0; i < item.Value.materials.Length; i++)
                    {
                        var mat = new MaterialPart(item.Value.materials[i].materialLine);

                        if (!invDic.ContainsKey(mat))
                        {
                            string matKey = CONSTs.SCENARIO_MATERIAL + counter.ToString("D3");
                            invDic.Add(mat, matKey);
                            idx.MaterialDic.Add(matKey, mat);
                            counter++;
                        }

                    }
                }

            }
            return idx;
        }

    }
}
