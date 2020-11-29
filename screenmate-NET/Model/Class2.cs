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
	class LocalSettings2 : ISettings
	{
		// Singleton pattern
		private static LocalSettings instance = null;
		private int maxSpeed;
		private int stamina;

		public string configFilePath = "config.json";

		SettingsSerializable SettingsSerializable;

		public Dictionary<ScreenMateStateID, StateSetting> StateSettings { get; private set; }

		public event Action SettingsChanged;

		private LocalSettings2()
		{
			StateSettings = new Dictionary<ScreenMateStateID, StateSetting>();
			try
			{
				ReadConfigFromJSON();
			}
			catch (Exception e)
			{
				Console.WriteLine("A CONFIG fájl beolvasása közben hiba történt. : " + e.Message);
				throw;
			}
			// Configból a fileneveket és azonosítókat hozzá és hogy aktív-e
			StateSetting stateSetting = new StateSetting(@"C:\Users\T480s\source\repos\screenmate-NET\res\tileset.png", true);
			StateSettings[ScreenMateStateID.CursorChasing] = stateSetting;

			StateSetting stateSetting2 = new StateSetting(@"C:\Users\T480s\source\repos\screenmate-NET\res\tileset.png", true);
			StateSettings[ScreenMateStateID.Idle] = stateSetting2;

		}

		public static LocalSettings2 Instance
		{
			get
			{
				if (instance == null)
					instance = new LocalSettings2();
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

		/// <summary>
		/// Reads Config from specified config.json file, or creates one if not exists
		/// </summary>
		public void ReadConfigFromJSON(bool reverseOrder = false)
		{
			if (File.Exists(configFilePath)) SettingsSerializable = Load(reverseOrder);
			else SettingsSerializable = new SettingsSerializable(); // Create New config with default values
		}

		public string GetConfigAsJSON() => JsonConvert.SerializeObject(this.SettingsSerializable);

		private SettingsSerializable Load(bool reverseOrder = false)
		{
			string json = File.ReadAllText(configFilePath);
			JToken token;
			if (reverseOrder)
				token = JArray.Parse(json).LastOrDefault(item => (int)(item["Count"]) != 0);
			else
				token = JArray.Parse(json).FirstOrDefault(item => (int)(item["Count"]) != 0);
			token = token ?? JArray.Parse(json).FirstOrDefault();
			SettingsSerializable config = token != null ? token.ToObject<SettingsSerializable>() : new SettingsSerializable();
			return config;
		}

		public void ResetConfigFile()
		{
			JArray configs = new JArray();
			File.WriteAllText(configFilePath, configs.ToString());
		}

		public void SaveConfigToJSON(SettingsSerializable toSave = null)
		{
			JArray configs = new JArray();
			if (File.Exists(configFilePath))
			{
				configs = JArray.Parse(File.ReadAllText(configFilePath));
			}
			configs.Add(JObject.FromObject(toSave ?? SettingsSerializable));
			File.WriteAllText(configFilePath, configs.ToString());
		}
	}
}
