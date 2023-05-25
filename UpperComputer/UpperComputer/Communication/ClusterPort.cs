using System;
using System.Buffers;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UpperComputer.Communication
{
    public class ClusterPort
    {
        public const int Count = 8;

        private readonly AudioSerialPort[] SerialPorts;
        private readonly AudioDatas datas;

        public AudioSerialPort this[Index index] => SerialPorts[index];

        public bool IsConnected
        {
            get
            {
                foreach (var port in SerialPorts)
                {
                    if (!port.IsOpen) return false;
                }
                return true;
            }
        }

        public ClusterPort(AudioDatas datas)
        {
            this.datas = datas;
            SerialPorts = new AudioSerialPort[Count];
            for (int i = 0; i < SerialPorts.Length; i++)
            {
                SerialPorts[i] = new();
            }

            Task.Run(MainLoop);
        }

        private void MainLoop()
        {
            Refresh();
            /*
            for (int i = 0; i < Count; i++)
            {
                SerialPorts[i].PortName = $"COM{i + 5}";
            }
            */
            int? serial = null;
            var current = 0;
            for (; ; )
            {
                if (IsConnected)
                {
                    if (Interlocked.Read(ref AudioSerialPort.OverallSerial) <= current)
                    {
                        AudioSerialPort.SyncEvent.WaitOne();
                    }

                    SpinWait.SpinUntil(() => AudioSerialPort.ChannelsSerial.All(x => x > current));

                    if (serial == null)
                    {
                        serial = AudioSerialPort.InitSerial;
                    }
                    /*
                    double[] data = new double[0x10];
                    for (int i = 0; i < Count; i++)
                    {
                        var d = SerialPorts[i].Read(serial);
                        data[i] = d.Data0;
                        data[i + 0x8] = d.Data1;
                    }
                    sequence.Add(data);
                    */
                    double[] data = ArrayPool<double>.Shared.Rent(0x10);
                    for (int i = 0; i < Count; i++)
                    {
                        var d = SerialPorts[i].Read((int)serial);
                        data[i] = d.Data0;
                        data[i + 0x8] = d.Data1;
                    }
                    datas.Add(data);
                    serial++;
                    current++;
                    serial %= AudioSerialPort.SerialCount;
                    if (Interlocked.Read(ref AudioSerialPort.OverallSerial) - current > 3000 && Random.Shared.NextDouble() < 0.01)
                    {
                        Console.WriteLine($"{Interlocked.Read(ref AudioSerialPort.OverallSerial)} / {current}");
                    }
                }
                else
                {
                    TryConnect();
                    Thread.Sleep(100);
                }
            }
        }

        private void Refresh()
        {
            string[] ports;
            do
            {
                ports = SerialPort.GetPortNames();
                Thread.Sleep(100);
            }
            while (ports.Length < Count);

            for (int i = 0; i < Count; i++)
            {
                SerialPorts[i].PortName = ports[i];
            }

            do
            {
                TryConnect();
                Thread.Sleep(100);
            }
            while (!IsConnected);

            Sort();
        }

        private void Sort()
        {
        ReRead:
            AudioSerialPort?[] ports = new AudioSerialPort?[Count];
            int[] channels = new int[Count];

            for (int i = 0; i < Count; i++)
            {
                SerialPorts[i].Read(0);
                channels[i] = SerialPorts[i].Channel;
            }
            for (int i = 0; i < Count; i++)
            {
                if (channels[i] == -1) goto ReRead;
                ports[channels[i]] = SerialPorts[i]; 
            }
            for (int i = 0; i < Count; i++)
            {
                if (ports[i] is null)
                {
                    Thread.Sleep(100);
                    goto ReRead;
                }
            }
            for (int i = 0; i < Count; i++)
            {
                SerialPorts[i] = ports[i];
            }
        }

        private void TryConnect()
        {
            foreach (var port in SerialPorts)
            {
                if (!port.IsOpen)
                {
                    try
                    {
                        port.Open();
                    }
                    catch { continue; }
                }
            }
        }
    }
}
