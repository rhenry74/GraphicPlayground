using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicPlayground
{
    public class RandomDot : IModelDrawer
    {
        public float X { get; set; }
        public float Y { get; set; }

        private Brush _brush;
        private static int Seed = Guid.NewGuid().ToByteArray()[0] +
            Guid.NewGuid().ToByteArray()[1] +
            Guid.NewGuid().ToByteArray()[2] +
            Guid.NewGuid().ToByteArray()[3];

        private Random Rnd = new Random(Seed++);

        public int G { get; set; }
        public int R { get; set; }
        public int B { get; set; }
        public int A { get; set; }
        public int Size { get; set; }

        public RandomDot()
        {
            
        }

        int storedInterval = 0;
        public bool DrawOn(Graphics g, IHost host, int elapsed)
        {        
            var maxMovePpS = (int)(Size * .5) * 10;
            var maxMove = maxMovePpS * (elapsed + storedInterval) / 1000f;

            if (maxMove < 1f)
            {
                storedInterval = storedInterval + elapsed;
            }
            else
            {
                storedInterval = 0;
            }

            if (maxMove > 1f)
            {
                X = X + Rnd.Next(1 - (int)maxMove, (int)maxMove);
                Y = Y + Rnd.Next(1 - (int)maxMove, (int)maxMove);
                if (X < 0) X = 0;
                if (X > host.Width) X = host.Width;
                if (Y < 0) Y = 0;
                if (Y > host.Height) Y = host.Height;
            }

            g.FillEllipse(_brush, X - Size / 2, Y - Size / 2, Size, Size);

            return true;
        }

        public void Setup(int w, int h, int count, IHost host = null)
        {
            Color customColor = Color.FromArgb(A, R, G, B);
            _brush = new SolidBrush(customColor);
            Size = 10;
        }
    }
}
