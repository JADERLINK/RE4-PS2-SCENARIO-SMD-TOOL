using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SHARED_2007PS2_SCENARIO_SMD.SCENARIO_EXTRACT
{
    public static class SmdExtract2007PS2
    {
        public static SMDLine[] Extract(Stream fileStream, out ushort smdMagic, out uint offsetBin, out uint offsetTpl) 
        {
            BinaryReader br = new BinaryReader(fileStream);

            smdMagic = br.ReadUInt16();

            // se não for um desses não é um .SMD valido;
            if (!(smdMagic == 0x0040 || smdMagic == 0x0031))
            {
                throw new ApplicationException("Invalid SMD file!!!");
            }

            ushort SmdLinesAmount = br.ReadUInt16();

            offsetBin = br.ReadUInt32(); // para offset dos bins (são relativos)
            offsetTpl = br.ReadUInt32(); // tpl offset relativo (aponta para um offset (relativo), dele que vai pro tpl)
            _ = br.ReadUInt32(); // nada

            // 64 bytes por linha smd
            SMDLine[] SMDLines = new SMDLine[SmdLinesAmount];

            for (int i = 0; i < SmdLinesAmount; i++)
            {
                SMDLine smdLine = new SMDLine();

                smdLine.PositionX = br.ReadSingle();
                smdLine.PositionY = br.ReadSingle();
                smdLine.PositionZ = br.ReadSingle();
                smdLine.PositionW = br.ReadSingle();
                smdLine.AngleX = br.ReadSingle();
                smdLine.AngleY = br.ReadSingle();
                smdLine.AngleZ = br.ReadSingle();
                smdLine.AngleW = br.ReadSingle();
                smdLine.ScaleX = br.ReadSingle();
                smdLine.ScaleY = br.ReadSingle();
                smdLine.ScaleZ = br.ReadSingle();
                smdLine.ScaleW = br.ReadSingle();

                smdLine.BinFileID = br.ReadByte();
                smdLine.TplFileID = br.ReadByte();
                smdLine.FixedFF = br.ReadByte();
                smdLine.SmxID = br.ReadByte();
                smdLine.Unused1 = br.ReadUInt32();
                smdLine.ObjectStatus = br.ReadUInt32();
                smdLine.Unused2 = br.ReadUInt32();

                SMDLines[i] = smdLine;
            }
            return SMDLines;
        }

    }

    public class SMDLine // 2007/PS2
    {
        public float PositionX;
        public float PositionY;
        public float PositionZ;
        public float PositionW;
        public float AngleX;
        public float AngleY;
        public float AngleZ;
        public float AngleW;
        public float ScaleX;
        public float ScaleY;
        public float ScaleZ;
        public float ScaleW;

        public byte BinFileID;
        public byte TplFileID;
        public byte FixedFF;
        public byte SmxID;
        public uint Unused1;
        public uint ObjectStatus;
        public uint Unused2;
    }
}
