namespace Multfinite.Utilities
{
	public class Entry<T> : IPrioritable, IPatterned
	{
		public int Priority { get; set; } = int.MinValue;
		public string Pattern { get; set; } = ".*";

		public T Value = default(T);

		public Entry() { }
		public Entry(T value)
		{
			Value = value;
		}

		public Entry(int priority, T value)
		{
			Priority = priority;
			Value = value;
		}

		public Entry(int priority, string pattern, T value)
		{
			Priority = priority;
			Pattern = pattern;
			Value = value;
		}
	}
}
