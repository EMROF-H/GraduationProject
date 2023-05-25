using UpperComputer.Basic;
using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;

namespace UpperComputer.Communication
{
    public class AudioSerialPort
    {
        public const byte StartFlag1 = (byte)'H';
        public const byte StartFlag2 = (byte)'I';
        public const byte EndFlag = (byte)'T';

        public const int BaudRate = 5000000;
        public const int SerialCount = 0x2000;

        public readonly SerialPort port;
        private readonly Buffer buffer;
        private readonly PackageUnit[] datas;

        public int Channel { get; private set; }
        public int CurrentSerial { get; private set; }
        public bool IsOpen => port.IsOpen;

        public string PortName
        {
            get => port.PortName;
            set => port.PortName = value;
        }

        public AudioSerialPort()
        {
            port = new()
            {
                BaudRate = BaudRate,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One,
                ReadBufferSize = 0x10000
            };
            /*
            buffer = new((buffer, offset, count) =>
            {
                if (port.IsOpen)
                {
                    try
                    {
                        int a = port.BytesToRead;
                        SpinWait.SpinUntil(() => port.BytesToRead >= buffer.Length);
                        port.Read(buffer, offset, buffer.Length - offset);
                        return true;
                    }
                    catch { }
                }
                return false;
            }, 0x100);
            */
            buffer = new(port.Read, 0x10000);
            datas = new PackageUnit[SerialCount];
            Channel = -1;
            CurrentSerial = -1;

            Task.Run(Receive);
        }

        public PackageUnit Read(int serial)
        {
            return datas[serial];
        }

        private void Receive()
        {
            Span<byte> data = stackalloc byte[8];
            while (true)
            {
                if (!IsOpen)
                {
                    Thread.Sleep(1); 
                    continue;
                }
                if (buffer.ReadByte() != StartFlag1) continue;
                if (buffer.ReadByte() != StartFlag2) continue;
                data = buffer.Read(data);
                if (buffer.ReadByte() != EndFlag) continue;
                Process(data);
            }
        }
        public static AutoResetEvent SyncEvent = new(false);
        public static long OverallSerial = 0;
        public static int? InitSerial { get; set; } = null;
        public static int[] ChannelsSerial = new int[8];
        private void Process(Span<byte> data)
        {
            CurrentSerial = (data[6] << 5) | (data[7] >> 3);
            Channel = data[7] & 0b111;

            datas[CurrentSerial] = new()
            {
                Data0 = GetAudioData(data, 0),
                Data1 = GetAudioData(data, 3)
            };
            ChannelsSerial[Channel]++;
            if (Channel == 7)
            {
                Interlocked.Increment(ref OverallSerial);
                if (InitSerial == null)
                    InitSerial = CurrentSerial;
                SyncEvent.Set();
            }
        }

        private static double GetAudioData(Span<byte> data, int offset)
        {
            int result = 0;
            unsafe
            {
                byte* p = (byte*)&result;
                for (int i = 0; i < 3; i++)
                {
                    *p = data[offset + i].Reverse();
                    p++;
                }
                if ((data[offset + 2] & 0x01) == 0) *p = 0x00;
                else *p = 0xFF;
            }
            return result;
        }

        public void Open() => port.Open();
    }
}
