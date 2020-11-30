using ScreenMateNET.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;

namespace ScreenMateNET.EventSenders
{
	class SMCursorChasing : SMEventSenderBase
	{
		public SMCursorChasing() : base(ScreenMateStateID.CursorChasing) { }
		protected override void EventListenerFunction()
		{
			while (true)
			{
				Thread.Sleep(5000);
				if (DateTime.Now.Minute % 10 == 0)
					IsActive = true;
				else
					IsActive = false;
			}
		}
	}
}
