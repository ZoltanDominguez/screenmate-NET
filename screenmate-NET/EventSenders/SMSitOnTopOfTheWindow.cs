using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ScreenMateNET.EventSenders
{
	class SMSitOnTopOfTheWindow : SMEventSenderBase
	{
		public SMSitOnTopOfTheWindow():base(ScreenMateStateID.SittingOnTopOfWindow)
		{

		}

		protected override void EventListenerFunction()
		{
			//.Sleep(10000);
			IsActive = true;
		}
	}
}
