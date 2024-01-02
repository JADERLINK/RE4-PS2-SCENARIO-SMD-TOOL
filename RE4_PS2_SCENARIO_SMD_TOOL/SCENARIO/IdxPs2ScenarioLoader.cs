using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RE4_PS2_BIN_TOOL.ALL;

namespace RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO
{
    public class IdxPs2ScenarioLoader
    {
        public static IdxPs2Scenario Loader(StreamReader idxFile)
        {
            Dictionary<string, string> pair = new Dictionary<string, string>();

            string line = "";
            while (line != null)
            {
                line = idxFile.ReadLine();
                if (line != null && line.Length != 0)
                {
                    var split = line.Trim().Split(new char[] { ':' });

                    if (line.TrimStart().StartsWith(":") || line.TrimStart().StartsWith("#") || line.TrimStart().StartsWith("/"))
                    {
                        continue;
                    }
                    else if (split.Length >= 2)
                    {
                        string key = split[0].ToUpper().Trim();

                        if (!pair.ContainsKey(key))
                        {
                            pair.Add(key, split[1].Trim());
                        }

                    }

                }
            }

            //----

            IdxPs2Scenario idxScenario = new IdxPs2Scenario();

            int smdAmount = 0;

            //SMDAMOUNT
            try
            {
                string value = Utils.ReturnValidDecValue(pair["SMDAMOUNT"]);
                smdAmount = int.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
            }

            //SMDFILENAME
            try
            {
                string value = pair["SMDFILENAME"].Trim();
                value = value.Replace('/', '\\')
              .Replace(":", "").Replace("*", "").Replace("\"", "").Replace("|", "")
              .Replace("<", "").Replace(">", "").Replace("?", "").Replace(" ", "_");

                value = value.Split('\\').Last();

                if (value.Length == 0)
                {
                    value = "null";
                }

                var fileinfo = new FileInfo(value);
                idxScenario.SmdFileName = fileinfo.Name.Remove(fileinfo.Name.Length - fileinfo.Extension.Length, fileinfo.Extension.Length) + ".SMD";
            }
            catch (Exception)
            {
            }

            //TPLFILE
            try
            {
                string value = pair["TPLFILE"].Trim();
                value = value.Replace('/', '\\')
              .Replace(":", "").Replace("*", "").Replace("\"", "").Replace("|", "")
              .Replace("<", "").Replace(">", "").Replace("?", "").Replace(" ", "_");

                value = value.Split('\\').Last();

                if (value.Length == 0)
                {
                    value = "null";
                }

                var fileinfo = new FileInfo(value);
                idxScenario.TplFile = fileinfo.Name.Remove(fileinfo.Name.Length - fileinfo.Extension.Length, fileinfo.Extension.Length) + ".TPL";
            }
            catch (Exception)
            {
            }

            //BinFolder
            try
            {
                string value = pair["BINFOLDER"].Trim();
                value = value.Replace('/', '\\')
              .Replace(":", "").Replace("*", "").Replace("\"", "").Replace("|", "")
              .Replace("<", "").Replace(">", "").Replace("?", "");

                value = value.Split('\\').Last();

                if (value.Length == 0)
                {
                    value = "null";
                }
                idxScenario.BinFolder = value;
            }
            catch (Exception)
            {
            }

            //BinAmount
            try
            {
                string value = Utils.ReturnValidDecValue(pair["BINAMOUNT"]);
                idxScenario.BinAmount = int.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
            }

            //UseIdxMaterial
            try
            {
                idxScenario.UseIdxMaterial = bool.Parse(pair["USEIDXMATERIAL"].ToLower());
            }
            catch (Exception)
            {
            }

            //---

            SMDLineIdx[] smdLines = new SMDLineIdx[smdAmount];
            SMDLineIdxExtras[] SmdLinesExtras = new SMDLineIdxExtras[smdAmount];
            BinRenderBox[] boxes = new BinRenderBox[smdAmount];

            for (int i = 0; i < smdAmount; i++)
            {
                #region SMDLineIdx
                string scaleXkey = i.ToString("D3") + "_SCALEX";
                string scaleYkey = i.ToString("D3") + "_SCALEY";
                string scaleZkey = i.ToString("D3") + "_SCALEZ";

                string positionXkey = i.ToString("D3") + "_POSITIONX";
                string positionYkey = i.ToString("D3") + "_POSITIONY";
                string positionZkey = i.ToString("D3") + "_POSITIONZ";

                string angleXkey = i.ToString("D3") + "_ANGLEX";
                string angleYkey = i.ToString("D3") + "_ANGLEY";
                string angleZkey = i.ToString("D3") + "_ANGLEZ";

                SMDLineIdx smdline = new SMDLineIdx();

                smdline.scaleX = GetFloat(ref pair, scaleXkey, 1f);
                smdline.scaleY = GetFloat(ref pair, scaleYkey, 1f);
                smdline.scaleZ = GetFloat(ref pair, scaleZkey, 1f);
                smdline.positionX = GetFloat(ref pair, positionXkey, 0f);
                smdline.positionY = GetFloat(ref pair, positionYkey, 0f);
                smdline.positionZ = GetFloat(ref pair, positionZkey, 0f);
                smdline.angleX = GetFloat(ref pair, angleXkey, 0f);
                smdline.angleY = GetFloat(ref pair, angleYkey, 0f);
                smdline.angleZ = GetFloat(ref pair, angleZkey, 0f);

                smdLines[i] = smdline;
                #endregion

                #region SMDLineIdxExtras
                string scaleWkey = i.ToString("D3") + "_SCALEW";
                string positionWkey = i.ToString("D3") + "_POSITIONW";
                string angleWkey = i.ToString("D3") + "_ANGLEW";

                string binIDkey = i.ToString("D3") + "_BINID";
                string fixedFFkey = i.ToString("D3") + "_FIXEDFF";
                string smdIDkey = i.ToString("D3") + "_SMXID";
                string unused1key = i.ToString("D3") + "_UNUSED1";
                string unused2key = i.ToString("D3") + "_UNUSED2";
                string objectStatuskey = i.ToString("D3") + "_OBJECTSTATUS";

                SMDLineIdxExtras extra = new SMDLineIdxExtras();

                extra.scaleW = GetFloat(ref pair, scaleWkey, 1f);
                extra.positionW = GetFloat(ref pair, positionWkey, 1f);
                extra.angleW = GetFloat(ref pair, angleWkey, 1f);

                extra.BinID = GetUshortDec(ref pair, binIDkey, 0);
                extra.SmxID = GetByteDec(ref pair, smdIDkey, 0);
                extra.FixedFF = GetByteHex(ref pair, fixedFFkey, 0xFF);
                extra.unused1 = GetUintHex(ref pair, unused1key, 0);
                extra.unused2 = GetUintHex(ref pair, unused2key, 0);
                extra.objectStatus = GetUintHex(ref pair, objectStatuskey, 0);

                SmdLinesExtras[i] = extra;
                #endregion

                #region boxes
                BinRenderBox box = new BinRenderBox();

                string key1 = i.ToString("D3") + "_DRAWDISTANCENEGATIVEX";
                string key2 = i.ToString("D3") + "_DRAWDISTANCENEGATIVEY";
                string key3 = i.ToString("D3") + "_DRAWDISTANCENEGATIVEZ";

                string key4 = i.ToString("D3") + "_DRAWDISTANCEPOSITIVEX";
                string key5 = i.ToString("D3") + "_DRAWDISTANCEPOSITIVEY";
                string key6 = i.ToString("D3") + "_DRAWDISTANCEPOSITIVEZ";

                box.DrawDistanceNegativeX = GetFloat(ref pair, key1, -327675f);
                box.DrawDistanceNegativeY = GetFloat(ref pair, key2, -327675f);
                box.DrawDistanceNegativeZ = GetFloat(ref pair, key3, -327675f);

                box.DrawDistancePositiveX = GetFloat(ref pair, key4, 655350f);
                box.DrawDistancePositiveY = GetFloat(ref pair, key5, 655350f);
                box.DrawDistancePositiveZ = GetFloat(ref pair, key6, 655350f);

                boxes[i] = box;
                #endregion
            }

            // ----

            idxScenario.SmdAmount = smdAmount;
            idxScenario.SmdLines = smdLines;
            idxScenario.SmdLinesExtras = SmdLinesExtras;
            idxScenario.BinRenderBoxes = boxes;

            //---
            idxFile.Close();


            return idxScenario;
        }

