using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScreenMateNET.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScreenMateNET
{
	class LocalSettings : ISettings
	{
		// Singleton pattern
		private static LocalSettings instance = null;
		private int maxSpeed;
		private int stamina;

		public int CPUPercentLimit { get; set; }
		public int MemoryPercentLimit { get; set; }
		public int WaitingToBoredInSec { get; set; }
		public bool IsBoringNeeded { get; set; }
		public bool IsCursorChasing { get; set; }


		public string configFilePath = "config.json";
		SettingsSerializable SettingsSerializable;

		public Dictionary<ScreenMateStateID, StateSetting> StateSettings { get; private set; }

		public event Action SettingsChanged;

		private LocalSettings()
		{
			StateSettings = new Dictionary<ScreenMateStateID, StateSetting>();

			try
			{
				ReadConfigFromJSON(configFilePath);
			}
			catch (Exception e)
			{
				Console.WriteLine("A CONFIG fájl beolvasása közben hiba történt. : " + e.Message);
				throw;
			}

			// Configból a fileneveket és azonosítókat hozzá és hogy aktív-e
			StateSetting stateSetting = new StateSetting(String.Format(@"{0}", SettingsSerializable.cursorChasingPath), true);
			StateSettings[ScreenMateStateID.CursorChasing] = stateSetting;

			StateSetting stateSetting2 = new StateSetting(String.Format(@"{0}", SettingsSerializable.idlePath), true);
			StateSettings[ScreenMateStateID.Idle] = stateSetting2;

            StateSetting stateSetting3 = new StateSetting(String.Format(@"{0}", SettingsSerializable.boredPath), true);
			StateSettings[ScreenMateStateID.Bored] = stateSetting3;

			StateSetting stateSetting4 = new StateSetting(String.Format(@"{0}", SettingsSerializable.goTopOfWindowPath), true);
			StateSettings[ScreenMateStateID.SittingOnTopOfWindow] = stateSetting4;

			StateSetting stateSetting5 = new StateSetting(String.Format(@"{0}", SettingsSerializable.warmPath), true);
			StateSettings[ScreenMateStateID.WarmCPU] = stateSetting5;

			CPUPercentLimit = SettingsSerializable.cpuPercentLimit;
			MemoryPercentLimit = SettingsSerializable.memoryPercentLimit;
			WaitingToBoredInSec = SettingsSerializable.waitingToBoredInSec;
			IsBoringNeeded = SettingsSerializable.isBoringNeeded;
			IsCursorChasing = SettingsSerializable.isCursorChasing;
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

		public void ReadConfigFromJSON(string configFilePath, bool reverseOrder = false)
		{
			SettingsSerializable = new SettingsSerializable();
			if (File.Exists(configFilePath)) SettingsSerializable = SettingsSerializable.Load(configFilePath, reverseOrder);
		}
	}
}
