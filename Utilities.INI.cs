using IniParser;
using IniParser.Model;
using System.Text;
using System.Text.RegularExpressions;

namespace Multfinite.Utilities
{
	public static partial class Utilities
	{
		public static List<IniData> ReadIni(this IEnumerable<FileInfo> files, FileIniDataParser parser)
		{
			var data = new List<IniData>();
			foreach (var file in files)
				data.Add(parser.ReadFile(file.FullName));
			return data;
		}

		public static List<IniData> ParseIni(this IEnumerable<string> contents, FileIniDataParser parser)
		{
			var data = new List<IniData>();
			foreach (var content in contents)
				data.Add(parser.Parser.Parse(content));
			return data;
		}

		public static string? GetTemplateInfoFromINIComments(this List<string> comments, bool removeTemplate = false)
		{
			var data = new List<string>(comments); // copy to prevent remove
			var sb = new StringBuilder();
			if (!data.Contains("#template"))
				return null;
			if (!data.Contains("#end"))
				throw new InvalidDataException("#end line not found.");

			int startIndex = data.IndexOf("#template");
			int endIndex = data.IndexOf("#end");
			if (removeTemplate)
				comments.RemoveRange(startIndex, endIndex - startIndex + 1);

			for (int i = startIndex + 1; i < endIndex; i++)
				sb.AppendLine(data[i]);
			for (int i = endIndex - 1; i > startIndex; i--)
				data.RemoveAt(i);

			return sb.ToString();
		}

		public static KeyData CreateKeyData(string key, string value) { var kd = new KeyData(key); kd.Value = value; return kd; }

		public static IniData Merge(this IEnumerable<IniData> collection)
		{
			IniData d = new IniData();
			foreach (var item in collection)
				d.Merge(item);
			return d;
		}

		public static void Iterate(this IEnumerable<IniData> collections, Action<SectionData> handler)
		{
			foreach (var collection in collections)
				foreach (var item in collection.Sections)
					handler(item);
		}

		public static void Iterate(this IEnumerable<IniData> collections, CancelableAction<SectionData> handler)
		{
			bool cancel = false;
			foreach (var collection in collections)
				foreach (var item in collection.Sections)
				{
					handler(item, ref cancel);
					if (cancel)
						return;
				}
		}

		public static void Iterate(this IEnumerable<SectionData> collections, Action<KeyData> handler)
		{
			foreach (var collection in collections)
				foreach (var item in collection.Keys)
					handler(item);
		}

		public static void Iterate(this IEnumerable<SectionData> collections, CancelableAction<KeyData> handler)
		{
			bool cancel = false;
			foreach (var collection in collections)
				foreach (var item in collection.Keys)
				{
					handler(item, ref cancel);
					if (cancel)
						return;
				}
		}

		public static SectionData FindSection(this IEnumerable<IniData> collection, string sectionName)
		{
			foreach (var item in collection)
				foreach (var s in item.Sections)
					if (s.SectionName == sectionName)
						return s;
			throw new KeyNotFoundException(sectionName);
		}

		public static SectionData FindSection(this IEnumerable<IniData> collection, Regex sectionRegex)
		{
			foreach (var item in collection)
				foreach (var s in item.Sections)
					if (sectionRegex.IsMatch(s.SectionName))
						return s;
			throw new KeyNotFoundException(sectionRegex.ToString());
		}

		public static List<SectionData> FindSections(this IEnumerable<IniData> collection, string sectionName)
		{
			var items = new List<SectionData>();
			collection.Iterate((s) =>
			{
				if(s.SectionName == sectionName)
					items.Add(s);
			});
			return items;
		}

		public static List<SectionData> FindSections(this IEnumerable<IniData> collection, Regex sectionRegex)
		{
			var items = new List<SectionData>();
			collection.Iterate((s) =>
			{
				if (sectionRegex.IsMatch(s.SectionName))
					items.Add(s);
			});
			return items;
		}

		public static List<SectionData> ExtractSections(this IEnumerable<IniData> collection, params string[] sectionNames)
		{
			var items = new List<SectionData>();
			collection.Iterate((s) =>
			{
				if (sectionNames.Contains(s.SectionName))
					items.Add(s);
			});
			return items;
		}

		public static List<SectionData> ExtractListedItems(this IEnumerable<IniData> collection, IEnumerable<SectionData> lists, bool skipEmptyOrNull = true)
		{
			var items = new List<SectionData>();
			lists.Iterate((itemName) =>
			{
				if (skipEmptyOrNull && string.IsNullOrEmpty(itemName.Value))
					return;

				var item = collection.FindSection(itemName.Value);
				items.Add(item);
			});
			return items;
		}

		public static List<SectionData> ExtractListedItems(this IEnumerable<IniData> collection, IEnumerable<SectionData> lists, out List<string> inListWithoutSection, bool skipEmptyOrNull = true)
		{
			var items = new List<SectionData>();
			var noSect = new List<string>();
			lists.Iterate((itemName) =>
			{
				if (skipEmptyOrNull && string.IsNullOrEmpty(itemName.Value))
					return;
				try
				{
					var item = collection.FindSection(itemName.Value);
					items.Add(item);
					
				}
				catch(KeyNotFoundException)
				{
					noSect.Add(itemName.Value);
				}				
			});
			inListWithoutSection = noSect;
			return items;
		}
	}
}
