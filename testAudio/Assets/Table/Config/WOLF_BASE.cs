// 文件由脚本自动生成，请勿直接修改
using System;
using System.IO;
using System.Collections.Generic;

namespace TBL.WOLF_BASE
{
    public class Loader : ReaL.ILoader
    {
        public string GetFileName() { return "WOLF_BASE"; }

        public void ReadFile(Stream stream)
        {
            try
            {
                BinaryReader In = new BinaryReader(stream, System.Text.Encoding.Unicode);

                Data.list.Clear();
                Data.map.Clear();
                while (stream.Length > stream.Position)
                {
                    Data.Values value = new Data.Values();
                    Data.Values.Read(In, ref value);
                    Data.Add(value);
                }

                In.Close();
                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

    public class Data
    {
        public static List<Values> list = new List<Values>();
        public static Dictionary<int, Values> map = new Dictionary<int, Values>();

        public static void Add(Values item) { list.Add(item); map[item.ID] = item; }

        [Serializable]
        public class Values
        {
            public int ID = 0;
            public string WOLF_NAME = "";
            public int GRAVITY = 0;

            public static void Read(BinaryReader In, ref Values arg)
            {
                arg.ID = In.ReadInt32();
                arg.WOLF_NAME = In.ReadString();
                arg.GRAVITY = In.ReadInt32();
            }
        }
    }
}