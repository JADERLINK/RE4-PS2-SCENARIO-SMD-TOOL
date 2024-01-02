﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RE4_PS2_SCENARIO_SMD_TOOL.SCENARIO
{
    public class SmdBaseLine
    {
        public int SmdId = -1;
        public int BinId = -1;
        public int SmxId = 0xFE;
        public uint Type = 0;
        public UseType useType = UseType.Color;
    }

    public enum UseType 
    {
        Color,
        Normal
    }
}
