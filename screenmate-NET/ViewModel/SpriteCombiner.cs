using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ScreenMateNET
{
	public class SpriteCombiner
	{
		public static List<Bitmap> CreateBitmap(List<Bitmap> bottom, Bitmap topSprite, Point Shift)
		{
			double scale = 2;
			Bitmap top = new Bitmap(topSprite, new Size((int)(topSprite.Width * scale), (int)(topSprite.Height * scale)));


			List<Bitmap> result = new List<Bitmap>();

			int topindex = 0;
			int saveindex = 0;
			Size size = new Size(5, 2);
			int xMax = top.Width;
			int yMax = top.Height;
			int tileWidth = xMax / size.Width;
			int tileHeight = yMax / size.Height;
			Size fireSize = new Size((int)(top.Width * 0.178), (int)(top.Height * 20 / 60));
			foreach (Bitmap item in bottom)
			{
				
				Rectangle tileBounds = new Rectangle(61 + topindex*fireSize.Width, (int)(top.Height*0.25), fireSize.Width+10, fireSize.Height);
				using (Graphics graphics = Graphics.FromImage(item))
				{
					graphics.DrawImage(
						top,
						new Rectangle(item.Width-fireSize.Width+50, item.Height-fireSize.Height, fireSize.Width-20, fireSize.Height-10),
						tileBounds,
						GraphicsUnit.Pixel);
				}
				topindex = (topindex + 1) % size.Width;
				result.Add(item);

				//Bitmap saveitem = new Bitmap(item, new Size(item.Width * 8, item.Height * 8)); // Transform to be the same size
				item.Save(@"C:\Users\Zoltán\source\repos\screenmate-NET\res\fire\" + saveindex.ToString() + ".png");
				saveindex++;

			}
			return result;
		}


	}
}
