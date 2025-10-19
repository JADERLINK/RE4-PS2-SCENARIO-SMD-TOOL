using SHARED_TOOLS.ALL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT;
using SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK.WithOBJ;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK
{
    public static class SmdLineParcer
    {
        public static SMDLine[] Parser(int SmdAmount, Dictionary<int, SMDLineIdx> SMDLineIdxDic, Dictionary<int, SmdBaseLine> ObjGroupInfosDic, out int binFilesCount)
        {
            binFilesCount = 0;

            SMDLine[] smdLines = new SMDLine[SmdAmount];
            for (int i = 0; i < SmdAmount; i++)
            {
                SMDLine line = new SMDLine();

                line.ScaleX = 1f;
                line.ScaleY = 1f;
                line.ScaleZ = 1f;
                line.ScaleW = 1f;
                line.AngleW = 1f;
                line.PositionW = 1f;
                line.FixedFF = 0xFF;
                line.SmxID = 0xFE;
                line.TplFileID = 0;

                if (SMDLineIdxDic.ContainsKey(i))
                {
                    line.PositionX = SMDLineIdxDic[i].PositionX * CONSTs.GLOBAL_POSITION_SCALE;
                    line.PositionY = SMDLineIdxDic[i].PositionY * CONSTs.GLOBAL_POSITION_SCALE;
                    line.PositionZ = SMDLineIdxDic[i].PositionZ * CONSTs.GLOBAL_POSITION_SCALE;
                    line.ScaleX = SMDLineIdxDic[i].ScaleX;
                    line.ScaleY = SMDLineIdxDic[i].ScaleY;
                    line.ScaleZ = SMDLineIdxDic[i].ScaleZ;
                    line.AngleX = SMDLineIdxDic[i].AngleX;
                    line.AngleY = SMDLineIdxDic[i].AngleY;
                    line.AngleZ = SMDLineIdxDic[i].AngleZ;
                }

                if (ObjGroupInfosDic.ContainsKey(i))
                {
                    line.BinFileID = (byte)ObjGroupInfosDic[i].BinId;
                    line.SmxID = (byte)ObjGroupInfosDic[i].SmxId;
                    line.ObjectStatus = ObjGroupInfosDic[i].Type;
                }

                if (line.BinFileID >= binFilesCount)
                {
                    binFilesCount = line.BinFileID + 1;
                }

                smdLines[i] = line;
            }

            return smdLines;
        }

        public static void SetTplFileIDInSmdLine(ref SMDLine[] SmdLines, out int tplFilesCount, Dictionary<int, SMDLineIdxPart2> SmdLineIdxPart2Dic)
        {
            tplFilesCount = 1;

            for (int i = 0; i < SmdLines.Length; i++)
            {
                if (SmdLineIdxPart2Dic.ContainsKey(i))
                {
                    SmdLines[i].TplFileID = SmdLineIdxPart2Dic[i].TplFileID;
                }

                if (SmdLines[i].TplFileID >= tplFilesCount)
                {
                    tplFilesCount = SmdLines[i].TplFileID + 1;
                }
            }

        }

        public static SMDLine[] ParserWithPart2(int smdLinesCount, Dictionary<int, SMDLineIdx> SmdLines, Dictionary<int, SMDLineIdxPart2> SmdLinesPart2, out int binFilesCount, out int tplFilesCount)
        {
            binFilesCount = 0;
            tplFilesCount = 1; // tem que ter no minimo 1;

            SMDLine[] smdLines = new SMDLine[smdLinesCount];
            for (int i = 0; i < smdLinesCount; i++)
            {
                SMDLine line = new SMDLine();

                line.ScaleX = 1f;
                line.ScaleY = 1f;
                line.ScaleZ = 1f;
                line.ScaleW = 1f;
                line.PositionW = 1f;
                line.AngleW = 1;
                line.FixedFF = 0xFF;
                line.SmxID = 0xFE;

                if (SmdLines.ContainsKey(i))
                {
                    line.PositionX = SmdLines[i].PositionX * CONSTs.GLOBAL_POSITION_SCALE;
                    line.PositionY = SmdLines[i].PositionY * CONSTs.GLOBAL_POSITION_SCALE;
                    line.PositionZ = SmdLines[i].PositionZ * CONSTs.GLOBAL_POSITION_SCALE;
                    line.ScaleX = SmdLines[i].ScaleX;
                    line.ScaleY = SmdLines[i].ScaleY;
                    line.ScaleZ = SmdLines[i].ScaleZ;
                    line.AngleX = SmdLines[i].AngleX;
                    line.AngleY = SmdLines[i].AngleY;
                    line.AngleZ = SmdLines[i].AngleZ;
                }

                if (SmdLinesPart2.ContainsKey(i))
                {
                    line.AngleW = SmdLinesPart2[i].AngleW;
                    line.PositionW = SmdLinesPart2[i].PositionW;
                    line.ScaleW = SmdLinesPart2[i].ScaleW;
                    line.BinFileID = SmdLinesPart2[i].BinFileID;
                    line.TplFileID = SmdLinesPart2[i].TplFileID;
                    line.FixedFF = SmdLinesPart2[i].FixedFF;
                    line.SmxID = SmdLinesPart2[i].SmxID;
                    line.Unused1 = SmdLinesPart2[i].Unused1;
                    line.Unused2 = SmdLinesPart2[i].Unused2;
                    line.ObjectStatus = SmdLinesPart2[i].ObjectStatus;
                }

                if (line.BinFileID >= binFilesCount)
                {
                    binFilesCount = line.BinFileID + 1;
                }

                if (line.TplFileID >= tplFilesCount)
                {
                    tplFilesCount = line.TplFileID + 1;
                }

                smdLines[i] = line;
            }

            return smdLines;
        }


    }

}
