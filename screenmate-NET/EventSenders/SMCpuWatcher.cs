using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ScreenMateNET
{
	class SMCpuWatcher : SMEventSenderBase
	{
		public SMCpuWatcher()
		{
			State = ScreenMateStateEnum.Warm; // azonosításra lesz
		}

		protected override void EventListenerFunction()
		{
			Thread.Sleep(5000);
			IsActive = !IsActive; //DEMO
			OnActiveStateChanged();
		}
	}
}
