using System;
using System.Collections.Generic;
using System.Text;

namespace ScreenMateNET
{
	public class Reactor
	{
		Dictionary<ScreenMateStateEnum, ISMEventSender> eventSenders;
		public Reactor()
		{
			// Eseményküldők létrehozása, nyílvántartása Dict-ben.
			eventSenders = new Dictionary<ScreenMateStateEnum, ISMEventSender>();
			SMCpuWatcher sMCpuWatcher = new SMCpuWatcher();
			sMCpuWatcher.ActiveStateChanged += CpuEventHandler;
			eventSenders.Add(sMCpuWatcher.State, sMCpuWatcher);
			// Eseménykezelő fv -> továbbítja az azont. a VM-nek. - Továbbdob egy eseményt <Enum>-al?
		}

		private void CpuEventHandler()
		{
			throw new NotImplementedException();
			// VM state-jének változtatása, ő pedig értesesíti a View-t
		}
	}

}
