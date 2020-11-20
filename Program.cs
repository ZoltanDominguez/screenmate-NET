using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace ScreenMateNET
{
  static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      Form1 f = new Form1();
      f.BackColor = Color.White;
      f.TransparencyKey = Color.White;
      f.FormBorderStyle = FormBorderStyle.None;
      f.Bounds = Screen.PrimaryScreen.Bounds;
      f.TopMost = true;
      Application.Run(f);

      // Application.Run(new Form1());



    }
  }
}
