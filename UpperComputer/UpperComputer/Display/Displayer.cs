using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DigitalSignalProcess.Basic;
using OxyPlot.Wpf;

namespace UpperComputer.Display
{   
    public abstract class Displayer : IDisplayer
    {
        private readonly UIElementCollection father;
        private readonly Image image;
        private readonly WriteableBitmap bitmap;
        private readonly byte[] buffer;

        public readonly int Width;
        public readonly int Height;
        private int Length;
        PlotView _plotView;

        protected Displayer(UIElementCollection father, int width, int height)
        {
            this.father = father;
            Width = width;
            Height = height;
            bitmap = new(width, height, 96, 96, PixelFormats.Gray8, null);
            image = new Image()
            {
                Width = 2 * width,
                Height = 2 * height,
                Source = bitmap
            };
            Length = Width * Height;
            buffer = new byte[Length];
            //ather.Add(image);
        }

        public abstract void Display(Func<double, Direction, double> calculate, double frequency);

        public void Remove() => father.Remove(image);

        protected void DisplayImage(double[,] imageData, double max)
        {
            for (int row = 0; row < Height; row++)
            {
                for (int column = 0; column < Width; column++)
                {
                    if (imageData[row, column] < 0)
                    {
                        buffer[row * Width + column] = 0xFF;
                    }
                    else
                    {
                        buffer[row * Width + column] = (byte)(byte.MaxValue * (imageData[row, column] / max));
                    }
                }
            }

            MainWindow.MainDispatcher.Invoke(() =>
            {
                bitmap.WritePixels
                (
                    new Int32Rect(0, 0, Width, Height),
                    buffer,
                    bitmap.BackBufferStride,
                    0
                );
            });
        }
    }
}
