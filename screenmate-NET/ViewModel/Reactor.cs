using ScreenMateNET.EventSenders;
using ScreenMateNET.Model;
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
			initEventSenders();
		}

		private void initEventSenders()
		{
			// Eseményküldők létrehozása, nyílvántartása Dict-ben.
			EventSenders = new Dictionary<ScreenMateStateID, ISMEventSender>();

			// Instantiate and add EventSenders to Dictionary
			RegisterEventSenderToDict(new SMCursorChasing());
			RegisterEventSenderToDict(new SMCpuWatcher());
			RegisterEventSenderToDict(new SMBored());
			RegisterEventSenderToDict(new SMSitOnTopOfTheWindow());



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

		private void RegisterEventSenderToDict( ISMEventSender eventSender)
		{
			if (LocalSettings.Instance.StateSettings[eventSender.StateID].IsEnabled)
				EventSenders.Add(eventSender.StateID, eventSender);
		}
	}

}
