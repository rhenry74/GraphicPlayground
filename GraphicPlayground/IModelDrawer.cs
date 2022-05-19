using System.Drawing;
using System.Windows.Forms;

namespace GraphicPlayground
{
    public interface IModelDrawer
    {
        bool DrawOn(Graphics g, IHost host, int elapsed);
        void Setup(int w, int h, int count, IHost host = null);
        int Size { get; set; }
        float X { get; set; }
        float Y { get; set; }
    }
}