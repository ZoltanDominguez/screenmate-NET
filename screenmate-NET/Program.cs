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

			ScreenMateForm screenMateForm = new ScreenMateForm();
			screenMateForm.BackColor = Color.White;
			screenMateForm.TransparencyKey = Color.White;
			screenMateForm.FormBorderStyle = FormBorderStyle.None;
			screenMateForm.Bounds = Screen.PrimaryScreen.Bounds;
			screenMateForm.TopMost = true;

			Application.Run(screenMateForm);
		}
	}
}
