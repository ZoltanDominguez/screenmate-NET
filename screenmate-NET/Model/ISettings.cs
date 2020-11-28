using System.Collections.Generic;

namespace ScreenMateNET
{
	interface ISettings
	{
		public Dictionary<ScreenMateStateID, StateSetting> StateSettings { get; }

		public int MaxSpeed { get; set; }
		public int Stamina { get; set; }

		public void ChangeSettings(ScreenMateStateID id, StateSetting stateSetting);
	}
}