using DigitalSignalProcess.Doa;
using DigitalSignalProcess.Doa.Subspace;
using DigitalSignalProcess.MicrophoneArrays;
using MathNet.Numerics.Data.Matlab;
using System.IO;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using UpperComputer.Communication;
using UpperComputer.Display;
using PolarCoordinatesHeatmap = UpperComputer.Display.OxyPlot.PolarCoordinatesHeatmap;

namespace UpperComputer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Dispatcher MainDispatcher;

        public const double SampleRate = 7812.5;
        public const int PointNumber = 0x500;

        private readonly ClusterPort ports;
        private readonly AudioDatas datas;

        private readonly IDoaSolver solver;
        private readonly IDisplayer displayer;

        public MainWindow()
        {
            InitializeComponent();

            MainDispatcher = Dispatcher;

            IMicrophoneArray array = new SquareArray(4, 0.04, (e, f)
                => e % 2 == 0 ? (-f / (SampleRate * 4)) : 0);

            datas = new(PointNumber, array.ElementCount);
            ports = new(datas);

            solver = new MusicSolver(array);
            displayer = new PolarCoordinatesHeatmap(DataModel);
            //displayer = new PolarCoordinatesHeatmap(MainPanel.Children, 90, 90);
            //displayer = new RectangularCoordinatesHeatmap(MainPanel.Children);

            TextBlock[] BufferCapacity = new TextBlock[]
            {
                BufferCapacity0,
                BufferCapacity1,
                BufferCapacity2,
                BufferCapacity3,
                BufferCapacity4,
                BufferCapacity5,
                BufferCapacity6,
                BufferCapacity7,
            };

            if (File.Exists("D:\\recordUp180.mat"))
            {
                solver.Solve(MatlabReader.Read<Complex>("D:\\recordUp180.mat").ToRowArrays());
                displayer.Display(solver.Calculate, 2000);
            }

            uint i = 0;
            System.Timers.Timer timer = new(500); // 1000 * PointNumber / SampleRate
            timer.Elapsed += (_, _) =>
            {
                if (ports.IsConnected)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        string s = $"{ports[i].port.BytesToRead}/{ports[i].port.ReadBufferSize} {ports[i].Channel}";
                        Dispatcher.Invoke(() => BufferCapacity[i].Text = s);
                    }

                    solver.Solve(datas.GetData());
                    displayer.Display(solver.Calculate, 2000);
                }
                MainDispatcher.Invoke(() => RefreshTimes.Text = i++.ToString());
            };
            timer.Start();
        }
    }
}
