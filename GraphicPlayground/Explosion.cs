using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicPlayground
{
    public class Explosion : IModelDrawer
    {

        public float X { get; set; }
        public float Y { get; set; }
        public int Size { get; set; }

        private int RingCount = 1;
        private int RunTime = 0;
        private DateTime FirstTime = DateTime.MinValue;

        public bool DrawOn(Graphics g, IHost host, int elapsed)
        {
            if (FirstTime == DateTime.MinValue)
            {
                FirstTime = DateTime.Now;
            }

            var ringsPerSec = 20;
            RunTime = RunTime + elapsed;
            var elapsedMillis = DateTime.Now.Subtract(FirstTime).TotalMilliseconds;

            var ringsToDraw = (int)(RunTime * ringsPerSec / 1000f);

            for (var cnt = 1; cnt < ringsToDraw; cnt++)
            {
                int bright = (int)(255 * cnt / ringsToDraw);
                var color = Color.FromArgb(170, 255, bright, bright);
                var size = cnt * 3;
                g.DrawEllipse(new Pen(color,1), X- size / 2, Y- size / 2, size, size);
            }

            RingCount = ringsToDraw;
            if (RingCount * 3 > Size * 2)
            {
                return false;
            }
            return true;
        }

        public void Setup(int w, int h, int count, IHost host)
        {
            throw new NotImplementedException();
        }
    }
}
