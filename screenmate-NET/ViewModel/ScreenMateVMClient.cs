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
		List<Bitmap> bitmaps;


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

		private void BitmapLoad(String filename)
		{
			Image image = Image.FromFile(filename);
			Size size = new Size(12, 8);

			int xMax = image.Width;
			int yMax = image.Height;
			int tileWidth = xMax / size.Width;
			int tileHeight = yMax / size.Height;

			// if (!Directory.Exists(outputPath)) { Directory.CreateDirectory(outputPath); }

			for (int x = 0; x < size.Width; x++)
			{
				for (int y = 0; y < size.Height; y++)
				{
					// string outputFileName = Path.Combine(outputPath, string.Format("{0}_{1}.jpg", x, y));

					Rectangle tileBounds = new Rectangle(x * tileWidth, y * tileHeight, tileWidth, tileHeight);
					Bitmap target = new Bitmap(tileWidth, tileHeight);

					using (Graphics graphics = Graphics.FromImage(target))
					{
						graphics.DrawImage(
							image,
							new Rectangle(0, 0, tileWidth, tileHeight),
							tileBounds,
							GraphicsUnit.Pixel);
					}
					bitmaps.Add(target);
				}
			}
		}

	}
}
