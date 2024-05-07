using Newtonsoft.Json.Linq;

namespace Multfinite.Utilities
{
	public static partial class Utilities
	{
		public static JObject ExpandInheritance(this JObject o, string ancestorKey = "_inherit")
		{
			var a = new List<string>();

			foreach(var item in o)
			{
				var io = item.Value as JObject;
				if (!io.ContainsKey(ancestorKey))
					a.Add(item.Key);
			}

			while(a.Count != o.Count)
			{
				foreach (var item in o)
				{
					var io = item.Value as JObject;
					if (io.ContainsKey(ancestorKey))
					{
						List<string> ancestors = new List<string>();
						var ancestorProp = io[ancestorKey];
						if (ancestorProp is JArray array)
							ancestors.AddRange(array.ToArray().ToList().ConvertAll((x) => x.ToString()));
						else if (ancestorProp is JValue value)
							ancestors.Add(value.ToString());

						ancestors.Reverse(); // the last item has the lowest priority

						bool allAvaliable = true; // All ancestors should be processed
						foreach(var ancestor in ancestors)
						{
							if (!a.Contains(ancestor))
							{
								if (!o.ContainsKey(ancestor))
									throw new ArgumentException($"<{ancestor}> is missing in root object");
								allAvaliable = false;
								break;
							}
						}
						if (!allAvaliable)
							continue;

						var obj = new JObject();
						foreach (var ancestor in ancestors)
						{
							var objBase = (o[ancestor] as JObject).DeepClone() as JObject;
							//objBase.Remove(ancestorKey); // ANCESTOR SHOULD NEVER HAVE THIS KEY
							obj.Merge(objBase);							
						}
						obj.Merge(io);
						obj.Remove(ancestorKey);
						o[item.Key] = obj;
						a.Add(item.Key);
					}
				}
			}
			return o;
		}
	}
}
