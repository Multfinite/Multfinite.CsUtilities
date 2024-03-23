using System.Text.RegularExpressions;

namespace Multfinite.Utilities
{
	public interface IPatterned
	{
		public string Pattern { get; set; }
		public bool IsMatch(string key)
		{
			return new Regex(Pattern).IsMatch(key);
		}
	}

	public static partial class Utilities
	{
		public static List<TPatterned> Extract<TPatterned>(this IEnumerable<TPatterned> t, string key)
			where TPatterned : IPatterned
		{
			var f = new List<TPatterned>();
			foreach (TPatterned i in t)
				if(i.IsMatch(key))
					f.Add(i);
			return f;
		}
	}
}
