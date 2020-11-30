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
		public SMCursorChasing():base(ScreenMateStateID.CursorChasing)
		{

		}
		protected override void EventListenerFunction()
		{
			while (true)
			{
				if (DateTime.Now.Minute % 10 == 0)
				{
					int restingTime = LocalSettings.Instance.Settings.Stamina < 5000 ? 5000 - LocalSettings.Instance.Settings.Stamina : 0;
					Thread.Sleep(restingTime); // 5 másodperc után kezdi megint.

					// Az egér pozícióját majd csak a form fogja tudni!
					IsActive = true;
					Thread.Sleep(10000); // 10 másodperc után elfárad, addig nem küld új eseményt
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
