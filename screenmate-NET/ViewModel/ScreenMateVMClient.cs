using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Timers;

enum Season
{
	Spring,
	Summer,
	Autumn,
	Winter
}

namespace ScreenMateNET
{
	class ScreenMateVMClient : IScreenMateVMClient
	{
		/// <summary>
		/// Singleton pattern
		/// </summary>

		private static ScreenMateVMClient instance = null;


		Bitmap currentBitmap;
		ScreenMateStateID currentState;
		public event Action DrawNeededEvent;
		private Timer fpsTimer;
		private Timer stateChangeTimer;


		Dictionary<ScreenMateStateID, List<Bitmap>> bitMapForStates;
		// need to keep the information here
		Dictionary<ScreenMateStateID, bool> stateIsActiveMap;

		Reactor reactor;

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private ScreenMateVMClient()
		{
			setupTimers();

			bitMapForStates = new Dictionary<ScreenMateStateID, List<Bitmap>>();
			stateIsActiveMap = new Dictionary<ScreenMateStateID, bool>();

			reactor = new Reactor();
			reactor.EventReceivedEvent += EventReceivedEventHandler;
		}

		private void setupTimers()
		{
			fpsTimer = new Timer(33);
			fpsTimer.AutoReset = true;
			fpsTimer.Enabled = true;
			fpsTimer.Elapsed += OnFpsTimerTick;

			stateChangeTimer = new Timer(2000);
			stateChangeTimer.AutoReset = true;
			stateChangeTimer.Enabled = true;
			stateChangeTimer.Elapsed += OnStateChangeTimerTick;
		}

		/// <summary>
		/// Singleton Instance static property
		/// </summary>
		public static ScreenMateVMClient Instance
		{
			get
			{
				if (instance == null)
					instance = new ScreenMateVMClient();
				return instance;
			}
		}

		/// <summary>
		///  Returns the current bitmap object that need to be drawn.
		/// </summary>
		public Bitmap getNextTileset() => currentBitmap;

		private void OnFpsTimerTick(object sender, ElapsedEventArgs e) => DrawNeededEvent.Invoke();
		
		/// <summary>
		/// Check if change on currentState is needed or not based on the policy implementation
		/// </summary>
		private void OnStateChangeTimerTick(object sender, ElapsedEventArgs e)
		{
			ActivePolicy();
		}

		/// <summary>
		/// Abstraction for the acutal Policy implementation to use
		/// </summary>
		private void ActivePolicy() => currentStateChangePolicy_FirstFound();


		/// <summary>
		/// Set the currentstate to the first different active state in stateIsActiveMap 
		/// </summary>
		private void currentStateChangePolicy_FirstFound()
		{
			foreach (ScreenMateStateID id in stateIsActiveMap.Keys)
			{
				bool stateIsActive = stateIsActiveMap[id];
				if (stateIsActive && id != currentState)
				{
					currentState = id;
					return;
				}
			}
		}


		public void EventReceivedEventHandler(ScreenMateStateID eventID, bool isActive)
		{
			stateIsActiveMap[eventID] = isActive;
			// ActivePolicy hívás? Akkor kell bele egy cooldown!
		}

	}
}
