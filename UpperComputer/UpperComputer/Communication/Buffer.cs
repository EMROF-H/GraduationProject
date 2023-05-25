using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace UpperComputer.Communication
{
    public class Buffer
    {
        public int Size => buffer.Length;
        private int number;

        private readonly byte[] buffer;
        private int p;
        static volatile bool flag = false;
        private readonly Func<byte[], int, int, int> ReadFunc;

        public Buffer(Func<byte[], int, int, int> readFunc, int size = 0x1000)
        {
            buffer = new byte[size];
            number = 0;
            p = number;
            ReadFunc = readFunc;
            lock (typeof(Buffer))
            {
                if (!flag)
                {
                    flag = true;
                    AllocConsole();
                }
            }
        }
        [DllImport("kernel32")]
        static extern bool AllocConsole();
        
        int bytes = 0;
        public byte ReadByte()
        {
            if (p == number)
            {
                while (true)
                {
                    try
                    {
                        number = ReadFunc(buffer, 0, Size);
                        Thread.Sleep(1);
                        if (number != 0) break;
                    }
                    catch { }
                }
                p = 0;
            }
            return buffer[p++];
        }

        public byte[] Read(int size)
        {
            byte[] result = new byte[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = ReadByte();
            }
            return result;
        }

        public Span<byte> Read(Span<byte> span)
        {
            /*
            if (number - p > span.Length)
            {
                // 我不知道能不能用
                buffer.AsSpan(p).Slice(0, span.Length).CopyTo(span);
                return span;
            }
            */
            for (int i = 0; i < span.Length; i++)
            {
                span[i] = ReadByte();
            }
            return span;
        }
    }
}
