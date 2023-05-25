using MathNet.Numerics.IntegralTransforms;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System.Numerics;
using DigitalSignalProcess.Basic;
using DigitalSignalProcess.MicrophoneArrays;

namespace DigitalSignalProcess.Doa
{
    public abstract class DoaSolver : IDoaSolver
    {
        protected readonly IMicrophoneArray array;

        public DoaSolver(IMicrophoneArray array)
        {
            this.array = array;
        }

        public abstract void Solve(Complex[][] signal);

        public abstract double Calculate(double frequency, Direction direction);

        protected static Matrix<Complex> ToComplexSignal(Complex[][] realSignal)
        {
            int row = realSignal.Length, column = realSignal[0].Length;
            for (int i = 0; i < row; i++)
            {
                Complex[] signal = realSignal[i];
                Fourier.Forward(signal);
                for (int j = 1; j < column / 2; j++)
                {
                    signal[j] *= 2;
                    signal[j + column / 2] = 0;
                }
                Fourier.Inverse(signal);
            }
            Matrix<Complex> result = new DenseMatrix(row, column);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    result[i, j] = realSignal[i][j];
                }
            }
            return result;
        }

        protected static Matrix<Complex> Autocorrelation(Matrix<Complex> signal)
            => signal.ConjugateTransposeAndMultiply(signal) / signal.ColumnCount;
    }
}
