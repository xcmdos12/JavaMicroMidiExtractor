using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaMobileGamesMidiExtractor
{
    public class MidiExtractor
    {
        static string head = "MThd";
        public static int ExtractFromFile(string name, string outputfolder)
        {

            List<int> offsets = new List<int>();
            byte[] file = File.ReadAllBytes(name);
            for (int i = 0; i < file.Length - 4; i++)
            {
                try
                {
                    byte[] buffer = { file[i], file[i + 1], file[i + 2], file[i + 3] };
                    string s = Encoding.ASCII.GetString(buffer);
                    if (s == head)
                    {
                        Debug.WriteLine(s + i.ToString());
                        offsets.Add(i);
                    }
                }
                catch
                { }
            }
            using (BinaryReader r = new BinaryReader(new MemoryStream(file)))
            {
                foreach (int i in offsets)
                {
                    try
                    {
                        r.BaseStream.Position = i;
                        char[] id = r.ReadChars(4);
                        byte[] header_length = r.ReadBytes(4);
                        byte[] data = r.ReadBytes(6);
                        if (BitConverter.IsLittleEndian)
                        {
                            Array.Reverse(header_length);
                            Array.Reverse(data);
                        }
                        uint h = BitConverter.ToUInt32(header_length, 0);
                        ushort d = BitConverter.ToUInt16(new byte[] { data[0], data[1] }, 0);
                        ushort t = BitConverter.ToUInt16(new byte[] { data[2], data[3] }, 0);
                        ushort f = BitConverter.ToUInt16(new byte[] { data[4], data[5] }, 0);

                        for (ushort track = 0; track < t; track++)
                        {
                            char[] th = r.ReadChars(4);
                            byte[] track_length = r.ReadBytes(4);
                            if (BitConverter.IsLittleEndian) Array.Reverse(track_length);
                            byte[] track_data = r.ReadBytes(BitConverter.ToInt32(track_length, 0));
                        }
                        long pos = r.BaseStream.Position;
                        int size = (int)pos - i;
                        r.BaseStream.Position = i;
                        File.WriteAllBytes(GetOutputFileName(name, outputfolder), r.ReadBytes(size));
                    }
                    catch
                    { 
                        
                    }
                }
            }
            return offsets.Count;
        }

        private static string GetOutputFileName(string sourcefile, string outputfolder, string start = "00000001")
        {
            int i = Convert.ToInt32(start);
            string ofile = outputfolder + "\\" + Path.GetFileNameWithoutExtension(sourcefile) + "_" + start + ".mid";
            while (File.Exists(ofile))
            {
                i++;
                string result = null;
                for (int j = 0; j < 8 - i.ToString().Length; j++)
                {
                    result += "0";
                }
                start = result + i.ToString();
                ofile = outputfolder + "\\" + Path.GetFileNameWithoutExtension(sourcefile) + "_" + start + ".mid";
            }
            return ofile;
        }
    }
}
