using System.Collections.Generic;

namespace ScreenMateNET
{
	interface ISettings
	{
		public Dictionary<ScreenMateStateID, StateSetting> StateSettings { get; }

		public void ChangeSettings(ScreenMateStateID id, StateSetting stateSetting);
	}
}