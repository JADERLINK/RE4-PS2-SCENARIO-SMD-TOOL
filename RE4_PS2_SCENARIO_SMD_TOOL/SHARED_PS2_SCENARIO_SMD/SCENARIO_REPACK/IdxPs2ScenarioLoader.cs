using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SHARED_TOOLS.ALL;
using System.Globalization;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK
{
    public class IdxPs2ScenarioLoader
    {
        public static IdxPs2Scenario Loader(Stream idxFile)
        {
            IdxPs2Scenario idxScenario = new IdxPs2Scenario();

            Dictionary<int, SMDLineIdx> SmdLinesDic = new Dictionary<int, SMDLineIdx>();
            Dictionary<int, SMDLineIdxPart2> SmdLinesPart2Dic = new Dictionary<int, SMDLineIdxPart2>();
            Dictionary<int, RenderLimitBox> boxes = new Dictionary<int, RenderLimitBox>();

            SMDLineIdx tempLine = new SMDLineIdx();
            SMDLineIdxPart2 tempPart2 = new SMDLineIdxPart2();
            RenderLimitBox tempBox = new RenderLimitBox();

            StreamReader reader = new StreamReader(idxFile, Encoding.ASCII);

            while (!reader.EndOfStream)
            {
                string lineCaseSensitive = reader?.ReadLine()?.Trim();
                string line = lineCaseSensitive?.ToUpperInvariant();

                if (line == null
                    || line.Length == 0
                    || line.StartsWith("\\")
                    || line.StartsWith("/")
                    || line.StartsWith("#")
                    || line.StartsWith(":")
                    || line.StartsWith("!")
                    || line.StartsWith("@")
                    || line.StartsWith("=")
                    )
                {
                    continue;
                }
                else if (line.StartsWith("SMDFILENAME"))
                {
                    var split = lineCaseSensitive.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                           string value = split[1].Replace('\\', '/')
                            .Replace(":", "").Replace("*", "").Replace("\"", "").Replace("|", "")
                            .Replace("<", "").Replace(">", "").Replace("?", "").Replace(" ", "_");

                            value = value.Split('\\').Last();

                            if (value.Length == 0)
                            {
                                value = "null";
                            }

                            idxScenario.SmdFileName = Path.GetFileNameWithoutExtension(value) + ".SMD";
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (line.StartsWith("TPLFILENAME"))
                {
                    var split = lineCaseSensitive.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            string value = split[1].Replace('\\', '/')
                             .Replace(":", "").Replace("*", "").Replace("\"", "").Replace("|", "")
                             .Replace("<", "").Replace(">", "").Replace("?", "").Replace(" ", "_");

                            value = value.Split('/').Last();

                            if (value.Length == 0)
                            {
                                value = "null";
                            }

                            idxScenario.TplFileName = Path.GetFileNameWithoutExtension(value) + ".TPL";
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (line.StartsWith("BINFOLDER"))
                {
                    var split = lineCaseSensitive.Split(':');
                    if (split.Length >= 2)
                    {
                        try
                        {
                            string value = split[1].Replace('\\', '/')
                             .Replace(":", "").Replace("*", "").Replace("\"", "").Replace("|", "")
                             .Replace("<", "").Replace(">", "").Replace("?", "").Replace(" ", "_");

                            value = value.Split('/').Last();

                            if (value.Length == 0)
                            {
                                value = "null";
                            }

                            idxScenario.BinFolder = value;
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                else if (line.StartsWith("SMD_"))
                {
                    tempLine = new SMDLineIdx();
                    tempLine.ScaleX = 1;
                    tempLine.ScaleY = 1;
                    tempLine.ScaleZ = 1;

                    tempPart2 = new SMDLineIdxPart2();
                    tempPart2.PositionW = 1;
                    tempPart2.ScaleW = 1;
                    tempPart2.AngleW = 1;
                    tempPart2.FixedFF = 0xFF;
                    tempPart2.SmxID = 0xFE;

                    tempBox = new RenderLimitBox();

                    var split = line.Split('_');
                    if (split.Length >= 2)
                    {
                        int ID = -1;
                        try
                        {
                            ID = int.Parse(Utils.ReturnValidDecValue(split[1]), NumberStyles.Integer, CultureInfo.InvariantCulture);
                        }
                        catch (Exception)
                        {
                        }

                        if (ID > -1 && ID < LimitConsts.SmdLineLimit && !SmdLinesDic.ContainsKey(ID))
                        {
                            SmdLinesDic.Add(ID, tempLine);
                            SmdLinesPart2Dic.Add(ID, tempPart2);
                            boxes.Add(ID, tempBox);
                        }
                    }
                }
                else
                {
                    _ = Utils.SetBoolean(ref line, "USEIDXMATERIAL", ref idxScenario.UseIdxMaterial)
                     || Utils.SetBoolean(ref line, "AUTOCALCBOUNDINGBOX", ref idxScenario.AutoCalcBoundingBox)
                     || Utils.SetBoolean(ref line, "IGNOREBOUNDINGBOX", ref idxScenario.IgnoreBoundingBox)
                     || Utils.SetUshortHex(ref line, "MAGIC", ref idxScenario.Magic)

                     || Utils.SetFloatDec(ref line, "POSITIONX", ref tempLine.PositionX)
                     || Utils.SetFloatDec(ref line, "POSITIONY", ref tempLine.PositionY)
                     || Utils.SetFloatDec(ref line, "POSITIONZ", ref tempLine.PositionZ)

                     || Utils.SetFloatDec(ref line, "ANGLEX", ref tempLine.AngleX)
                     || Utils.SetFloatDec(ref line, "ANGLEY", ref tempLine.AngleY)
                     || Utils.SetFloatDec(ref line, "ANGLEZ", ref tempLine.AngleZ)

                     || Utils.SetFloatDec(ref line, "SCALEX", ref tempLine.ScaleX)
                     || Utils.SetFloatDec(ref line, "SCALEY", ref tempLine.ScaleY)
                     || Utils.SetFloatDec(ref line, "SCALEZ", ref tempLine.ScaleZ)

                     || Utils.SetFloatDec(ref line, "ANGLEW", ref tempPart2.AngleW)
                     || Utils.SetFloatDec(ref line, "POSITIONW", ref tempPart2.PositionW)
                     || Utils.SetFloatDec(ref line, "SCALEW", ref tempPart2.ScaleW)

                     || Utils.SetByteDec(ref line, "BINFILEID", ref tempPart2.BinFileID)
                     || Utils.SetByteDec(ref line, "TPLFILEID", ref tempPart2.TplFileID)
                     || Utils.SetByteDec(ref line, "SMXID", ref tempPart2.SmxID)
                     || Utils.SetByteHex(ref line, "FIXEDFF", ref tempPart2.FixedFF)
                     || Utils.SetUintHex(ref line, "OBJECTSTATUS", ref tempPart2.ObjectStatus)
                     || Utils.SetUintHex(ref line, "UNUSED1", ref tempPart2.Unused1)
                     || Utils.SetUintHex(ref line, "UNUSED2", ref tempPart2.Unused2)

                     || Utils.SetFloatDec(ref line, "BBOXMINX", ref tempBox.BBoxMinX)
                     || Utils.SetFloatDec(ref line, "BBOXMINY", ref tempBox.BBoxMinY)
                     || Utils.SetFloatDec(ref line, "BBOXMINZ", ref tempBox.BBoxMinZ)
                     || Utils.SetFloatDec(ref line, "BBOXMAXX", ref tempBox.BBoxMaxX)
                     || Utils.SetFloatDec(ref line, "BBOXMAXY", ref tempBox.BBoxMaxY)
                     || Utils.SetFloatDec(ref line, "BBOXMAXZ", ref tempBox.BBoxMaxZ)
                     ;
                }

            }

            idxScenario.SmdLinesDic = SmdLinesDic;
            idxScenario.SmdLinesPart2Dic = SmdLinesPart2Dic;
            idxScenario.RenderLimitBoxes = boxes;

            idxFile.Close();

            return idxScenario;
        }

    }


    public class IdxPs2Scenario
    {
        public ushort Magic = 0x0040;

        public string SmdFileName = "null.SMD";
        public string TplFileName = "null.TPL";
        public string BinFolder = "null";

        public Dictionary<int, SMDLineIdx> SmdLinesDic;
      
        // only in .idx_ps2_scenario
        public bool UseIdxMaterial = false;
        public bool AutoCalcBoundingBox = false;
        public bool IgnoreBoundingBox = false;
        public Dictionary<int, RenderLimitBox> RenderLimitBoxes;

        // only in .idx_ps2_smd
        public Dictionary<int, SMDLineIdxPart2> SmdLinesPart2Dic;
    }

    public class SMDLineIdx
    {
        public float PositionX;
        public float PositionY;
        public float PositionZ;

        public float AngleX;
        public float AngleY;
        public float AngleZ;

        public float ScaleX;
        public float ScaleY;
        public float ScaleZ;
    }

    public class SMDLineIdxPart2
    {
        // only in .idx_ps2_smd
        public float PositionW;
        public float AngleW;
        public float ScaleW;
        public byte BinFileID;
        public byte TplFileID;
        public byte FixedFF;
        public byte SmxID;
        public uint Unused1;
        public uint ObjectStatus;
        public uint Unused2;
    }

    public class RenderLimitBox
    {
        public float BBoxMinX;
        public float BBoxMinY;
        public float BBoxMinZ;
        public float BBoxMaxX;
        public float BBoxMaxY;
        public float BBoxMaxZ;
    }

}

