using System.Windows.Controls;
using System;
using static System.Math;
using DigitalSignalProcess.Basic;

namespace UpperComputer.Display.Heatmap
{
    public class PolarCoordinatesHeatmap : Heatmap
    {
        public readonly double Radius;
        public readonly double PitchMax;

        public PolarCoordinatesHeatmap(UIElementCollection father, double radius, double pitchMax = 90) : base(father, (int)(2 * radius), (int)(2 * radius))
        {
            Radius = radius;
            PitchMax = pitchMax;
        }

        public override void Display(Func<double, Direction, double> calculate, double frequency)
        {
            double[,] data = new double[Width, Height];
            double max = double.MinValue;
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    double x = +((Width  / 2) - j - 0.5);
                    double y = -((Height / 2) - i - 0.5);
                    double ρ = Sqrt(Pow(x, 2) + Pow(y, 2));
                    double θ = Atan2(y, x);
                    double azimuth = θ;
                    double pitch = ρ / Radius * PitchMax * Tau / 360;
                    Direction direction = new()
                    {
                        Azimuth = azimuth,
                        Pitch = pitch
                    };
                    if (ρ > Radius)
                    {
                        data[i, j] = -1;
                    }
                    else
                    {
                        data[i, j] = calculate(frequency, direction);
                    }
                    if (data[i, j] > max) max = data[i, j];
                }
            }
            DisplayImage(data, max);
        }
    }
}
