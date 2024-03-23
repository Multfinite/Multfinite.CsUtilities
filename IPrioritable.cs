namespace Multfinite.Utilities
{
	public interface IPrioritable
	{
		public class Comparer : IComparer<IPrioritable>
		{
			public static readonly Comparer Instance = new Comparer();

			public int Compare(IPrioritable? x, IPrioritable? y)
			{
				if (x == null && y == null) 
					return 0;
				if (x == null)
					return -y.Priority;
				if (y == null)
					return x.Priority;
				return x.Priority - y.Priority;
			}
		}

		public static int Compare(IPrioritable? x, IPrioritable? y)
		{
			return Comparer.Instance.Compare(x, y);
		}

		public int Priority { get; set; }
	}
}
