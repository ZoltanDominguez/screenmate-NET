using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace ScreenMateNET
{
	public class Reactor
	{
		public event Action<ScreenMateStateID, bool> EventReceivedEvent;

		private Dictionary<ScreenMateStateID, ISMEventSender> EventSenders { get; set; }

		public Reactor()
		{
			// Eseményküldők létrehozása, nyílvántartása Dict-ben.
			EventSenders = new Dictionary<ScreenMateStateID, ISMEventSender>();

			// Instantiate and add EventSenders to Dictionary
			AddEventSenderToDict(new SMCpuWatcher());

			// Subscribe GeneralLocalEventHandler to each EventSender's event
			foreach (ISMEventSender eventSender in EventSenders.Values)
			{
				eventSender.ActiveStateChanged += GeneralLocalEventHandler;
			}
		}

		private void GeneralLocalEventHandler(ScreenMateStateID stateID)
		{
			if (EventReceivedEvent != null)
				EventReceivedEvent.Invoke(stateID, EventSenders[stateID].IsActive);
		}

		private void AddEventSenderToDict( ISMEventSender eventSender)
		{
			EventSenders.Add(eventSender.StateID, eventSender);
		}
	}

}
