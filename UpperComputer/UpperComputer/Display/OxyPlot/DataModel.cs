using System;
using OxyPlot.Annotations;
using OxyPlot.Axes;
using OxyPlot;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UpperComputer.Display.Heatmap;

namespace UpperComputer.Display.OxyPlot
{
    public class DataModel : INotifyPropertyChanged
    {
        PlotModel _model;

        public DataModel() { }

        public void Update(double[,] data, List<(int X, int Y, double)> maximaPoints)
        {
            var tmp = new PlotModel { Title = "Directional measurement result", PlotType = PlotType.Polar };
            tmp.Axes.Add(new AngleAxis { StartAngle = 0, EndAngle = 360, Minimum = 0, Maximum = 360, MajorStep = 30, MinorStep = 15 });
            tmp.Axes.Add(new MagnitudeAxis { Minimum = 0, Maximum = 90, MajorStep = 30, MinorStep = 5 });
            tmp.Axes.Add(new LinearColorAxis { Position = AxisPosition.Right, Palette = OxyPalettes.Jet(256), HighColor = OxyColors.Gray, LowColor = OxyColors.Black/*, Maximum = 0.4*/});

            var series1 = new PolarHeatMapSeries() { Angle0 = 0, Angle1 = 360, Magnitude0 = 0, Magnitude1 = 90, Interpolate = true };
            series1.Data = data;
            
            //Console.WriteLine("点数: " + maximas.Count);
            foreach (var (x, y, value) in maximaPoints)
            {
                tmp.Annotations.Add(new PointAnnotation() { Text = $"{value:F2}", X = y, Y = x, /*???*/ Stroke = OxyColor.Parse("#FFFFFF"), StrokeThickness = 0.8});
            }
            
            tmp.Series.Add(series1);
            
            this.Model = tmp;
        }

        public PlotModel Model
        {
            get => _model;
            private set
            {
                if (Equals(value, _model)) return;
                _model = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
