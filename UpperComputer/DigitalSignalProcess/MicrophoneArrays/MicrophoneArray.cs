using DigitalSignalProcess.Basic;
using MathNet.Numerics.LinearAlgebra;
using System.Numerics;
using static System.Math;

namespace DigitalSignalProcess.MicrophoneArrays
{
    public class MicrophoneArray : IMicrophoneArray
    {
        public delegate double CompensationFunc(int element, double frequency);

        protected static readonly CompensationFunc DefaultCompensation = (element, frequency) => 0;

        public const double WaveVelocity = 343;

        public int ElementCount { get; }

        protected readonly Point[] _elements;

        public Point this[Index index] => _elements[index];

        private readonly CompensationFunc Compensation;

        public MicrophoneArray(int elementCount, Point[] elements, CompensationFunc? compensation = null)
        {
            ElementCount = elementCount;
            _elements = elements;
            Compensation = compensation ?? DefaultCompensation;
        }

        private Complex GetArrayValue(int element, double frequency, Direction direction)
        {
            Point e = this[element];
            return Complex.Exp(Complex.ImaginaryOne * Tau
                * (
                    (
                        (e.X * Cos(direction.Azimuth + PI) + e.Y * Sin(direction.Azimuth + PI))
                        * Sin(direction.Pitch) / (WaveVelocity / frequency)
                    )
                    + Compensation(element, frequency)
                  ));
        }

        public Matrix<Complex> GetArrayVector(double frequency, Direction direction)
        {
            Matrix<Complex> result = new MathNet.Numerics.LinearAlgebra.Complex.DenseMatrix(ElementCount, 1);
            for (int i = 0; i < result.RowCount; i++)
            {
                result[i, 0] = GetArrayValue(i, frequency, direction);
            }
            return result;
        }
    }
}
