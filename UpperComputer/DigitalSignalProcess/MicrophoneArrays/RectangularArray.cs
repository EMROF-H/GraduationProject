using DigitalSignalProcess.Basic;

namespace DigitalSignalProcess.MicrophoneArrays
{
    public class RectangularArray : MicrophoneArray
    {
        public RectangularArray(int row, double rowDirection, int column, double columnDirection, CompensationFunc? compensation = null)
            : base(row * column, new Point[row * column], compensation)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    _elements[i * column + j] = new Point(i * rowDirection, j * columnDirection);
                }
            }
        }
    }
}
