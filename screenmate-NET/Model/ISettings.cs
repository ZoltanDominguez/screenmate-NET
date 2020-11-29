using ScreenMateNET.Model;
using System.Collections.Generic;

namespace ScreenMateNET.Model
{
	interface ISettings
	{
		public Dictionary<ScreenMateStateID, StateSetting> StateSettings { get; }
		public SettingsSerializable Settings { get; }
		public void ChangeSettings(ScreenMateStateID id, StateSetting stateSetting);
	}
}