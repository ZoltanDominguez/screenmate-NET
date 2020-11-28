using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms.Design;

namespace ScreenMateNET
{
	class SMCpuWatcher : SMEventSenderBase
	{
		public SMCpuWatcher():base(ScreenMateStateID.WarmCPU)
		{

		}

		protected override void EventListenerFunction()
		{
			Thread.Sleep(5000);
			IsActive = !IsActive; //DEMO
			OnActiveStateChanged();
		}
	}
}
