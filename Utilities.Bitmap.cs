/*
using System.Drawing;
using System.Runtime.InteropServices;

namespace Multfinite.Utilities
{
	public partial class Utilities
	{
		/// <summary>
		///  invert bgr->rgb and rgb ->bgr (replace values on the edge)
		/// </summary>
		/// <param name="source"></param>
		/// <param name="buffMult"></param>
		/// <returns></returns>
		public static Bitmap Invert(this Bitmap source, int step = 3) => Invert(source, source.PixelFormat, step);
		/// <summary>
		///  invert bgr->rgb and rgb ->bgr (replace values on the edge)
		/// </summary>
		/// <param name="source"></param>
		/// <param name="buffMult"></param>
		/// <returns></returns>
		public static unsafe Bitmap Invert(this Bitmap source, PixelFormat pixelFormat, int step = 3)
		{
			BitmapData data = source.LockBits(new Rectangle(0, 0, source.Width, source.Height),
				ImageLockMode.ReadWrite, pixelFormat);

			int length = Math.Abs(data.Stride) * source.Height;


			{
				byte* rgbValues = (byte*)data.Scan0.ToPointer();

				for (int i = 0; i < length; i += step)
				{
					byte dummy = rgbValues[i];
					rgbValues[i] = rgbValues[i + 2];
					rgbValues[i + 2] = dummy;
				}
			}

			source.UnlockBits(data);
			return source;
		}
		public static Bitmap ToBitmap(this byte[] data, int width, int height, PixelFormat pixelFormat = PixelFormat.Format24bppRgb)
		{
			Bitmap bmp = new Bitmap(width, height, pixelFormat);
			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, bmp.PixelFormat);
			Marshal.Copy(data, 0, bmpData.Scan0, data.Length);
			bmp.UnlockBits(bmpData);

			return bmp;
		}
	}
}
*/
