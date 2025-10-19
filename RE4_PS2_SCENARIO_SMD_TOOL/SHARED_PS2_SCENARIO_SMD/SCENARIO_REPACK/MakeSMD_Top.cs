using SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK
{
    public static class MakeSMD_Top
    {
        public static void FillTopSmd(BinaryWriter bw, ushort smdMagic, SMDLine[] SmdLines, long startOffset, out long EndOffset)
        {
            bw.BaseStream.Position = startOffset;

            bw.Write((ushort)smdMagic);
            bw.Write((ushort)SmdLines.Length);

            bw.BaseStream.Position = startOffset + 0x10;

            //-------------------------
            //SmdLines

            for (int i = 0; i < SmdLines.Length; i++)
            {
                float positionX = SmdLines[i].PositionX;
                float positionY = SmdLines[i].PositionY;
                float positionZ = SmdLines[i].PositionZ;
                float positionW = SmdLines[i].PositionW;

                float angleX = SmdLines[i].AngleX;
                float angleY = SmdLines[i].AngleY;
                float angleZ = SmdLines[i].AngleZ;
                float angleW = SmdLines[i].AngleW;

                float scaleX = SmdLines[i].ScaleX;
                float scaleY = SmdLines[i].ScaleY;
                float scaleZ = SmdLines[i].ScaleZ;
                float scaleW = SmdLines[i].ScaleW;

                byte BinFileID = SmdLines[i].BinFileID;
                byte TplFileID = SmdLines[i].TplFileID;
                byte FixedFF = SmdLines[i].FixedFF;
                byte SmxID = SmdLines[i].SmxID;
                uint unused1 = SmdLines[i].Unused1;
                uint unused2 = SmdLines[i].Unused2;
                uint objectStatus = SmdLines[i].ObjectStatus;

                byte[] SMDLine = new byte[64];

                BitConverter.GetBytes(positionX).CopyTo(SMDLine, 0);
                BitConverter.GetBytes(positionY).CopyTo(SMDLine, 4);
                BitConverter.GetBytes(positionZ).CopyTo(SMDLine, 8);
                BitConverter.GetBytes(positionW).CopyTo(SMDLine, 12);
                BitConverter.GetBytes(angleX).CopyTo(SMDLine, 16);
                BitConverter.GetBytes(angleY).CopyTo(SMDLine, 20);
                BitConverter.GetBytes(angleZ).CopyTo(SMDLine, 24);
                BitConverter.GetBytes(angleW).CopyTo(SMDLine, 28);
                BitConverter.GetBytes(scaleX).CopyTo(SMDLine, 32);
                BitConverter.GetBytes(scaleY).CopyTo(SMDLine, 36);
                BitConverter.GetBytes(scaleZ).CopyTo(SMDLine, 40);
                BitConverter.GetBytes(scaleW).CopyTo(SMDLine, 44);
                SMDLine[48] = BinFileID;
                SMDLine[49] = TplFileID;
                SMDLine[50] = FixedFF;
                SMDLine[51] = SmxID;
                BitConverter.GetBytes(unused1).CopyTo(SMDLine, 52);
                BitConverter.GetBytes(objectStatus).CopyTo(SMDLine, 56);
                BitConverter.GetBytes(unused2).CopyTo(SMDLine, 60);

                bw.Write(SMDLine, 0, 64);
            }

            EndOffset = bw.BaseStream.Position;
        }

    }
}
