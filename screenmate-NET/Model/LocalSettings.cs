using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenMateNET
{
	class LocalSettings : ISettings
	{
		// Singleton pattern
		private static LocalSettings instance = null;

		public Dictionary<ScreenMateStateID, StateSetting> StateSettings { get; set; }
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
	}
}
