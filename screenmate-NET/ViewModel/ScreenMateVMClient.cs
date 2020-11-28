using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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

		private Point currentLocation, nextLocation;
		public Point CurrentLocation { get => currentLocation; set => currentLocation = value; }
		public Point NextLocation { get => nextLocation; set => nextLocation = value; }

		Bitmap currentBitmap;
		int framecounter = 0;
		int epsilon = 20; // 20 pixel distance is considered equal
		int speed = 5;


		ScreenMateStateID currentState = ScreenMateStateID.Idle;
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

			bitMapForStates = new Dictionary<ScreenMateStateID, List<Bitmap>>();
			stateIsActiveMap = new Dictionary<ScreenMateStateID, bool>();

			reactor = new Reactor();
			this.LoadAllBitmap();
			reactor.EventReceivedEvent += EventReceivedEventHandler;

			CurrentLocation = new Point(100, 100); // Setup starting location
			NextLocation = new Point(100, 100); // Setup starting location

			setupTimers();
		}

		private void LoadAllBitmap()
		{
			LoadBitmap(ScreenMateStateID.Idle);
			LoadBitmap(ScreenMateStateID.CursorChasing);
		}

		private void setupTimers()
		{
			fpsTimer = new Timer(140);
			fpsTimer.AutoReset = true;
			fpsTimer.Enabled = true;
			fpsTimer.Elapsed += OnFpsTimerTick;

			stateChangeTimer = new Timer(1000);
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

		private void OnFpsTimerTick(object sender, ElapsedEventArgs e)
		{
			if(currentState == ScreenMateStateID.CursorChasing) MoveTowardMouse();

			currentBitmap = this.bitMapForStates[currentState][framecounter%3];
			framecounter++;

			DrawNeededEvent.Invoke();
		}

		/// <summary>
		/// 
		/// </summary>
		private void MoveTowardMouse()
		{
			currentLocation = nextLocation;
			Point mousePosition = System.Windows.Forms.Control.MousePosition;
			if (CurrentLocation.X + epsilon < mousePosition.X) nextLocation.X = currentLocation.X + speed;
			if (CurrentLocation.X - epsilon > mousePosition.X) nextLocation.X = currentLocation.X - speed;
			if (CurrentLocation.Y + epsilon < mousePosition.Y) nextLocation.Y = currentLocation.Y + speed;
			if (CurrentLocation.Y - epsilon > mousePosition.Y) nextLocation.Y = currentLocation.Y - speed;
		}

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
			Trace.WriteLine("Policy called. Current State is: " + currentState.ToString());
			printMap();
			stateIsActiveMap[ScreenMateStateID.Bored] = false;
			bool noneIsActive = true;
			foreach (ScreenMateStateID id in stateIsActiveMap.Keys)
			{
				bool stateIsActive = stateIsActiveMap[id];
				if (stateIsActive)
				{
					noneIsActive = false;
					Trace.WriteLine("Policy CHANGE! ");
					if(id != currentState)
					{
						currentState = id;
						return;
					}
				}
			}
			if(noneIsActive) currentState = ScreenMateStateID.Idle; // Idle if no active was found
		}

		void printMap()
		{
			Trace.WriteLine("\n");
			foreach (ScreenMateStateID id in stateIsActiveMap.Keys)
			{
				Trace.WriteLine("ID: " + id.ToString() + " State: " + stateIsActiveMap[id].ToString());
			}
		}


		public void EventReceivedEventHandler(ScreenMateStateID eventID, bool isActive)
		{
			Trace.WriteLine("Event Received in VM. Event ID: " + eventID.ToString() + "  state: " + isActive.ToString());

			stateIsActiveMap[eventID] = isActive;
			// ActivePolicy hívás? Akkor kell bele egy cooldown!
		}

		private void LoadBitmap(ScreenMateStateID id)
		{
			Image image = Image.FromFile(LocalSettings.Instance.StateSettings[id].FilePath);
			Size size = new Size(12, 8);

			int xMax = image.Width;
			int yMax = image.Height;
			int tileWidth = xMax / size.Width;
			int tileHeight = yMax / size.Height;

			// if (!Directory.Exists(outputPath)) { Directory.CreateDirectory(outputPath); }
			List<Bitmap> bitmapsForState = new List<Bitmap>();
			int y = 0; // ezen variálni kell, ha mindent ebből a fájlból akarunk beolvasni.
			for (int x = 0; x < xMax; x+=tileWidth)
			{
				Rectangle tileBounds = new Rectangle(x, y, tileWidth, tileHeight);
				Bitmap target = new Bitmap(tileWidth, tileHeight);
				using (Graphics graphics = Graphics.FromImage(target))
				{
					graphics.DrawImage(
						image,
						new Rectangle(0, 0, tileWidth, tileHeight),
						tileBounds,
						GraphicsUnit.Pixel);
				}
				bitmapsForState.Add(target);
			}
			this.bitMapForStates[id] = bitmapsForState;
		}

	}
}
