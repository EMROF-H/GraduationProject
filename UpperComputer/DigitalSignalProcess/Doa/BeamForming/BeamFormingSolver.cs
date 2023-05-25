using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System.Numerics;
using DigitalSignalProcess.Basic;
using DigitalSignalProcess.MicrophoneArrays;
using DigitalSignalProcess.Doa;

namespace UpperComputer.Solve.BeamForming
{
    public class BeamFormingSolver : DoaSolver
    {
        protected Matrix<Complex> autocorrelationMatrix;

        public BeamFormingSolver(IMicrophoneArray array) : base(array)
        {
            autocorrelationMatrix = new DiagonalMatrix(array.ElementCount, array.ElementCount);
        }

        public override void Solve(Complex[][] signal)
        {
            Matrix<Complex> complexSignal = ToComplexSignal(signal);
            autocorrelationMatrix = Autocorrelation(complexSignal);
        }

        public override double Calculate(double frequency, Direction direction)
        {
            var a = array.GetArrayVector(frequency, direction);
            return Complex.Abs((a.ConjugateTranspose() * autocorrelationMatrix * a)[0, 0]);
        }
    }
}