        public static float GetFloat(ref Dictionary<string, string> pair, string key, float DefaultValue)
        {
            float res = DefaultValue;

            if (pair.ContainsKey(key))
            {
                try
                {
                    string value = Utils.ReturnValidFloatValue(pair[key]);
                    res = float.Parse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    res = DefaultValue;
                }
            }

            return res;
        }

        public static ushort GetUshortDec(ref Dictionary<string, string> pair, string key, ushort DefaultValue)
        {
            ushort res = DefaultValue;

            if (pair.ContainsKey(key))
            {
                try
                {
                    string value = Utils.ReturnValidDecValue(pair[key]);
                    res = ushort.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    res = DefaultValue;
                }
            }

            return res;
        }

        public static byte GetByteDec(ref Dictionary<string, string> pair, string key, byte DefaultValue)
        {
            byte res = DefaultValue;

            if (pair.ContainsKey(key))
            {
                try
                {
                    string value = Utils.ReturnValidDecValue(pair[key]);
                    res = byte.Parse(value, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    res = DefaultValue;
                }
            }

            return res;
        }

        public static byte GetByteHex(ref Dictionary<string, string> pair, string key, byte DefaultValue)
        {
            byte res = DefaultValue;

            if (pair.ContainsKey(key))
            {
                try
                {
                    string value = Utils.ReturnValidHexValue(pair[key]);
                    res = byte.Parse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    res = DefaultValue;
                }
            }

            return res;
        }

        public static uint GetUintHex(ref Dictionary<string, string> pair, string key, uint DefaultValue)
        {
            uint res = DefaultValue;

            if (pair.ContainsKey(key))
            {
                try
                {
                    string value = Utils.ReturnValidHexValue(pair[key]);
                    res = uint.Parse(value, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                    res = DefaultValue;
                }
            }

            return res;
        }


    }



    public class IdxPs2Scenario
    {
        public int SmdAmount = 0;

        public string SmdFileName = "null.smd";

        public string BinFolder = "null";

        public string TplFile = "null.tpl";

        public SMDLineIdx[] SmdLines;

        public BinRenderBox[] BinRenderBoxes;

        // only in .idxuhdscenario
        public bool UseIdxMaterial = false;

        // only in .idxuhdsmd
        public int BinAmount = 0;
        public SMDLineIdxExtras[] SmdLinesExtras;

    }

    public class SMDLineIdx
    {
        public float positionX;
        public float positionY;
        public float positionZ;

        public float angleX;
        public float angleY;
        public float angleZ;

        public float scaleX;
        public float scaleY;
        public float scaleZ;
    }

    public class SMDLineIdxExtras
    {
        // only in .idxuhdsmd
        public float positionW;
        public float angleW;
        public float scaleW;
        public ushort BinID;
        public byte FixedFF;
        public byte SmxID;
        public uint unused1;
        public uint objectStatus;
        public uint unused2;    
    }

}

