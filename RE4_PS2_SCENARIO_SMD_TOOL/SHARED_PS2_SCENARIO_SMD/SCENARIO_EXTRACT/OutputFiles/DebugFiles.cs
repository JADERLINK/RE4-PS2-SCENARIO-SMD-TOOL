using SHARED_PS2_BIN.ALL;
using SHARED_TOOLS.ALL;
using SHARED_TOOLS.SCENARIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT.OutputFiles
{
    public static class DebugFiles
    {
        public static void CreateSmdModelReference(string fullFilePath, SMDLine[] SMDLines)
        {
            TextWriter text = new FileInfo(fullFilePath).CreateText();
            text.WriteLine("version 1");
            text.WriteLine("nodes");

            for (int i = 0; i < SMDLines.Length; i++)
            {
                text.WriteLine(i + " \"SMD_" + i.ToString("D3") + "_BIN_" + SMDLines[i].BinFileID.ToString("D3") + "\" -1");
            }
            text.WriteLine(SMDLines.Length + " \"Center\" -1");
            text.WriteLine("end");

            text.WriteLine("skeleton");
            text.WriteLine("time 0");

            for (int i = 0; i < SMDLines.Length; i++)
            {
                text.WriteLine(i +
                   " " + (SMDLines[i].PositionX / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString() +
                   " " + (SMDLines[i].PositionZ / CONSTs.GLOBAL_POSITION_SCALE * -1).ToFloatString() +
                   " " + (SMDLines[i].PositionY / CONSTs.GLOBAL_POSITION_SCALE).ToFloatString() +
                   " " + SMDLines[i].AngleX.ToFloatString() +
                   " " + SMDLines[i].AngleZ.ToFloatString() +
                   " " + SMDLines[i].AngleY.ToFloatString()
                   );
            }
            text.WriteLine(SMDLines.Length + " 0.0 0.0 0.0 0.0 0.0 0.0"); //center

            text.WriteLine("end");

            text.WriteLine("triangles");

            for (int i = 0; i < SMDLines.Length; i++)
            {
                float[] pos1 = new float[3]; // 0 = x, 1 = y, 2 = z
                //XYZ
                pos1[0] = SMDLines[i].PositionX / CONSTs.GLOBAL_POSITION_SCALE;
                pos1[1] = SMDLines[i].PositionY / CONSTs.GLOBAL_POSITION_SCALE;
                pos1[2] = SMDLines[i].PositionZ / CONSTs.GLOBAL_POSITION_SCALE;

                //--------
                float[] pos2 = new float[3]; // 0 = x, 1 = y, 2 = z

                pos2[0] = 0;
                pos2[1] = -1000;
                pos2[2] = 1000;

                //XYZ

                pos2 = RotationUtils.RotationInX(pos2, SMDLines[i].AngleX);
                pos2 = RotationUtils.RotationInY(pos2, SMDLines[i].AngleY);
                pos2 = RotationUtils.RotationInZ(pos2, SMDLines[i].AngleZ);

                pos2[0] = ((pos2[0] * SMDLines[i].ScaleX) + SMDLines[i].PositionX) / CONSTs.GLOBAL_POSITION_SCALE;
                pos2[1] = ((pos2[1] * SMDLines[i].ScaleY) + SMDLines[i].PositionY) / CONSTs.GLOBAL_POSITION_SCALE;
                pos2[2] = ((pos2[2] * SMDLines[i].ScaleZ) + SMDLines[i].PositionZ) / CONSTs.GLOBAL_POSITION_SCALE;

                //----
                float[] pos3 = new float[3]; // 0 = x, 1 = y, 2 = z

                pos3[0] = 0;
                pos3[1] = -1000;
                pos3[2] = -1000;

                //XYZ

                pos3 = RotationUtils.RotationInX(pos3, SMDLines[i].AngleX);
                pos3 = RotationUtils.RotationInY(pos3, SMDLines[i].AngleY);
                pos3 = RotationUtils.RotationInZ(pos3, SMDLines[i].AngleZ);

                pos3[0] = ((pos3[0] * SMDLines[i].ScaleX) + SMDLines[i].PositionX) / CONSTs.GLOBAL_POSITION_SCALE;
                pos3[1] = ((pos3[1] * SMDLines[i].ScaleY) + SMDLines[i].PositionY) / CONSTs.GLOBAL_POSITION_SCALE;
                pos3[2] = ((pos3[2] * SMDLines[i].ScaleZ) + SMDLines[i].PositionZ) / CONSTs.GLOBAL_POSITION_SCALE;

                //----------

                text.WriteLine("NOMATERIAL");
                text.WriteLine(i.ToString() + " " + (pos1[0]).ToFloatString() + " " + (pos1[2] * -1).ToFloatString() + " " + (pos1[1]).ToFloatString() + " 0 0 0 0 0 0");
                text.WriteLine(i.ToString() + " " + (pos2[0]).ToFloatString() + " " + (pos2[2] * -1).ToFloatString() + " " + (pos2[1]).ToFloatString() + " 0 0 0 0 0 0");
                text.WriteLine(i.ToString() + " " + (pos3[0]).ToFloatString() + " " + (pos3[2] * -1).ToFloatString() + " " + (pos3[1]).ToFloatString() + " 0 0 0 0 0 0");
            }

            // center
            text.WriteLine("NOMATERIAL");
            text.WriteLine(SMDLines.Length + " 0 0 0 0 0 0 0 0 0");
            text.WriteLine(SMDLines.Length + " 0 10 -10 0 0 0 0 0 0");
            text.WriteLine(SMDLines.Length + " 0 -10 -10 0 0 0 0 0 0");

            text.WriteLine("end");
            text.WriteLine("// RE4_PS2_SCENARIO_SMD_TOOL by JADERLINK");
            text.WriteLine("// youtube.com/@JADERLINK");

            text.Close();
        }

        public static void CreateBoundingBoxOBJ(string fullFilePath, SMDLine[] smdLine, Dictionary<int, BoundingBox> boxes)
        {
            TextWriter text = new FileInfo(fullFilePath).CreateText();
            text.WriteLine(SHARED_TOOLS.Shared.HeaderText());
            text.WriteLine("");
            int index = 0;

            for (int i = 0; i < smdLine.Length; i++)
            {
                if (boxes.ContainsKey(smdLine[i].BinFileID))
                {
                    CreateBoundingBoxPart(i, ref text, smdLine[i], boxes[smdLine[i].BinFileID], ref index);
                }

            }

            text.Close();
        }

        private static void CreateBoundingBoxPart(int id, ref TextWriter text, SMDLine smdLine, BoundingBox box, ref int index)
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

            string NX = (pos1[0]).ToFloatString();
            string NY = (pos1[1]).ToFloatString();
            string NZ = (pos1[2]).ToFloatString();

            string PX = (pos2[0]).ToFloatString();
            string PY = (pos2[1]).ToFloatString();
            string PZ = (pos2[2]).ToFloatString();

            // pontas 1
            text.WriteLine("v " + NX + " " + NY + " " + NZ);
            text.WriteLine("v " + PX + " " + PY + " " + PZ);

            text.WriteLine("v " + PX + " " + NY + " " + PZ);
            text.WriteLine("v " + PX + " " + NY + " " + NZ);
            text.WriteLine("v " + NX + " " + NY + " " + PZ);

            text.WriteLine("v " + NX + " " + PY + " " + NZ);
            text.WriteLine("v " + NX + " " + PY + " " + PZ);
            text.WriteLine("v " + PX + " " + PY + " " + NZ);

            int i1 = index + 1;
            int i2 = index + 2;
            
            int i3 = index + 3;
            int i4 = index + 4;
            int i5 = index + 5;
            
            int i6 = index + 6;
            int i7 = index + 7;
            int i8 = index + 8;

            text.WriteLine("g " + "BoundingBoxLine_SMD_" + id.ToString("D3"));
            text.WriteLine($"l {i1} {i2}");

            text.WriteLine("g " + "BoundingBox_SMD_" + id.ToString("D3"));
            //up
            text.WriteLine($"l {i1} {i6}");
            text.WriteLine($"l {i2} {i3}");

            text.WriteLine($"l {i4} {i8}");
            text.WriteLine($"l {i5} {i7}");

            // Z
            text.WriteLine($"l {i1} {i4}");
            text.WriteLine($"l {i2} {i7}");

            text.WriteLine($"l {i6} {i8}");
            text.WriteLine($"l {i3} {i5}");

            // X
            text.WriteLine($"l {i1} {i5}");
            text.WriteLine($"l {i2} {i8}");

            text.WriteLine($"l {i6} {i7}");
            text.WriteLine($"l {i3} {i4}");

            index += 8;
        }

    }
}
