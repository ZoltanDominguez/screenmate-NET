using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScreenMateNET.Model
{
	public class StateSetting
	{
		public String FilePath { get; set; }
		public bool IsActivated { get; set; }

		//public int AnimationTimeFrame{ get; set; } lehet kell int érték az animáció gyorsaságára?

		public StateSetting(String filePath, bool isActivated)
		{
			FilePath = filePath;
			IsActivated = isActivated;
		}
	}
}
