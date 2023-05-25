using DigitalSignalProcess.Basic;
using System;
using System.Collections.Generic;

namespace UpperComputer.Display.OxyPlot
{
    public class PolarCoordinatesHeatmap : IDisplayer
    {
        private readonly DataModel model;
        private readonly double[,] data;

        public PolarCoordinatesHeatmap(DataModel model)
        {
            this.model = model;
            data = new double[360, 90];
        }

        public void Display(Func<double, Direction, double> calculate, double frequency)
        {
            for (int θ = 0; θ < 360; θ++)
            {
                for (int ρ = 0; ρ < 90; ρ++)
                {
                    data[θ, ρ] = calculate(frequency, new()
                    {
                        Azimuth = θ * Math.Tau / 360,
                        Pitch = ρ * Math.Tau / 360
                    });
                }
            }
            model.Update(data, FindLocalMaxima(data));
        }

        public static List<(int, int, double)> FindLocalMaxima(double[,] data)
        {
            int rows = data.GetLength(0);
            int cols = data.GetLength(1);
            List<(int, int, double)> maxima = new ();

            for (int i = 1; i < rows - 1; i++)
            {
                for (int j = 1; j < cols - 1; j++)
                {
                    double value = data[i, j];
                    if (value > data[i - 1, j - 1] &&
                        value > data[i - 1, j] &&
                        value > data[i - 1, j + 1] &&
                        value > data[i, j - 1] &&
                        value > data[i, j + 1] &&
                        value > data[i + 1, j - 1] &&
                        value > data[i + 1, j] &&
                        value > data[i + 1, j + 1])
                    {
                        maxima.Add((i, j, value));
                    }
                }
            }

            //maxima = FilterOutPoints(maxima);
            return maxima;
        }

        static List<(int X, int Y, double)> FilterOutPoints(List<(int X, int Y, double)> list)
        {
            var range = 5;
            List<(int X, int Y, double)> mergedPoints = new ();
            
            mergedPoints.Add(list[0]);
            for (int i = 1; i < list.Count; i++)
            {
                var prevPoint = mergedPoints[mergedPoints.Count - 1];
                var currPoint = list[i];

                // 如果当前点与前一个点重叠，则合并它们
                if (currPoint.X - prevPoint.X <= range && currPoint.Y - prevPoint.Y <= range)
                {
                    mergedPoints[mergedPoints.Count - 1] = ((prevPoint.X + currPoint.X) / 2, (prevPoint.Y + currPoint.Y) / 2, (prevPoint.Item3 + currPoint.Item3) / 2);
                }
                else
                {
                    mergedPoints.Add(currPoint);
                }
            }

            return mergedPoints;
        }
        
    }
}
