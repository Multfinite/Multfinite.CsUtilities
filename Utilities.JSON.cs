using Newtonsoft.Json.Linq;

namespace Multfinite.Utilities
{
	public static partial class Utilities
	{
		public static JObject ExpandInheritance(this JObject o, string ancestorKey = "_inherit")
		{
			var a = new List<string>();
			var b = new List<string>();

			foreach(var item in o)
			{
				var io = item.Value as JObject;
				if (!io.ContainsKey(ancestorKey))
					a.Add(item.Key);
			}

			while(a.Count > 0)
			{
				foreach (var item in o)
				{
					var io = item.Value as JObject;
					if (io.ContainsKey(ancestorKey))
					{
						var ancestor = io[ancestorKey].ToString();
						if (!a.Contains(ancestor))
							continue;
						var ancestorObj = o[ancestor] as JObject;
						var merged = ancestorObj.DeepClone() as JObject;
						merged.Merge(io);
						merged.Remove(ancestorKey);
						o[item.Key] = merged;
						b.Add(item.Key);
					}
				}

				var c = a; a = b; b = c;
				b.Clear();
			}
			return o;
		}
	}
}
