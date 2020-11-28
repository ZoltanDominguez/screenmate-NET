using System;
using System.Drawing;

namespace ScreenMateNET
{
	enum ScreenMateStateEnum
	{
		Idle,
		RunLeft,
		RinRight,
		Bored,
		Warm,
		Cold
	}

	interface ISMEventSender
	{
		public Bitmap ScreenMateBitmap{ get; }
		public ScreenMateStateEnum State { get; }
		public bool IsActive { get; }

		public event Action ActiveStateChanged; // Handle
	}
}