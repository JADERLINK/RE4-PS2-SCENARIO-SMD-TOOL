using System;

namespace SHARED_TOOLS
{
    public static class Shared
    {
        private const string VERSION = "V.1.3.0 (2025-10-19)";

        public static string HeaderText()
        {
            return "# github.com/JADERLINK/RE4-PS2-SCENARIO-SMD-TOOL" + Environment.NewLine +
                   "# youtube.com/@JADERLINK" + Environment.NewLine +
                   "# RE4_PS2_SCENARIO_SMD_TOOL by: JADERLINK" + Environment.NewLine +
                   "# Thanks to \"HardRain\"" + Environment.NewLine +
                   "# Material information by \"Albert\"" + Environment.NewLine +
                  $"# Version {VERSION}";
        }

    }
}
