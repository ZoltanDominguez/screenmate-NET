using System;
using System.Drawing;

namespace ScreenMateNET
{
	public interface ISMEventSender
	{
		public String TileSetFilePath { get; }
		public ScreenMateStateID StateID { get; }
		public bool IsActive { get; }

		public event Action<ScreenMateStateID> ActiveStateChanged; // Handle
	}
}