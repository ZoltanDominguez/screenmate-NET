using ScreenMateNET.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenMateNET
{
	public partial class ScreenMateForm : Form
	{
		IScreenMateVMClient screenMateVMClient;

		public ScreenMateForm()
		{
			InitializeComponent();
			DoubleBuffered = true;
			screenMateVMClient = ScreenMateVMClient.Instance;
			screenMateVMClient.DrawNeededEvent += DrawNeededEventHandler;
		}

		private void ScreenMateFormOnPaint(object sender, PaintEventArgs e)
		{
			Bitmap bitmap = screenMateVMClient.getNextTileset();
			int width = 48;
			int height = 72;
			if (bitmap != null)
			{
				e.Graphics.DrawImage(bitmap, screenMateVMClient.CurrentLocation); 

			}
		}

		private void MouseMoved(object sender, MouseEventArgs e)
		{
			Text = e.Location.X + ":" + e.Location.Y;
		}


		private void DrawNeededEventHandler()
		{
			Invalidate();
		}

	}
}
