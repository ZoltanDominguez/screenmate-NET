using ScreenMateNET.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;


enum SMSpriteState
{
	Idle,
	Run,
	Jump,
	Walk,
	Dead,
	OnFire
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

		bool wasAlreadyHappy = false;

		Bitmap currentBitmap;
		int framecounter = 0;
		int epsilon = 20; // 20 pixel distance is considered equal
		int speed = 3;


		ScreenMateStateID currentState = ScreenMateStateID.Idle;
		public event Action DrawNeededEvent;
		private Timer fpsTimer;
		private Timer stateChangeTimer;
		private int idleCounter = 0;
		private int sleepingTime = 0;

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
			LoadBitmap(ScreenMateStateID.Bored);
			LoadBitmap(ScreenMateStateID.SittingOnTopOfWindow);
			LoadBitmap(ScreenMateStateID.WarmCPU);

			// WarmCPU
			/*bitMapForStates[ScreenMateStateID.WarmCPU] = SpriteCombiner.CreateBitmap(
				bitMapForStates[ScreenMateStateID.Bored],
				new Bitmap(@"C:\Users\Zoltán\source\repos\screenmate-NET\res\fire\fire.jpg"),
				new Point(100, 0)
				);*/
		}

		private void setupTimers()
		{
			fpsTimer = new Timer(80);
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
			switch (currentState)
			{
				case ScreenMateStateID.CursorChasing:
					MoveTowardMouse();
					break;
				case ScreenMateStateID.Bored:
					BoredAnimation();
					break;
				case ScreenMateStateID.WarmCPU:
					WarmCPUAnimation();
					break;
				case ScreenMateStateID.SittingOnTopOfWindow:
					break;
				case ScreenMateStateID.Idle:

					IdleCPUAnimation();
					break;
				default:
					//currentBitmap = this.bitMapForStates[ScreenMateStateID.Idle][framecounter % 10];
					IdleCPUAnimation();
					break;
			}
			framecounter++;
			DrawNeededEvent.Invoke();
		}

		private void IdleCPUAnimation()
		{
			double waitInFps = Math.Floor(LocalSettings.instance.Settings.WaitingToBoredInSec * 1000 /fpsTimer.Interval);
			// idleCounter = (currentState == ScreenMateStateID.Idle || currentState == ScreenMateStateID.Bored) ? idleCounter + 1 : 0;
			if (idleCounter == waitInFps) sleepingTime = 0;
			if (idleCounter >= waitInFps) currentState = ScreenMateStateID.Bored;
            else currentBitmap = this.bitMapForStates[ScreenMateStateID.Idle][framecounter % 10];
			idleCounter++;
		}
		private void BoredAnimation()
		{
			int offset = sleepingTime >= 4 ? 4 : 0;
			currentBitmap = this.bitMapForStates[ScreenMateStateID.Bored][framecounter % 4 + offset];
			sleepingTime++;
		}

		private void WarmCPUAnimation()
		{
			currentBitmap = this.bitMapForStates[ScreenMateStateID.WarmCPU][framecounter % 12];
			idleCounter = 0;
		}

		/// <summary>
		/// 
		/// </summary>
		private void MoveTowardMouse()
		{
			currentLocation = nextLocation;
			Point mousePosition = System.Windows.Forms.Control.MousePosition;
			if (Math.Abs(CurrentLocation.X - mousePosition.X) < epsilon && Math.Abs(CurrentLocation.Y - mousePosition.Y) < epsilon)
            {
				if (wasAlreadyHappy)
				{
					currentState = ScreenMateStateID.Idle;
					return;
				}
				currentState = ScreenMateStateID.WarmCPU;
				wasAlreadyHappy = true;
				return;
            }
			wasAlreadyHappy = false;
			idleCounter = 0;
			if (CurrentLocation.X + epsilon < mousePosition.X) nextLocation.X = currentLocation.X + speed;
			if (CurrentLocation.X - epsilon > mousePosition.X) nextLocation.X = currentLocation.X - speed;
			if (CurrentLocation.Y + epsilon < mousePosition.Y) nextLocation.Y = currentLocation.Y + speed;
			if (CurrentLocation.Y - epsilon > mousePosition.Y) nextLocation.Y = currentLocation.Y - speed;
			

			Bitmap ResultBitmap = this.bitMapForStates[ScreenMateStateID.CursorChasing][framecounter % 8];
			if (currentLocation.X < mousePosition.X)
				currentBitmap = ResultBitmap;
			else
			{
				Bitmap mirrored = (Bitmap)ResultBitmap.Clone(); //Cloning
				//Mirroring
				mirrored.RotateFlip(RotateFlipType.RotateNoneFlipX);
				currentBitmap = mirrored;
			}
			LocalSettings.Instance.Settings.Stamina+=10;
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

		/// <summary>
		/// Debug function
		/// </summary>
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
			string folderPath = LocalSettings.Instance.StateSettings[id].FilePath;
			DirectoryInfo folder = new DirectoryInfo(folderPath);
			FileInfo[] images = folder.GetFiles();
			List<Bitmap> bitmapsForState = new List<Bitmap>();
			for (int i = 0; i < images.Length; i++)
			{
				Bitmap sourceImage = new Bitmap(String.Format(@"{0}/{1}", folderPath, images[i].Name));
				Bitmap target = new Bitmap(sourceImage, new Size(sourceImage.Width / 8, sourceImage.Height / 8));
				//Bitmap target = new Bitmap(sourceImage, new Size(sourceImage.Width, sourceImage.Height));

				bitmapsForState.Add(target);
			}
			bitMapForStates[id] = bitmapsForState;
        }

	}
}
