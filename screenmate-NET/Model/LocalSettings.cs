using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenMateNET
{
	class LocalSettings : ISettings
	{
		// Singleton pattern
		private static LocalSettings instance = null;

		public Dictionary<ScreenMateStateID, StateSetting> StateSettings { get; private set; }

		public event Action SettingsChanged;

		private LocalSettings()
		{
			StateSettings = new Dictionary<ScreenMateStateID, StateSetting>();
			// Configból a fileneveket és azonosítókat hozzá és hogy aktív-e

		}

		public static LocalSettings Instance
		{
			get
			{
				if (instance == null)
					instance = new LocalSettings();
				return instance;
			}
		}

		public void ChangeSettings(ScreenMateStateID id, StateSetting stateSetting)
		{
			this.StateSettings[id].FilePath = stateSetting.FilePath;
			this.StateSettings[id].IsActivated = stateSetting.IsActivated;
			// kimentés
			SettingsChanged.Invoke();
		}
	}
}
