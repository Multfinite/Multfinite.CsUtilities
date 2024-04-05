using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multfinite.Utilities
{
	public static partial class Utilities
	{
		/// <summary>
		/// Null encoding = UTF8
		/// </summary>
		/// <param name="source"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string ToBase64(this string source, Encoding encoding = null)
		{
			if (String.IsNullOrEmpty(source)) return source;
			if (encoding == null) encoding = Encoding.UTF8;
			byte[] buffer = encoding.GetBytes(source);
			return Convert.ToBase64String(buffer);
		}
		/// <summary>
		/// Null encoding = UTF8
		/// </summary>
		/// <param name="source"></param>
		/// <param name="encoding"></param>
		/// <returns></returns>
		public static string FromBase64(this string source, Encoding encoding = null)
		{
			if (String.IsNullOrEmpty(source)) return source;
			if (encoding == null) encoding = Encoding.UTF8;
			byte[] buffer = Convert.FromBase64String(source);
			return encoding.GetString(buffer);
		}

		public static string ConvertString(string src, Encoding encodingIn, Encoding encodingOut)
		{
			byte[] b = encodingIn.GetBytes(src);
			b = Encoding.Convert(encodingIn, encodingOut, b);
			return encodingOut.GetString(b);
		}
	}
}
