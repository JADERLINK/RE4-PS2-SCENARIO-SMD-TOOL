using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO
{
    public static class SmdLineParcer
    {
        public static SMDLine[] Parser(int SmdAmount, SMDLineIdx[] SmdLines, Dictionary<int, SMDLineIdx> SMDLineIdxDic, Dictionary<int, SmdBaseLine> objGroupInfos)
        {
            SMDLine[] smdLines = new SMDLine[SmdAmount];
            for (int i = 0; i < SmdAmount; i++)
            {
                SMDLine line = new SMDLine();

                line.scaleX = 1f;
                line.scaleY = 1f;
                line.scaleZ = 1f;
                line.scaleW = 1f;
                line.angleW = 1f;
                line.positionW = 1f;
                line.FixedFF = 0xFF;

                if (SMDLineIdxDic.ContainsKey(i))
                {
                    line.positionX = SMDLineIdxDic[i].positionX * CONSTs.GLOBAL_SCALE;
                    line.positionY = SMDLineIdxDic[i].positionY * CONSTs.GLOBAL_SCALE;
                    line.positionZ = SMDLineIdxDic[i].positionZ * CONSTs.GLOBAL_SCALE;
                    line.scaleX = SMDLineIdxDic[i].scaleX;
                    line.scaleY = SMDLineIdxDic[i].scaleY;
                    line.scaleZ = SMDLineIdxDic[i].scaleZ;
                    line.angleX = SMDLineIdxDic[i].angleX;
                    line.angleY = SMDLineIdxDic[i].angleY;
                    line.angleZ = SMDLineIdxDic[i].angleZ;
                }
                else if (SmdLines.Length > i)
                {
                    line.positionX = SmdLines[i].positionX * CONSTs.GLOBAL_SCALE;
                    line.positionY = SmdLines[i].positionY * CONSTs.GLOBAL_SCALE;
                    line.positionZ = SmdLines[i].positionZ * CONSTs.GLOBAL_SCALE;
                    line.scaleX = SmdLines[i].scaleX;
                    line.scaleY = SmdLines[i].scaleY;
                    line.scaleZ = SmdLines[i].scaleZ;
                    line.angleX = SmdLines[i].angleX;
                    line.angleY = SmdLines[i].angleY;
                    line.angleZ = SmdLines[i].angleZ;
                }

                if (objGroupInfos.ContainsKey(i))
                {
                    line.BinID = (ushort)objGroupInfos[i].BinId;
                    line.SmxID = (byte)objGroupInfos[i].SmxId;
                    line.FixedFF = 0xFF;
                    line.objectStatus = objGroupInfos[i].Type;
                }

                smdLines[i] = line;
            }

            return smdLines;
        }




    }

}
