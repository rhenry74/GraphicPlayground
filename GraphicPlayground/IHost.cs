using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicPlayground
{
    public interface IHost
    {
        int Width { get; }
        int Height { get; }

        void SetStatictic(string key, string val);
        void SetStatictic(string key, double val);

        MouseEventArgs PeakMouse();
        MouseEventArgs DequeueMouse();

        string LoadWav(string fileName);
        void PlayBuffer(string name);
    }
}
