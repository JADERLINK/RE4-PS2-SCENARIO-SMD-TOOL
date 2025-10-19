using System;
using System.Collections.Generic;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_REPACK
{
    public static class ValidateMagic
    {
        public static void Validate(uint Magic)
        {

            if (!(
                      Magic == 0x0040
                   || Magic == 0x0031
                   ))
            {
                throw new ApplicationException("The content of the 'Magic' property is invalid.");
            }

        }

    }
}
