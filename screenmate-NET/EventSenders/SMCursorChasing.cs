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
				// Az egér pozícióját majd csak a form fogja tudni!
				IsActive = true;
				OnActiveStateChanged();
				Thread.Sleep(10000); // 10 másodperc után elfárad, addig nem küld új eseményt
				IsActive = false;
				OnActiveStateChanged();
				Thread.Sleep(5000); // 5 másodperc után kezdi megint.

			}
		}
	}
}
