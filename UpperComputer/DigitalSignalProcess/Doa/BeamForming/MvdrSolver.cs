using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Complex;
using System.Numerics;
using DigitalSignalProcess.Basic;
using DigitalSignalProcess.MicrophoneArrays;

namespace UpperComputer.Solve.BeamForming
{
    public class MvdrSolver : BeamFormingSolver
    {
        private Matrix<Complex> inverseAutocorrelationMatrix;

        public MvdrSolver(IMicrophoneArray array) : base(array)
        {
            inverseAutocorrelationMatrix = new DiagonalMatrix(array.ElementCount, array.ElementCount);
        }

        public override void Solve(Complex[][] signal)
        {
            base.Solve(signal);
            inverseAutocorrelationMatrix = autocorrelationMatrix.Inverse();
        }

        public override double Calculate(double frequency, Direction direction)
        {
            var a = array.GetArrayVector(frequency, direction);
            return 1 / Complex.Abs((a.ConjugateTranspose() * inverseAutocorrelationMatrix * a)[0, 0]);
        }
    }
}
