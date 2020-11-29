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
		private string idlePath = @"..\..\..\..\res\idle";
		private string cursorChasingPath = @"..\..\..\..\res\cursorchasing";
		private string boredPath = @"..\..\..\..\res\dead";

		private string goTopOfWindowPath = @"..\..\..\..\res\sitontopofwindow";
		private string warmPath = @"..\..\..\..\res\jumpwithfire";

        private int cpuPercentLimit = 50;
		private int memoryPercentLimit = 80;
        private int waitingToBoredInSec = 60;

        private bool isBoringNeeded = true;
        private bool isCursorChasing = true;
		private bool isSittingOnTopOfWindow = true;
		private bool isWatchingCPU = true;

		private int stamina = 100;
		private int speed = 3;

		public string IdlePath { get => idlePath; set => idlePath = value; }
		public string CursorChasingPath { get => cursorChasingPath; set => cursorChasingPath = value; }
		public string BoredPath { get => boredPath; set => boredPath = value; }
		public string GoTopOfWindowPath { get => goTopOfWindowPath; set => goTopOfWindowPath = value; }
		public string WarmPath { get => warmPath; set => warmPath = value; }
		public int CpuPercentLimit { get => cpuPercentLimit; set => cpuPercentLimit = value; }
		public int MemoryPercentLimit { get => memoryPercentLimit; set => memoryPercentLimit = value; }
		public int WaitingToBoredInSec { get => waitingToBoredInSec; set => waitingToBoredInSec = value; }
		public bool IsBoringNeeded { get => isBoringNeeded; set => isBoringNeeded = value; }
		public bool IsCursorChasing { get => isCursorChasing; set => isCursorChasing = value; }
		public bool IsSittingOnTopOfWindow { get => isSittingOnTopOfWindow; set => isSittingOnTopOfWindow = value; }
		public bool IsWatchingCPU { get => isWatchingCPU; set => isWatchingCPU = value; }
		public int Stamina { get => stamina; set => stamina = value; }
		public int Speed { get => speed; set => speed = value; }


		/// <summary>
		/// Reads Config from specified config.json file, or creates one if not exists
		/// </summary>
		public string GetConfigAsJSON() => JsonConvert.SerializeObject(this);

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

		public static SettingsSerializable Load(string configFilePath, bool reverseOrder = false)
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
	}
}
