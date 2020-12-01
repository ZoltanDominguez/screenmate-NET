using ScreenMateNET.Model;
using System;
using System.Drawing;
using System.Threading;

namespace ScreenMateNET
{
	abstract class SMEventSenderBase : ISMEventSender
	{
		/// <summary>
		/// Child class must implement EventListenerFunction.
		/// This function is started in a separate Thread.
		/// </summary>

		//ScreenMateStateID stateID;
		public ScreenMateStateID StateID
		{ get; private set; }

		protected event Action<ScreenMateStateID> activeStateChanged;
		//public event Action ActiveStateChanged;

		private object lockObjAction = new Object();
		Thread thread;

		private object lockObjIsActive = new Object();

		private bool isActive = false;

		public virtual bool IsActive 
		{
			get 
			{
				lock (lockObjIsActive)
				{
					return isActive;
				}
			}
			protected set
			{
				lock (lockObjIsActive)
				{
					if (isActive != value)
					{
						isActive = value;
						OnActiveStateChanged();
					}
				}
			} 
		}

		public string TileSetFilePath { get; set; }

		public SMEventSenderBase(ScreenMateStateID stateID)
		{
			StateID = stateID;
			TileSetFilePath = LocalSettings.Instance.StateSettings[stateID].FilePath;
			thread = new Thread(EventListenerFunction);
			thread.IsBackground = true;
			thread.Start();
		}

		event Action<ScreenMateStateID> ISMEventSender.ActiveStateChanged
		{
			add
			{
				lock (lockObjAction)
				{
					activeStateChanged += value;
				}
			}

			remove
			{
				lock (lockObjAction)
				{
					activeStateChanged -= value;
				}
			}
		}

		protected abstract void EventListenerFunction();

		protected void OnActiveStateChanged()
		{
			if (activeStateChanged != null)
				activeStateChanged.Invoke(StateID);
		}

		//protected abstract EventListenerFunction();
	}
}