using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicPlayground
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            var display = new DoubleBufferPlayground();
            display.Width = int.Parse(tbWidth.Text);
            display.Height = int.Parse(tbHeight.Text);
            display.FPS = int.Parse(tbFPS.Text);
            display.Time = int.Parse(tbTime.Text);
            display.Amount = int.Parse(tbAmount.Text);

            IModelDrawer model = new WorldsCollide();
            display.Model = model;            

            display.ShowDialog(this);
        }

       
    }
}
