using System;
using System.Windows.Controls;
using DigitalSignalProcess.Basic;

namespace UpperComputer.Display.Heatmap
{
    public class RectangularCoordinatesHeatmap : Heatmap
    {
        public RectangularCoordinatesHeatmap(UIElementCollection father) : base(father, 90, 360) { }

        public override void Display(Func<double, Direction, double> calculate, double frequency)
        {
            double[,] data = new double[Height, Width];
            double max = double.MinValue;
            for (int θ = 0; θ < Height; θ++)
            {
                for (int φ = 0; φ < Width; φ++)
                {
                    data[θ, φ] = calculate(frequency, new()
                    {
                        Azimuth = θ,
                        Pitch = φ
                    });
                    if (data[θ, φ] > max) max = data[θ, φ];
                }
            }
            DisplayImage(data, max);
        }
    }
}
