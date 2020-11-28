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

		//protected Bitmap bitmap;
		public ScreenMateStateEnum State{ get; protected set; }

		public event Action ActiveStateChanged;

		private object lockObj = new Object();

		Thread thread;

		protected const String filename = "";

		public Bitmap ScreenMateBitmap { get; private set; }

		public bool IsActive { get; protected set; }

		public SMEventSenderBase()
		{
			ReadFromBitmapFile(filename);
			thread = new Thread(EventListenerFunction);
		}

		protected abstract void EventListenerFunction();

		protected void ReadFromBitmapFile(String filename)
		{
			ScreenMateBitmap = new Bitmap(1, 2); // TODO impl.
		}

		protected void OnActiveStateChanged()
		{
			if (ActiveStateChanged != null)
				ActiveStateChanged();
		}

		//protected abstract EventListenerFunction();
	}
}