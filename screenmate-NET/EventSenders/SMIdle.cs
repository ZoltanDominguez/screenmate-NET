using ScreenMateNET.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ScreenMateNET.EventSenders
{
	class SMIdle : SMEventSenderBase
	{
		public SMIdle():base(ScreenMateStateID.Idle)
		{

		}
		protected override void EventListenerFunction()
		{
			while (true)
			{
				if (DateTime.Now.Minute % 10 == 9)
				{
					IsActive = true;
					Thread.Sleep(60000);
					IsActive = false;
				}
				else
				{
					IsActive = false;
					Thread.Sleep(5000);
				}

			}
		}
	}
}
