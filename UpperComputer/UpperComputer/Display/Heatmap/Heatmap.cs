using System.Windows.Controls;

namespace UpperComputer.Display.Heatmap
{
    public abstract class Heatmap : Displayer
    {
        protected Heatmap(UIElementCollection father, int width, int height) : base(father, width, height) { }
    }
}
