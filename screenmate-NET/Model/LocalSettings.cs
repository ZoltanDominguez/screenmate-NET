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
			StateSetting stateSetting = new StateSetting(@"..\..\..\..\res\walk", true);
			StateSettings[ScreenMateStateID.CursorChasing] = stateSetting;

			StateSetting stateSetting2 = new StateSetting(@"..\..\..\..\res\idle", true);
			StateSettings[ScreenMateStateID.Idle] = stateSetting2;

            StateSetting stateSetting3 = new StateSetting(@"..\..\..\..\res\jump", true);
            StateSettings[ScreenMateStateID.Bored] = stateSetting3;

			StateSetting stateSetting4 = new StateSetting(@"..\..\..\..\res\dead", true);
			StateSettings[ScreenMateStateID.SittingOnTopOfWindow] = stateSetting4;

			StateSetting stateSetting5 = new StateSetting(@"..\..\..\..\res\jumpwithfire", true);
			StateSettings[ScreenMateStateID.WarmCPU] = stateSetting5;
			// TODO: upload "warm" images into /res/warm folder
			//StateSetting stateSetting5 = new StateSetting(@"..\..\..\..\res\warm", true);
			//StateSettings[ScreenMateStateID.WarmCPU] = stateSetting5;

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
