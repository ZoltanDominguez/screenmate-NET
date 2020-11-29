using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenMateNET.EventSenders
{
	class SitOnTopOfTheWindowEventSender : SMEventSenderBase
	{
		public SitOnTopOfTheWindowEventSender():base(ScreenMateStateID.SittingOnTopOfWindow)
		{

		}

		protected override void EventListenerFunction()
		{
			throw new NotImplementedException();
		}
	}
}
