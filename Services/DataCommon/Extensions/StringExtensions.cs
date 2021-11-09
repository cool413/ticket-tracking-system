using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Services.DataCommon.Extensions
{
    public static class StringExtensions
    {
        public static string Compress(this string source)
        {
            var buffer = Encoding.UTF8.GetBytes(source);
            using (var outStream = new MemoryStream())
            using (var zip = new GZipStream(outStream, CompressionMode.Compress))
            {
                zip.Write(buffer, 0, buffer.Length);
                zip.Close();

                return Convert.ToBase64String(outStream.ToArray());
            }
        }

        public static string Decompress(this string source)
        {
            var buffer = Convert.FromBase64String(source);

            using (var inStream = new MemoryStream(buffer))
            using (var outStream = new MemoryStream())
            using (var zip = new GZipStream(inStream, CompressionMode.Decompress))
            {
                zip.CopyTo(outStream);
                zip.Close();

                return Encoding.UTF8.GetString(outStream.ToArray());
            }
        }
    }
}
