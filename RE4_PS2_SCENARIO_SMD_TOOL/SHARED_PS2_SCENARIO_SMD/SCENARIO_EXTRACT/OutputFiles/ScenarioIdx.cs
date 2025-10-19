using SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT;
using SHARED_PS2_BIN.ALL;
using SHARED_TOOLS.ALL;
using SHARED_TOOLS.SCENARIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT.OutputFiles
{
    public static class ScenarioIdx
    {
        public static void CreateIdxScenario(string idxFullName, SMDLine[] smdLines, Dictionary<int, BoundingBox> boxes, 
            ushort smdMagic, string binFolder, string SmdFileName, string TplFileName)
        {
            TextWriter text = new FileInfo(idxFullName).CreateText();
            text.WriteLine(SHARED_TOOLS.Shared.HeaderText());
            text.WriteLine("");
            text.WriteLine("");

            if (smdMagic != 0x0040)
            {
                text.WriteLine("Magic:" + smdMagic.ToString("X4"));
            }

            text.WriteLine("SmdFileName:" + SmdFileName);
            text.WriteLine("TplFileName:" + TplFileName);
            text.WriteLine("BinFolder:" + binFolder);
            text.WriteLine("UseIdxMaterial:false");
            text.WriteLine("AutoCalcBoundingBox:true");
            text.WriteLine("IgnoreBoundingBox:false");

            text.WriteLine("");
            text.WriteLine("");

            for (int i = 0; i < smdLines.Length; i++)
            {
                text.WriteLine("SMD_" + i.ToString("D3"));
                CreateIdxScenario_Parts(ref text, smdLines[i]);
                if (boxes.ContainsKey(smdLines[i].BinFileID))
                {
                    CreateIdxScenario_RenderLimitBox(ref text, smdLines[i], boxes[smdLines[i].BinFileID]);
                }
                else 
                {
                    CreateIdxScenario_RenderLimitBoxEmpty(ref text);
                }
                CreateIdxScenario_TplFileID(ref text, smdLines[i]);
                text.WriteLine("");
                text.WriteLine("");
            }

            text.Close();
        }

        private static void CreateIdxScenario_Parts(ref TextWriter text, SMDLine smdLine)
        {
            string positionX = (smdLine.PositionX / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString();
            string positionY = (smdLine.PositionY / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString();
            string positionZ = (smdLine.PositionZ / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString();
            text.WriteLine("PositionX:" + positionX);
            text.WriteLine("PositionY:" + positionY);
            text.WriteLine("PositionZ:" + positionZ);

            string angleX = (smdLine.AngleX).ToFloatString();
            string angleY = (smdLine.AngleY).ToFloatString();
            string angleZ = (smdLine.AngleZ).ToFloatString();
            text.WriteLine("AngleX:" + angleX);
            text.WriteLine("AngleY:" + angleY);
            text.WriteLine("AngleZ:" + angleZ);

            string scaleX = (smdLine.ScaleX).ToFloatString();
            string scaleY = (smdLine.ScaleY).ToFloatString();
            string scaleZ = (smdLine.ScaleZ).ToFloatString();
            text.WriteLine("ScaleX:" + scaleX);
            text.WriteLine("ScaleY:" + scaleY);
            text.WriteLine("ScaleZ:" + scaleZ);
        }

        private static void CreateIdxScenario_RenderLimitBox(ref TextWriter text, SMDLine smdLine, BoundingBox box)
        {
            float[] pos1 = new float[3];// 0 = x, 1 = y, 2 = z
            pos1[0] = box.BoundingBoxPosX - box.BoundingBoxWidth;
            pos1[1] = box.BoundingBoxPosY - box.BoundingBoxHeight;
            pos1[2] = box.BoundingBoxPosZ - box.BoundingBoxDepth;

            pos1 = RotationUtils.RotationInX(pos1, smdLine.AngleX);
            pos1 = RotationUtils.RotationInY(pos1, smdLine.AngleY);
            pos1 = RotationUtils.RotationInZ(pos1, smdLine.AngleZ);

            pos1[0] = ((pos1[0] * smdLine.ScaleX) + (smdLine.PositionX)) / CONSTs.GLOBAL_POSITION_SCALE;
            pos1[1] = ((pos1[1] * smdLine.ScaleY) + (smdLine.PositionY)) / CONSTs.GLOBAL_POSITION_SCALE;
            pos1[2] = ((pos1[2] * smdLine.ScaleZ) + (smdLine.PositionZ)) / CONSTs.GLOBAL_POSITION_SCALE;


            float[] pos2 = new float[3];// 0 = x, 1 = y, 2 = z
            pos2[0] = box.BoundingBoxPosX + box.BoundingBoxWidth;
            pos2[1] = box.BoundingBoxPosY + box.BoundingBoxHeight;
            pos2[2] = box.BoundingBoxPosZ + box.BoundingBoxDepth;

            pos2 = RotationUtils.RotationInX(pos2, smdLine.AngleX);
            pos2 = RotationUtils.RotationInY(pos2, smdLine.AngleY);
            pos2 = RotationUtils.RotationInZ(pos2, smdLine.AngleZ);

            pos2[0] = ((pos2[0] * smdLine.ScaleX) + (smdLine.PositionX)) / CONSTs.GLOBAL_POSITION_SCALE;
            pos2[1] = ((pos2[1] * smdLine.ScaleY) + (smdLine.PositionY)) / CONSTs.GLOBAL_POSITION_SCALE;
            pos2[2] = ((pos2[2] * smdLine.ScaleZ) + (smdLine.PositionZ)) / CONSTs.GLOBAL_POSITION_SCALE;

            string BBoxMinX = (pos1[0]).ToFloatString();
            string BBoxMinY = (pos1[1]).ToFloatString();
            string BBoxMinZ = (pos1[2]).ToFloatString();

            string BBoxMaxX = (pos2[0]).ToFloatString();
            string BBoxMaxY = (pos2[1]).ToFloatString();
            string BBoxMaxZ = (pos2[2]).ToFloatString();

            text.WriteLine("BBoxMinX:" + BBoxMinX);
            text.WriteLine("BBoxMinY:" + BBoxMinY);
            text.WriteLine("BBoxMinZ:" + BBoxMinZ);
            text.WriteLine("BBoxMaxX:" + BBoxMaxX);
            text.WriteLine("BBoxMaxY:" + BBoxMaxY);
            text.WriteLine("BBoxMaxZ:" + BBoxMaxZ);
        }

        private static void CreateIdxScenario_RenderLimitBoxEmpty(ref TextWriter text) 
        {
            text.WriteLine("BBoxMinX:0.0");
            text.WriteLine("BBoxMinY:0.0");
            text.WriteLine("BBoxMinZ:0.0");
            text.WriteLine("BBoxMaxX:0.0");
            text.WriteLine("BBoxMaxY:0.0");
            text.WriteLine("BBoxMaxZ:0.0");
        }

        private static void CreateIdxScenario_TplFileID(ref TextWriter text, SMDLine smdLine)
        {
            if (smdLine.TplFileID != 0)
            {
                text.WriteLine("TplFileID:" + smdLine.TplFileID);
            }
        }

        public static void CreateIdxSmd(string idxFullName, SMDLine[] smdLines, ushort smdMagic, string binFolder, string SmdFileName, string TplFileName)
        {
            TextWriter text = new FileInfo(idxFullName).CreateText();
            text.WriteLine(SHARED_TOOLS.Shared.HeaderText());
            text.WriteLine("");
            text.WriteLine("");

            if (smdMagic != 0x0040)
            {
                text.WriteLine("Magic:" + smdMagic.ToString("X4"));
            }

            text.WriteLine("SmdFileName:" + SmdFileName);
            text.WriteLine("TplFileName:" + TplFileName);
            text.WriteLine("BinFolder:" + binFolder);

            text.WriteLine("");
            text.WriteLine("");

            for (int i = 0; i < smdLines.Length; i++)
            {
                text.WriteLine("SMD_" + i.ToString("D3"));
                CreateIdxScenario_Parts(ref text, smdLines[i]);
                CreateIdxuhdSmd_Parts(ref text, smdLines[i]);

                text.WriteLine("");
                text.WriteLine("");
            }

            text.Close();

        }

        private static void CreateIdxuhdSmd_Parts(ref TextWriter text, SMDLine smdLine)
        {
            if (smdLine.PositionW != 1)
            {
                text.WriteLine("PositionW:" + smdLine.PositionW.ToFloatString());
            }

            if (smdLine.AngleW != 1)
            {
                text.WriteLine("AngleW:" + smdLine.AngleW.ToFloatString());
            }

            if (smdLine.ScaleW != 1)
            {
                text.WriteLine("ScaleW:" + smdLine.ScaleW.ToFloatString());
            }

            text.WriteLine("BinFileID:" + smdLine.BinFileID);
            text.WriteLine("TplFileID:" + smdLine.TplFileID);
            text.WriteLine("SmxID:" + smdLine.SmxID);

            if (smdLine.FixedFF != 0xFF)
            {
                text.WriteLine("FixedFF:" + smdLine.FixedFF.ToString("X2"));
            }

            text.WriteLine("ObjectStatus:" + smdLine.ObjectStatus.ToString("X2"));

            if (smdLine.Unused1 != 0)
            {
                text.WriteLine("Unused1:" + smdLine.Unused1.ToString("X8"));
            }

            if (smdLine.Unused2 != 0)
            {
                text.WriteLine("Unused2:" + smdLine.Unused2.ToString("X8"));
            }

        }

    }
}
