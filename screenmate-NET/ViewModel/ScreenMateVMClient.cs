using ScreenMateNET.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

namespace ScreenMateNET.ViewModel
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
		int epsilon;
		int speed = 3;
		private int charHeightOffset = 50;


		ScreenMateStateID currentState = ScreenMateStateID.Idle;
		public event Action DrawNeededEvent;
		private Timer fpsTimer;
		private Timer stateChangeTimer;
		private int idleCounter = 0;
		private int sleepingTime = 0;

		Dictionary<ScreenMateStateID, List<Bitmap>> bitMapForStates;
		// need to keep the information here in memory
		Dictionary<ScreenMateStateID, bool> stateIsActiveMap;
		private Object stateMapLock = new Object();

		Reactor reactor;

		/// <summary>
		/// Private constructor for singleton pattern
		/// </summary>
		private ScreenMateVMClient()
		{
			epsilon = speed + 1; // it ensures it wont oscillate around it.
			bitMapForStates = new Dictionary<ScreenMateStateID, List<Bitmap>>();
			stateIsActiveMap = new Dictionary<ScreenMateStateID, bool>();

			reactor = new Reactor();
			this.LoadAllBitmap();
			reactor.EventReceivedEvent += EventReceivedEventHandler;

			CurrentLocation = new Point(100, 100); // Setup starting location
			NextLocation = new Point(100, 100); // Can be used for interpolation


			setupTimers();
		}

		private void LoadAllBitmap()
		{
			LoadBitmap(ScreenMateStateID.Idle);
			LoadBitmap(ScreenMateStateID.CursorChasing);
			LoadBitmap(ScreenMateStateID.Bored);
			LoadBitmap(ScreenMateStateID.SittingOnTopOfWindow);
			LoadBitmap(ScreenMateStateID.WarmCPU);
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
			/*switch (currentState)
			{
				case ScreenMateStateID.CursorChasing:
					MoveTowardMouseAnimation();
					break;
				case ScreenMateStateID.Bored:
					BoredAnimation();
					break;
				case ScreenMateStateID.WarmCPU:
					WarmCPUAnimation();
					break;
				case ScreenMateStateID.SittingOnTopOfWindow:
					SitOnTopOfWindowAnimation();
					break;
				case ScreenMateStateID.Idle:

					IdleCPUAnimation();
					break;
				default:
					//currentBitmap = this.bitMapForStates[ScreenMateStateID.Idle][framecounter % 10];
					IdleCPUAnimation();
					break;
			}*/
			SitOnTopOfWindowAnimation();
			framecounter++;
			DrawNeededEvent.Invoke();
		}

		private void IdleCPUAnimation()
		{

		}

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

		Point topWindowDestination = new Point();
		int laydownindex = 0;

		private void SitOnTopOfWindowAnimation()
		{
			if (CloseToDestination(topWindowDestination))
			{
				if (laydownindex < 8)
				{
					currentBitmap = FaceToDestination(topWindowDestination, bitMapForStates[ScreenMateStateID.SittingOnTopOfWindow][8 + laydownindex]);
					laydownindex++;
				}
			}
			else
			{
				MoveTowardDestination(topWindowDestination);
				currentBitmap = FaceToDestination(topWindowDestination, bitMapForStates[ScreenMateStateID.SittingOnTopOfWindow][framecounter % 8]);
				laydownindex = 0;
			}
		}

		private void IdleAnimation()
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
		private void MoveTowardMouseAnimation()
		{
			Point mousePosition = System.Windows.Forms.Control.MousePosition;
			if (CloseToDestination(mousePosition))
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
			MoveTowardDestination(mousePosition);

			currentBitmap = FaceToDestination(mousePosition, this.bitMapForStates[ScreenMateStateID.CursorChasing][framecounter % 8]);
			
			
			// LocalSettings.Instance.Settings.Stamina += 10;
		}



		private bool CloseToDestination(Point destination)
		{
			return (Math.Abs(CurrentLocation.X - destination.X) < epsilon) && (Math.Abs(CurrentLocation.Y - destination.Y) < epsilon);
		}

		private bool __CloseToDestination(Point destination)
		{
			return Math.Abs(CurrentLocation.X - destination.X) < epsilon && Math.Abs(CurrentLocation.Y - destination.Y) < epsilon;
		}

		private Bitmap FaceToDestination(Point destination, Bitmap bitmap)
		{
			if (currentLocation.X < destination.X)
				return bitmap;
			else
			{
				Bitmap mirrored = (Bitmap)bitmap.Clone(); //Cloning
														  //Mirroring
				mirrored.RotateFlip(RotateFlipType.RotateNoneFlipX);
				return mirrored;
			}
		}

		private void MoveTowardDestination(Point destination)
		{
			//currentLocation = nextLocation;
			if (CurrentLocation.X < destination.X) currentLocation.X = currentLocation.X + speed;
			if (CurrentLocation.X > destination.X) currentLocation.X = currentLocation.X - speed;
			if (CurrentLocation.Y < destination.Y) currentLocation.Y = currentLocation.Y + speed;
			if (CurrentLocation.Y > destination.Y) currentLocation.Y = currentLocation.Y - speed;
		}
		private void MoveTowardDestinationWithEpsilon(Point destination)
		{
			//currentLocation = nextLocation;
			if (CurrentLocation.X + epsilon < destination.X) currentLocation.X = currentLocation.X + speed;
			if (CurrentLocation.X - epsilon > destination.X) currentLocation.X = currentLocation.X - speed;
			if (CurrentLocation.Y + epsilon < destination.Y) currentLocation.Y = currentLocation.Y + speed;
			if (CurrentLocation.Y - epsilon > destination.Y) currentLocation.Y = currentLocation.Y - speed;
		}
		/// <summary>
		/// Check if change on currentState is needed or not based on the policy implementation
		/// </summary>
		private void OnStateChangeTimerTick(object sender, ElapsedEventArgs e)
		{
			ActivePolicy();
			
			topWindowDestination = WndSearcher.Instance.GetCoordinatesOfTopWindow(); // now here, consider making him a separate timer
			topWindowDestination.Y -= charHeightOffset; // character heightoffset
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
			lock (stateMapLock)
			{
				foreach (ScreenMateStateID id in stateIsActiveMap.Keys)
				{
					bool stateIsActive = stateIsActiveMap[id];
					if (stateIsActive)
					{
						noneIsActive = false;
						Trace.WriteLine("Policy CHANGE! ");
						if (id != currentState)
						{
							currentState = id;
							return;
						}
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
			lock (stateMapLock)
			{
				foreach (ScreenMateStateID id in stateIsActiveMap.Keys)
				{
					Trace.WriteLine("ID: " + id.ToString() + " State: " + stateIsActiveMap[id].ToString());
				}
			}
		}


		public void EventReceivedEventHandler(ScreenMateStateID eventID, bool isActive)
		{
			Trace.WriteLine("Event Received in VM. Event ID: " + eventID.ToString() + "  state: " + isActive.ToString());

			lock (stateMapLock)
			{
				stateIsActiveMap[eventID] = isActive;
			}
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
