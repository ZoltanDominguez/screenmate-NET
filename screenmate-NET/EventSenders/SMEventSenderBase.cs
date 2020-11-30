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

		private object lockObj = new Object();
		Thread thread;

		private bool isActive;

		public virtual bool IsActive 
		{ 
			get => isActive;
			protected set {
				if (isActive != value)
				{
					isActive = value;
					OnActiveStateChanged();
				}
			} 
		}

		public string TileSetFilePath { get; set; }

		public SMEventSenderBase(ScreenMateStateID stateID)
		{
			StateID = stateID;
			TileSetFilePath = LocalSettings.Instance.StateSettings[stateID].FilePath;
			thread = new Thread(EventListenerFunction);
			thread.Start();
		}

		event Action<ScreenMateStateID> ISMEventSender.ActiveStateChanged
		{
			add
			{
				lock (lockObj)
				{
					activeStateChanged += value;
				}
			}

			remove
			{
				lock (lockObj)
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