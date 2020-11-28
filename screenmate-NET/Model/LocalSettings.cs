using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenMateNET
{
	class LocalSettings : ISettings
	{
		// Singleton pattern
		private static LocalSettings instance = null;
		private int maxSpeed;
		private int stamina;

		public Dictionary<ScreenMateStateID, StateSetting> StateSettings { get; private set; }

		public event Action SettingsChanged;

		private LocalSettings()
		{
			StateSettings = new Dictionary<ScreenMateStateID, StateSetting>();
			// Configból a fileneveket és azonosítókat hozzá és hogy aktív-e
			StateSetting stateSetting = new StateSetting(@"C:\Users\T480s\source\repos\screenmate-NET\res\tileset.png", true);
			StateSettings[ScreenMateStateID.CursorChasing] = stateSetting;

			StateSetting stateSetting2 = new StateSetting(@"C:\Users\T480s\source\repos\screenmate-NET\res\tileset.png", true);
			StateSettings[ScreenMateStateID.Idle] = stateSetting2;

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

		public int MaxSpeed { get => maxSpeed; set => maxSpeed = value; }
		public int Stamina { get => stamina; set => stamina = value; }

		public void ChangeSettings(ScreenMateStateID id, StateSetting stateSetting)
		{
			this.StateSettings[id].FilePath = stateSetting.FilePath;
			this.StateSettings[id].IsActivated = stateSetting.IsActivated;
			// kimentés
			SettingsChanged.Invoke();
		}
	}
}
