using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Serialization
{
    public static class Serializer
    {
        public static void WriteString(string value, List<byte> data)
        {
            if(value == null)
            {
                WriteInt(0, data);
                return;
            }
            int length = value.Length;
            WriteInt(length, data);
            data.AddRange(Encoding.UTF8.GetBytes(value));

        }
        public static string ReadString(List<byte> data) 
        {
            int length = ReadInt(data);
            if(length == 0)
            {
                return null;
            }
            string value = Encoding.UTF8.GetString(data.ToArray(), 0, length);
            data.RemoveRange(0, length);
            return value;
        }
        public static void WriteInt(int value, List<byte> data) 
        {
            data.AddRange(BitConverter.GetBytes(value));
        }
        public static int ReadInt(List<byte> data) 
        {
            int value = BitConverter.ToInt32(data.ToArray(), 0);
            data.RemoveRange(0, sizeof(int));
            return value;
        }
        public static void WriteDouble(double value, List<byte> data)
        {
            data.AddRange(BitConverter.GetBytes(value));
        }
        public static double ReadDouble(List<byte> data) 
        {
            double value = BitConverter.ToDouble(data.ToArray(), 0);
            data.RemoveRange(0, sizeof(double));
            return value;
        }
        public static void WriteFloat(float value, List<byte> data)
        {
            data.AddRange(BitConverter.GetBytes(value));
        }
        public static float ReadFloat(List<byte> data)
        {
            float value = BitConverter.ToSingle(data.ToArray(), 0);
            data.RemoveRange(0,sizeof(float));
            return value;
        }
        public static void WriteBool(bool value, List<byte> data)
        {
            data.AddRange(BitConverter.GetBytes(value));
        }
        public static bool ReadBool(List<byte> data)
        {
            bool value = BitConverter.ToBoolean(data.ToArray(), 0);
            data.RemoveRange(0, sizeof(bool));
            return value;
        }
        public static void WriteLong(long value, List<byte> data) 
        {
            data.AddRange(BitConverter.GetBytes(value));
        }
        public static long ReadLong(List<byte> data)
        {
            long value = BitConverter.ToInt64(data.ToArray(), 0);
            data.RemoveRange(0, sizeof(long));
            return value;
        }
    }
}
