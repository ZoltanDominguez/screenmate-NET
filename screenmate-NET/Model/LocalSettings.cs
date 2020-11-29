using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScreenMateNET.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace ScreenMateNET.Model
{
	public class LocalSettings : ISettings
	{
		// Singleton pattern
		public static LocalSettings instance = null;
		private int maxSpeed;
		private int stamina;

		public string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "config.json";
		SettingsSerializable settings;
		public SettingsSerializable Settings { get => settings; private set => settings = value; }

		public Dictionary<ScreenMateStateID, StateSetting> StateSettings { get; private set; }

		public event Action SettingsChanged;

		private LocalSettings()
		{
			initSettingsFromJSON();

			StateSettings = new Dictionary<ScreenMateStateID, StateSetting>();
			// Info from Settings

			// IDLE state always needed as default
			StateSettings[ScreenMateStateID.Idle] = new StateSetting(
			String.Format(@"{0}", Settings.IdlePath),
			true);

			StateSettings[ScreenMateStateID.CursorChasing] = new StateSetting(
				String.Format(@"{0}", Settings.CursorChasingPath),
				Settings.IsCursorChasing);

			StateSettings[ScreenMateStateID.Bored] = new StateSetting(
				String.Format(@"{0}", Settings.BoredPath),
				Settings.IsBoringNeeded);

			StateSettings[ScreenMateStateID.SittingOnTopOfWindow] = new StateSetting(
				String.Format(@"{0}",Settings.GoTopOfWindowPath),
				Settings.IsSittingOnTopOfWindow);

			StateSettings[ScreenMateStateID.WarmCPU] = new StateSetting(
				String.Format(@"{0}", Settings.WarmPath),
				Settings.IsWatchingCPU);
		}

		private void initSettingsFromJSON()
		{
			try
			{
				ReadConfigFromJSON(configFilePath);
			}
			catch (Exception e)
			{
				Trace.WriteLine("A CONFIG fájl beolvasása közben hiba történt. : " + e.Message);
				Settings = new SettingsSerializable(); // Using default constructor then
				Settings.SaveConfigToJSON(configFilePath);
			}
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


		/// <summary>
		/// Feature will be added in the future, to notify classes for changing settings dynamically
		/// </summary>
		/// <param name="id"></param>
		/// <param name="stateSetting"></param>
		public void ChangeSettings(ScreenMateStateID id, StateSetting stateSetting)
		{
			this.StateSettings[id].FilePath = stateSetting.FilePath;
			this.StateSettings[id].IsActivated = stateSetting.IsActivated;
			// kimentés
			SettingsChanged.Invoke();
		}

		public void ReadConfigFromJSON(string configFilePath)
		{
			Settings = SettingsSerializable.Load(configFilePath, false);
		}

		public void SaveStatePermanent()
		{
			Settings.SaveConfigToJSON(configFilePath);
		}
	}
}
