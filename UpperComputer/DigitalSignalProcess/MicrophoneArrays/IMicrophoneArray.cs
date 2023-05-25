using DigitalSignalProcess.Basic;
using MathNet.Numerics.LinearAlgebra;
using System.Numerics;

namespace DigitalSignalProcess.MicrophoneArrays
{
    public interface IMicrophoneArray
    {
        public int ElementCount { get; }

        public Matrix<Complex> GetArrayVector(double frequency, Direction direction);
    }
}
