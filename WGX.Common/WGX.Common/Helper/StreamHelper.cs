using System;
using System.IO;
using System.Linq;

namespace WGX.Common.Helper
{
    public static class StreamHelper
    {
        public static byte[] GetBytes(this Stream stm, int perCount = 1024)
        {
            if (stm == null)
                throw new ArgumentNullException("stm");
            if (perCount <= 0)
                throw new ArgumentOutOfRangeException("perCount", "perCount 必须大于等于0");

            if (stm.CanSeek)
                stm.Position = 0;

            var bytes = new byte[stm.Length];
            var offset = 0;
            int count;
            while (0 != (count = stm.Read(bytes, offset, stm.Length - stm.Position > perCount ? perCount : (int)(stm.Length - stm.Position))))
            {
                offset += count;
            }

            return bytes;
        }

        private static int Chunk = 256;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stm"></param>
        /// <param name="readCallBack"></param>
        /// <param name="complete"></param>
        public static void AsyncGetBytes(this Stream stm, Action<byte[]> readCallBack, Action complete)
        {
            var buffer = new byte[Chunk];
            stm.BeginRead(buffer, 0, Chunk, ReadAsyncCallback, new { Stream = stm, Buffer = buffer, Action = readCallBack, Complete = complete });
        }

        private static void ReadAsyncCallback(IAsyncResult ar)
        {
            var obj = ar.AsyncState as dynamic;
            var stream = obj.Stream as Stream;
            var buffer = obj.Buffer as byte[];
            var act = obj.Action as Action<byte[]>;
            var complete = obj.Complete as Action;

            if (stream != null)
            {
                var count = stream.EndRead(ar);
                if (count > 0)
                {
                    if (buffer != null)
                    {
                        act(buffer.Take(count).ToArray());
                        stream.BeginRead(buffer, 0, Chunk, ReadAsyncCallback, new { Stream = stream, Buffer = buffer, Action = act, Complete = complete });
                    }
                }
                else
                {
                    if (complete != null) complete();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stm"></param>
        /// <returns></returns>
        public static string ToBase64Url(this Stream stm)
        {
            return Convert.ToBase64String(stm.GetBytes());
        }
    }
}
