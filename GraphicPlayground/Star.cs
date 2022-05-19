using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicPlayground
{
    public class Star : IModelDrawer
    {

        public float X { get; set; }
        public float Y { get; set; }
        public int Size { get; set; }
        
        private int RingCount = 1;
        private int RunTime = 0;
        private DateTime FirstTime = DateTime.MinValue;
        private int WhenToTwinkle = 0;
        private int TwinkleDuration = 0;
        private int TwinkleElapsed = 100;

        private static int Seed = Guid.NewGuid().ToByteArray()[0] +
            Guid.NewGuid().ToByteArray()[1] +
            Guid.NewGuid().ToByteArray()[2] +
            Guid.NewGuid().ToByteArray()[3];
        private static Random Rnd = new Random(Seed++);

        private Color color;
        private Pen pen;
        private Brush brush;
        public bool DrawOn(Graphics g, IHost host, int elapsed)
        {
            if (FirstTime == DateTime.MinValue)
            {
                FirstTime = DateTime.Now;
            }
            
            RunTime = RunTime + elapsed;
            var elapsedMillis = DateTime.Now.Subtract(FirstTime).TotalMilliseconds;            

            if (elapsedMillis > WhenToTwinkle)
            {
                var sizeToDraw = (int)((TwinkleElapsed / (float)TwinkleDuration) * (Size * 4f));
                g.DrawLine(pen, X - sizeToDraw / 2, Y, X + sizeToDraw / 2, Y);
                g.DrawLine(pen, X, Y - sizeToDraw / 2, X, Y + sizeToDraw / 2);

                TwinkleElapsed += elapsed;

                if (TwinkleElapsed > TwinkleDuration)
                {
                    FirstTime = DateTime.Now;
                    RunTime = 0;
                    TwinkleElapsed = 100;
                }
            }

            g.FillEllipse(brush , X - Size / 2, Y - Size / 2, Size, Size);

            return true;
        }

        public void Setup(int w, int h, int count, IHost host = null)
        {
            X = Rnd.Next(0, w);
            Y = Rnd.Next(0, h);
            Size = Rnd.Next(2, 8);
            WhenToTwinkle = Rnd.Next(2000, 8000);//every 2 to 8 seconds
            TwinkleDuration = Size * 200;
            color = Color.FromArgb(180, 240, Rnd.Next(200, 240), 255);
            pen = new Pen(color, 1);
            brush = new SolidBrush(color);
        }
    }
}
