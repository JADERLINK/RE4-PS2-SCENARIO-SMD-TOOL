using SHARED_PS2_BIN.ALL;
using SHARED_PS2_BIN.EXTRACT;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT
{
    public class Extract_BIN_Content_PS2
    {
        public Dictionary<int, PS2GenericModelBIN> BIN_DIC { get; private set; }
        public Dictionary<int, BoundingBox> Boxes { get; private set; }

        public Extract_BIN_Content_PS2()
        {
            BIN_DIC = new Dictionary<int, PS2GenericModelBIN>();
            Boxes = new Dictionary<int, BoundingBox>();
        }

        public long ToExtractBin(int BinID, Stream fileStream, long StartOffset)
        {
            long endOffset = StartOffset;

            if (StartOffset > 0)
            {
                try
                {
                    var bin = BINdecoder.Decode(fileStream, StartOffset, out endOffset);
                    BIN_DIC.Add(BinID, PS2BIN_To_GenericModel.Converter(bin));

                    //cria BinRenderBox
                    BoundingBox box = new BoundingBox();
                    box.BoundingBoxPosX = bin.BoundingBoxPosX;
                    box.BoundingBoxPosY = bin.BoundingBoxPosY;
                    box.BoundingBoxPosZ = bin.BoundingBoxPosZ;
                    box.BoundingBoxPosW = bin.BoundingBoxPosW;
                    box.BoundingBoxWidth = bin.BoundingBoxWidth;
                    box.BoundingBoxHeight = bin.BoundingBoxHeight;
                    box.BoundingBoxDepth = bin.BoundingBoxDepth;
                    Boxes.Add(BinID, box);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error on Read BIN in SMD: " + BinID.ToString("D3") + Environment.NewLine + ex.ToString());
                }
            }

            return endOffset;
        }

    }
}
