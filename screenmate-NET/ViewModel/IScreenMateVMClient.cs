using System;
using System.Drawing;

namespace ScreenMateNET
{
	public interface IScreenMateVMClient
	{
		event Action DrawNeededEvent;
		public Bitmap getNextTileset();
		public Point CurrentLocation { get; set; }
		public Point NextLocation { get; set; }

	}
}