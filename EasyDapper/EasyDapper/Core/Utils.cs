using System.IO;
using System.Text;

namespace EasyDapper.Core
{
    public static class Utils
    {
        public static string ReadFromBuffer(byte[] byteArray)
        {
            var memoryStream = new MemoryStream(byteArray.Length);
            memoryStream.Write(byteArray, 0, byteArray.Length);
            var empty = string.Empty;
            if (byteArray.Length < 2048)
                return Encoding.Default.GetString(byteArray);
            var numArray = new byte[2048];
            memoryStream.Position = 0L;
            while (memoryStream.Position < memoryStream.Length)
            {
                var num = memoryStream.Read(numArray, 0, numArray.Length);
                var chars1 = new char[Encoding.Default.GetCharCount(numArray, 0, num)];
                var chars2 = Encoding.Default.GetChars(numArray, 0, num, chars1, 0);
                empty += new string(chars1, 0, chars2);
            }

            return empty;
        }
    }
}