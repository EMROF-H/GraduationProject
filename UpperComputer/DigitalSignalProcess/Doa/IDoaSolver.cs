using System.Numerics;
using DigitalSignalProcess.Basic;

namespace DigitalSignalProcess.Doa
{
    public interface IDoaSolver
    {
        public void Solve(Complex[][] realSignal);

        public double Calculate(double frequency, Direction direction);
    }
}
