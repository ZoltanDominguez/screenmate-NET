using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenMateNET
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

        private void Form1_Paint_1(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.Green, 200, 200, 200, 150);
            e.Graphics.DrawRectangle(Pens.Blue, 30, 30, 150, 150);
            e.Graphics.DrawEllipse(Pens.Red, 0, 0, 150, 150);
            e.Graphics.DrawEllipse(Pens.Yellow, 20, 20, 250, 125);
        }
    }
}
