using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System.Numerics;
using DigitalSignalProcess.Basic;
using DigitalSignalProcess.MicrophoneArrays;

namespace DigitalSignalProcess.Doa.Subspace
{
    public class MusicSolver : DoaSolver
    {
        private const double Threshold = 0.9;

        private Matrix<Complex> noiseMatrix;

        public MusicSolver(IMicrophoneArray array) : base(array)
        {
            noiseMatrix = new DiagonalMatrix(array.ElementCount, array.ElementCount);
        }

        public override void Solve(Complex[][] signal)
        {
            Matrix<Complex> complexSignal = ToComplexSignal(signal);
            Matrix<Complex> autocorrelation = Autocorrelation(complexSignal);
            Matrix<Complex> noiseSpace = SelectNoiseSpace(autocorrelation);
            noiseMatrix = noiseSpace.ConjugateTransposeAndMultiply(noiseSpace);
        }

        public override double Calculate(double frequency, Direction direction)
        {
            var a = array.GetArrayVector(frequency, direction);
            return Complex.Abs(1 / (a.ConjugateTranspose() * noiseMatrix * a)[0, 0]);
        }

        private static Matrix<Complex> SelectNoiseSpace(Matrix<Complex> autocorrelation)
        {
            Evd<Complex> evd = autocorrelation.Evd(Symmetricity.Hermitian);
            double[] eValues = evd.EigenValues.Real().Reverse().ToArray();
            Matrix<Complex> eVectors = new DenseMatrix(evd.EigenVectors.RowCount, evd.EigenVectors.ColumnCount);
            for (int row = 0; row < eVectors.RowCount; row++)
            {
                for (int column = 0; column < eVectors.ColumnCount; column++)
                {
                    eVectors[row, column] = evd.EigenVectors[row, eVectors.ColumnCount - 1 - column];
                }
            }

            int signalNumber = 0;
            for (int i = 1; i < eValues.Length; i++)
            {
                eValues[i] = (i == 0 ? 0 : eValues[i - 1]) + eValues[i];
            }
            for (int i = 0; i < eValues.Length; i++)
            {
                eValues[i] /= eValues[^1];
                if (eValues[i] >= Threshold)
                {
                    signalNumber = i + 1;
                    break;
                }
            }
#warning 信号数量
            signalNumber = 1;
            if (signalNumber > eVectors.ColumnCount - 1) signalNumber = eVectors.ColumnCount - 1;
            return eVectors.SubMatrix(0, eVectors.RowCount, signalNumber, eVectors.ColumnCount - signalNumber);
        }
    }
}
