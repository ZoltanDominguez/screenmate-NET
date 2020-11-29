using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScreenMateNET.Model
{
	public class SettingsSerializable
	{
		public string idlePath;
		public string cursorChasingPath;
		public string boredPath;
		// public string cursorReachedPath;
		public string goTopOfWindowPath;
		public string warmPath;
        public int cpuPercentLimit;
        public int memoryPercentLimit;
        public int waitingToBoredInSec;
        public bool isBoringNeeded;
        public bool isCursorChasing;

		/// <summary>
		/// Reads Config from specified config.json file, or creates one if not exists
		/// </summary>
		

		public string GetConfigAsJSON() => JsonConvert.SerializeObject(this);

		public SettingsSerializable Load(string configFilePath, bool reverseOrder = false)
		{
			string json = File.ReadAllText(configFilePath);
			JToken token;
			if (reverseOrder)
				token = JArray.Parse(json).LastOrDefault(item => (int)(item["Count"]) != 0);
			else
				token = JArray.Parse(json).FirstOrDefault(item => (int)(item.Count()) != 0);
			token = token ?? JArray.Parse(json).FirstOrDefault();
			SettingsSerializable config = token != null ? token.ToObject<SettingsSerializable>() : new SettingsSerializable();
			return config;
		}

		public void ResetConfigFile(string configFilePath)
		{
			JArray configs = new JArray();
			File.WriteAllText(configFilePath, configs.ToString());
		}

		public void SaveConfigToJSON(string configFilePath)
		{
			JArray configs = new JArray();
			//if (File.Exists(configFilePath))
			//{
			//	configs = JArray.Parse(File.ReadAllText(configFilePath));
			//}
			configs.Add(JObject.FromObject(this));
			File.WriteAllText(configFilePath, configs.ToString());
		}
	}
}
