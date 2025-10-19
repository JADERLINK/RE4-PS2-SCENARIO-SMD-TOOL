using SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT;
using System;
using System.Collections.Generic;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT
{
    public static class CounterBinTpl
    {
        public static void Calc(SMDLine[] SMDLines, out int BinFilesCount, out int TplFilesCount)
        {
            BinFilesCount = 0;
            TplFilesCount = 0;

            foreach (var line in SMDLines)
            {
                if (line.BinFileID + 1 > BinFilesCount)
                {
                    BinFilesCount = line.BinFileID + 1;
                }

                if (line.TplFileID + 1 > TplFilesCount)
                {
                    TplFilesCount = line.TplFileID + 1;
                }
            }

        }

    }
}
