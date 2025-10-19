using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK.WithOBJ
{
    public class SmdBaseLine
    {
        public int SmdId = -1;
        public int BinId = -1;
        public int SmxId = 0xFE;
        public uint Type = 0;
        public UseType useType = UseType.Color;
    }

    public enum UseType : byte
    {
        Color = 1,
        Normal = 0,
    }
}
