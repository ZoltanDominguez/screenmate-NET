using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenMateNET
{
	public partial class ScreenMateForm : Form
	{
		ScreenMateVMClient screenMateVMClient;

		public ScreenMateForm()
		{
			InitializeComponent();
			screenMateVMClient = new ScreenMateVMClient();
			screenMateVMClient.DrawNeededEvent += DrawNeededEventHandler;
		}

		private void ScreenMateFormOnPaint(object sender, PaintEventArgs e)
		{
			Bitmap b = screenMateVMClient.getNextTileset();
			Trace.WriteLine("OnPaint Called");
			// TODO Draw tileset
		}


		private void DrawNeededEventHandler()
		{
			Invalidate();
		}

	}
}
