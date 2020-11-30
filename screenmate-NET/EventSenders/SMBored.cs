using ScreenMateNET.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ScreenMateNET.EventSenders
{
	class SMBored: SMEventSenderBase
	{
        Point previousMousePoint = new Point();
        int notMovingTicks = 0; 

		public SMBored() : base(ScreenMateStateID.Bored) 
		{
        }

		protected override void EventListenerFunction()
		{
            // Felhasználói interakciók figyelése
            //throw new NotImplementedException();
            
            try
            {
                while (true)
                {
                    Thread.Sleep(1000); 
                    Point mousePos = System.Windows.Forms.Control.MousePosition;
                    double distance = Math.Sqrt(Math.Pow(mousePos.X - previousMousePoint.X, 2) + Math.Pow(mousePos.Y - previousMousePoint.Y, 2));
                    if (distance < 5)
                        notMovingTicks++;
                    else
                        notMovingTicks = 0;

                    IsActive = (notMovingTicks >= LocalSettings.Instance.Settings.WaitingToBoredInSec) ? true : false;

                    previousMousePoint = mousePos;
                }
            }
            catch (Exception)
            {
                throw;
            }


		}

    }
}
