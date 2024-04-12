using System.Xml;

namespace Multfinite.Utilities
{
	public static partial class Utilities
	{
		public static string? GetValue(this XmlNode node, string path)
		{
			var p = path.Split('.');
			XmlNode item = node;
			if(p.Length > 1)
			{
				for (int i = 0; i < p.Length; i++)
				{
					var name = p[i];
					item = item[name];
				}
			}
			return item.Value;
		}

		public static string GetInnerText(this XmlNode node, string path)
		{
			var p = path.Split('.');
			XmlNode item = node;
			if (p.Length > 1)
			{
				for (int i = 0; i < p.Length; i++)
				{
					var name = p[i];
					item = item[name];
				}
			}
			return item.InnerText;
		}

		public static List<KeyValuePair<string, string>> Dump(this XmlNode node, bool recursive = true, string prefix = "")
		{
			var items = new List<KeyValuePair<string, string>>();
			foreach(XmlNode item in node.ChildNodes)
			{
				if (item.ChildNodes.Count == 0)
				{
					item.Normalize();
					items.Add(new KeyValuePair<string, string>(
						string.IsNullOrWhiteSpace(prefix) ? item.ParentNode.Name : $"{prefix}.{item.ParentNode.Name}",
						item.InnerText
					));
				}

			}
			if(recursive)
			{
				var p = string.IsNullOrWhiteSpace(prefix) ? node.Name : $"{prefix}.{node.Name}";
				foreach (XmlNode item in node.ChildNodes)
					if (item.ChildNodes.Count > 0)
						items.AddRange(item.Dump(recursive, p));
			}
			return items;
		}
	}
}
