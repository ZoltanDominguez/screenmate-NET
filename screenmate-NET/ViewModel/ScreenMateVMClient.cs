using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;

enum Season
{
	Spring,
	Summer,
	Autumn,
	Winter
}

namespace ScreenMateNET
{
	class ScreenMateVMClient : IScreenMateVMClient
	{
		/// <summary>
		/// Lehet érdemes Singletonként?
		/// </summary>
		
		Bitmap currentBitmap;
		public event Action DrawNeededEvent;
		private Timer fpsTimer;
		

		public ScreenMateVMClient()
		{
			fpsTimer = new Timer(33);
			fpsTimer.AutoReset = true;
			fpsTimer.Enabled = true;
			fpsTimer.Elapsed += OnFpsTimerTick;
		}


		/// <summary>
		///  Returns the current bitmap object that need to be drawn.
		/// </summary>
		public Bitmap getNextTileset()
		{
			return currentBitmap;
		}

		private void OnFpsTimerTick(object sender, ElapsedEventArgs e)
		{
			DrawNeededEvent();
		}
	}
}
