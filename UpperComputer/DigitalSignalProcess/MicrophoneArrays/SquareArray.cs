namespace DigitalSignalProcess.MicrophoneArrays
{
    public class SquareArray : RectangularArray
    {
        public SquareArray(int side, double direction, CompensationFunc? compensation = null)
            : base(side, direction, side, direction, compensation) { }
    }
}
