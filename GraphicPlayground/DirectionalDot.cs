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
    public class DirectionalDot : IModelDrawer
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
        public float XDirPerSec { get; set; }
        public float YDirPerSec { get; set; }
        
        public bool DrawOn(Graphics g, IHost host, int elapsed)
        {         
            if (_brush == null)
            {
                Color customColor = Color.FromArgb(A, R, G, B);
                _brush = new SolidBrush(customColor);
            }

            X = X + XDirPerSec * elapsed / 1000;
            Y = Y + YDirPerSec * elapsed / 1000;

            if (X < 0) RandomizeDirection();
            if (X > host.Width) RandomizeDirection();
            if (Y < 0) RandomizeDirection();
            if (Y > host.Height) RandomizeDirection();

            g.FillEllipse(_brush, X - Size / 2, Y - Size / 2, Size, Size);

            return true;            
        }

        private void RandomizeDirection()
        {
            var maxDir = Size / 2 * 10;
            XDirPerSec = Rnd.Next(1 - maxDir, maxDir);
            YDirPerSec = Rnd.Next(1 - maxDir, maxDir);
        }

        public void Setup(int w, int h, int count, IHost host = null)
        {                       
            Size = Rnd.Next(8,12);
            RandomizeDirection();
        }
    }
}
