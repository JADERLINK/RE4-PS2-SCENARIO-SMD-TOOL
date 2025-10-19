using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SHARED_PS2_SCENARIO_SMD.SCENARIO_EXTRACT
{
    public class Extract_TPL_Inside_SMD_PS2
    {
        //Action<Stream fileStream, long tplOffset, long endOffset, int TplID>
        public Action<Stream, long, long, int> ToFileTpl;

        //Func<int TplID, Stream fileStream, long tplOffset, long endOffset>
        public Func<int, Stream, long, long> ToExtractTpl;

        public void ExtractTPLs(Stream fileStream, uint offsetTplArr, int TplFilesCount)
        {
            BinaryReader br = new BinaryReader(fileStream);
            br.BaseStream.Position = offsetTplArr;

            (uint startOffset, uint endOffset)[] TplArr = new (uint startOffset, uint endOffset)[TplFilesCount];

            for (int i = 0; i < TplFilesCount; i++)
            {
                uint offset = br.ReadUInt32();
                if (offset != 0 && offset + offsetTplArr < fileStream.Length)
                {
                    TplArr[i].startOffset = offset + offsetTplArr;
                }
            }

            var Orderned = TplArr.Select(a => a.startOffset).ToList();
            Orderned.Add((uint)fileStream.Length);
            Orderned = Orderned.OrderByDescending(a => a).ToList();

            for (int i = 0; i < TplFilesCount; i++)
            {
                uint endOffset = Orderned.First();

                foreach (var item in Orderned)
                {
                    if (item > TplArr[i].startOffset)
                    {
                        endOffset = item;
                    }
                }

                TplArr[i].endOffset = endOffset;
            }

            for (int i = 0; i < TplFilesCount; i++)
            {
                ToExtractTpl?.Invoke(i, fileStream, TplArr[i].startOffset);
                ToFileTpl?.Invoke(fileStream, TplArr[i].startOffset, TplArr[i].endOffset, i);
            }

        }
    }
}
