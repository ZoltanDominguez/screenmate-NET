using ScreenMateNET.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms.Design;

namespace ScreenMateNET
{
	class SMCpuWatcher : SMEventSenderBase
	{
		protected static PerformanceCounter cpuCounter;
		int limit;
		public SMCpuWatcher():base(ScreenMateStateID.WarmCPU)
		{
			limit = LocalSettings.instance.Settings.CpuPercentLimit;
		}

		protected override void EventListenerFunction()
		{
			cpuCounter = new PerformanceCounter();
			cpuCounter.CategoryName = "Processor";
			cpuCounter.CounterName = "% Processor Time";
			cpuCounter.InstanceName = "_Total";


			while (true)
            {
				Thread.Sleep(1000);
				// ha itt el akar szállni valami invalidexceptionnel (cannot load counter name - invalid index), akkor admin módban kell futtatni a cmd-t, majd: 
				// "C:\windows\SysWOW64> lodctr /r". Ha erre kapsz egy "Error: Unable to rebuild performance counter setting..."-et, akkor "C:\windows\SysWOW64> lodctr /r" lesz a megoldás.
				float cpuPercent = cpuCounter.NextValue();


				if (cpuPercent >= limit)
				{
					IsActive = true;
					OnActiveStateChanged();
				}
				else
				{
					IsActive = false;
					OnActiveStateChanged();
				}
			}
			
            
        }
	}
}
