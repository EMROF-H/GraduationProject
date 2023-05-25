using System;
using DigitalSignalProcess.Basic;

namespace UpperComputer.Display
{
    public interface IDisplayer
    {
        public void Display(Func<double, Direction, double> calculate, double frequency);
    }
}
