using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SMD_PS2_Repack
{
    /*
    Codigo feito por JADERLINK
    Pesquisas feitas por HardHain e JaderLink.
    https://www.youtube.com/@JADERLINK
    https://www.youtube.com/@HardRainModder

    Em desenvolvimento
    Para Pesquisas
    12-03-2023
    version: alfa.1.0.0.0
    */

    public static class SMDPS2Repack
    {
        public static void Repack(string idxPath, string smdPath, string parentDirectory) 
        {

            StreamReader idx = File.OpenText(idxPath);

            Dictionary<string, string> pair = new Dictionary<string, string>();

            List<string> lines = new List<string>();

            string endLine = "";
            while (endLine != null)
            {
                endLine = idx.ReadLine();
                lines.Add(endLine);
            }

            idx.Close();

            foreach (var item in lines)
            {
                if (item != null)
                {
                    var split = item.Split(new char[] { ':' });
                    if (split.Length >= 2)
                    {
                        string key = split[0].ToUpper().Trim();
                        if (!pair.ContainsKey(key))
                        {
                            pair.Add(key, split[1]);
                        }
                    }
                }
            }

            ushort SmdCount = 0;
            ushort BinCount = 0;
            string BinBaseName = "null";
            string TplFilePatch = "null";

            if (pair.ContainsKey("SMDCOUNT"))
            {
                try
                {
                    SmdCount = ushort.Parse(ReturnValidDecValue(pair["SMDCOUNT"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("BINCOUNT"))
            {
                try
                {
                    BinCount = ushort.Parse(ReturnValidDecValue(pair["BINCOUNT"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("BINBASENAME"))
            {
                try
                {
                    BinBaseName = pair["BINBASENAME"].Trim();
                }
                catch (Exception)
                {
                }
            }

            if (pair.ContainsKey("TPLFILEPATCH"))
            {
                try
                {
                    TplFilePatch = pair["TPLFILEPATCH"].Trim();
                }
                catch (Exception)
                {
                }
            }

            //----------

            Stream stream = new FileInfo(smdPath).Create();

            byte[] header = new byte[0x10];
            header[0] = 0x40;

            byte[] b_SmdCount = BitConverter.GetBytes(SmdCount);
            header[2] = b_SmdCount[0];
            header[3] = b_SmdCount[1];

            uint binStreamPosition = (uint)(SmdCount * 64) + 0x10;
            byte[] b_binStreamPosition = BitConverter.GetBytes(binStreamPosition);
            header[4] = b_binStreamPosition[0];
            header[5] = b_binStreamPosition[1];
            header[6] = b_binStreamPosition[2];
            header[7] = b_binStreamPosition[3];

            stream.Write(header, 0, 0x10);


            for (int i = 0; i < SmdCount; i++)
            {
                float positionX = 0;
                float positionY = 0;
                float positionZ = 0;
                float positionW = 1;
                float angleX = 0;
                float angleY = 0;
                float angleZ = 0;
                float angleW = 1;
                float scaleX = 0;
                float scaleY = 0;
                float scaleZ = 0;
                float scaleW = 1;

                ushort BinID = 0;
                byte FixedFF = 0xFF;
                byte SmxID = 0;
                uint unused1 = 0;
                uint objectStatus = 0;
                uint unused2 = 0;

                if (pair.ContainsKey(i.ToString("D3")+"_BINID"))
                {
                    try
                    {
                        BinID = ushort.Parse(ReturnValidDecValue(pair[i.ToString("D3") + "_BINID"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_SMXID"))
                {
                    try
                    {
                        SmxID = byte.Parse(ReturnValidDecValue(pair[i.ToString("D3") + "_SMXID"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_FIXEDFF"))
                {
                    try
                    {
                        FixedFF = byte.Parse(ReturnValidHexValue(pair[i.ToString("D3") + "_FIXEDFF"].Trim().ToUpperInvariant()), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_UNUSED1"))
                {
                    try
                    {
                        unused1 = uint.Parse(ReturnValidHexValue(pair[i.ToString("D3") + "_UNUSED1"].Trim().ToUpperInvariant()), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_OBJECTSTATUS"))
                {
                    try
                    {
                        objectStatus = uint.Parse(ReturnValidHexValue(pair[i.ToString("D3") + "_OBJECTSTATUS"].Trim().ToUpperInvariant()), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_UNUSED2"))
                {
                    try
                    {
                        unused2 = uint.Parse(ReturnValidHexValue(pair[i.ToString("D3") + "_UNUSED2"].Trim().ToUpperInvariant()), System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                //----

                if (pair.ContainsKey(i.ToString("D3") + "_POSITIONX"))
                {
                    try
                    {
                        positionX = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_POSITIONX"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_POSITIONY"))
                {
                    try
                    {
                        positionY = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_POSITIONY"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_POSITIONZ"))
                {
                    try
                    {
                        positionZ = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_POSITIONZ"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_POSITIONW"))
                {
                    try
                    {
                        positionW = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_POSITIONW"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_ANGLEX"))
                {
                    try
                    {
                        angleX = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_ANGLEX"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_ANGLEY"))
                {
                    try
                    {
                        angleY = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_ANGLEY"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_ANGLEZ"))
                {
                    try
                    {
                        angleZ = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_ANGLEZ"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_ANGLEW"))
                {
                    try
                    {
                        angleW = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_ANGLEW"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_SCALEX"))
                {
                    try
                    {
                        scaleX = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_SCALEX"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_SCALEY"))
                {
                    try
                    {
                        scaleY = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_SCALEY"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_SCALEZ"))
                {
                    try
                    {
                        scaleZ = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_SCALEZ"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                if (pair.ContainsKey(i.ToString("D3") + "_SCALEW"))
                {
                    try
                    {
                        scaleW = float.Parse(ReturnValidFloatValue(pair[i.ToString("D3") + "_SCALEW"].Trim()), System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                    }
                }

                //----

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
                BitConverter.GetBytes(BinID).CopyTo(SMDLine, 48);
                SMDLine[50] = FixedFF;
                SMDLine[51] = SmxID;
                BitConverter.GetBytes(unused1).CopyTo(SMDLine, 52);
                BitConverter.GetBytes(objectStatus).CopyTo(SMDLine, 56);
                BitConverter.GetBytes(unused2).CopyTo(SMDLine, 60);

                stream.Write(SMDLine, 0, 64);
            }

            // PARTE DOS ARQUIVOS BINS

            // BLOCO DOS OFFSETS

            int offsetBlockCount = BinCount * 4;
            //int CalcDiv = offsetBlockCount % 0x10;
            int CalcLines = offsetBlockCount / 0x10;
            //if (CalcDiv != 0)
            //{
                CalcLines += 1;
            //}
            offsetBlockCount = CalcLines * 0x10;

            long StartOffset = stream.Position;

            stream.Write(new byte[offsetBlockCount], 0, offsetBlockCount);

            uint firtOffset = (uint)offsetBlockCount;

            stream.Position = StartOffset;
            stream.Write(BitConverter.GetBytes(firtOffset), 0 ,4);

            stream.Position = StartOffset + firtOffset;

            //
            long tempOffset = StartOffset + firtOffset;
            uint InternalOffset = firtOffset;

            for (int i = 0; i < BinCount; i++)
            {
                byte[] fileBytes = new byte[0];

                try
                {
                    FileInfo file = new FileInfo(parentDirectory + BinBaseName + i + ".BIN");
                    if (file.Exists)
                    {
                        fileBytes = File.ReadAllBytes(file.FullName);
                    }
                    else
                    {
                        Console.WriteLine("Not Exists: " + file.FullName);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Error in: " + BinBaseName + i + ".BIN");
                }

                uint FileLength = (uint)fileBytes.Length;

                stream.Write(fileBytes, 0, fileBytes.Length);

                tempOffset = stream.Position;


                stream.Position = StartOffset + (i * 4);
                stream.Write(BitConverter.GetBytes(InternalOffset), 0, 4);

                stream.Position = tempOffset;

                InternalOffset += FileLength;
            }

            // tpl

            uint TplOffset = (uint)stream.Position;

            stream.Position = 8;
            stream.Write(BitConverter.GetBytes(TplOffset), 0, 4);
            stream.Position = TplOffset;

            byte[] Tpl_Padding = new byte[0x10];
            Tpl_Padding[0] = 0x10;
            stream.Write(Tpl_Padding, 0, 0x10);

            byte[] TplBytes = new byte[0];

            try
            {
                FileInfo file = new FileInfo(parentDirectory + TplFilePatch);
                if (file.Exists)
                {
                    TplBytes = File.ReadAllBytes(file.FullName);
                }
                else 
                {
                    Console.WriteLine("Not Exists: " + file.FullName);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error in: TPL file");
            }
            stream.Write(TplBytes, 0, TplBytes.Length);


            stream.Close();
        }



        static string ReturnValidHexValue(string cont)
        {
            string res = "";
            foreach (var c in cont)
            {
                if (char.IsDigit(c)
                    || c == 'A'
                    || c == 'B'
                    || c == 'C'
                    || c == 'D'
                    || c == 'E'
                    || c == 'F'
                    )
                {
                    res += c;
                }
            }
            return res;
        }

        static string ReturnValidDecValue(string cont)
        {
            string res = "";
            foreach (var c in cont)
            {
                if (char.IsDigit(c))
                {
                    res += c;
                }
            }
            return res;
        }

        static string ReturnValidFloatValue(string cont)
        {
            bool Dot = false;
            bool negative = false;

            string res = "";
            foreach (var c in cont)
            {
                if (negative == false && c == '-')
                {
                    res = c + res;
                    negative = true;
                }

                if (Dot == false && c == '.')
                {
                    res += c;
                    Dot = true;
                }
                if (char.IsDigit(c))
                {
                    res += c;
                }
            }
            return res;
        }

    }
}
