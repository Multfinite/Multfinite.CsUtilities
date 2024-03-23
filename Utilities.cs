using System.Text.RegularExpressions;

namespace Multfinite.Utilities
{
	public static partial class Utilities
	{
		public static List<FileInfo> GetFiles(this DirectoryInfo dir, Regex pattern)
		{
			var files = new List<FileInfo>();
			foreach (var file in dir.GetFiles())
				if (pattern.IsMatch(file.Name))
					files.Add(file);
			return files;
		}

		public static List<string> ReadText(this IEnumerable<FileInfo> files)
		{
			var data = new List<string>();
			foreach (var file in files)
				data.Add(File.ReadAllText(file.FullName));
			return data;
		}

		public static List<T> RemoveNullEntries<T>(this List<T> list)
		{
			list.RemoveAll((item) => item == null);
			return list;
		}

		public static T? Has<T>(this IEnumerable<T> items, Predicate<T> predicate) where T : class?
		{
			foreach (T item in items)
				if (predicate(item))
					return item;
			return null;
		}

		public delegate void CancelableAction<T>(T value, ref bool cancel);

		public static void Iterate<T>(this IEnumerable<IEnumerable<T>> collections, Action<T> handler)
		{
			foreach (var collection in collections)
				foreach (var item in collection)
					handler(item);
		}

		public static void Iterate<T>(this IEnumerable<IEnumerable<T>> collections, CancelableAction<T> handler)
		{
			bool cancel = false;
			foreach (var collection in collections)
				foreach (var item in collection)
				{
					handler(item, ref cancel);
					if (cancel)
						break;
				}
		}

		public static void ForEach<T>(this IEnumerable<T> collection, CancelableAction<T> handler)
		{
			bool cancel = false;
			foreach (var item in collection)
			{
				handler(item, ref cancel);
				if (cancel)
					break;
			}
		}

		public static Dictionary<TKey, TValue> Set<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
		{
			/*
			if(dict.ContainsKey(key))
				dict[key] = value;
			else dict.Add(key, value);
			*/
			try
			{
				dict[key] = value;
			}
			catch (KeyNotFoundException) { }
			return dict;
		}

		public static Dictionary<TKey, TValue> Counter<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue step)
			where TKey : notnull
		{
			if(dict.ContainsKey(key))
				dict[key] = (dynamic) dict[key] + step;
			else dict.Add(key, (dynamic) step);
			return dict;
		}

		public static string RemoveSharedPathParts(this string path, string sharedPath)
		{
			var pathSplitten = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
			var sharedPathSplitten = sharedPath.Split("/", StringSplitOptions.RemoveEmptyEntries);
			var shared = new List<string>();
			for (var i = 0; i < Math.Min(pathSplitten.Length, sharedPathSplitten.Length); i++)
				if (pathSplitten[i] == sharedPathSplitten[i])
					shared.Add(pathSplitten[i]);
				else break;

			if (shared.Count == 0)
				throw new ArgumentException("There is not shared path parts");

			var unique = "";
			for (var i = shared.Count; i < pathSplitten.Length; i++)
				unique = Path.Combine(unique, pathSplitten[i]);
			return unique;
		}

		public static List<List<T>> Divide<T>(this IEnumerable<T> src, int count)
		{
			var r = new List<List<T>>();
			for (int i = 0; i < count; i++)
				r.Add(new List<T>());
			for(int i = 0; i < src.Count(); i++)
			{
				var item = src.ElementAt(i);
				var index = i % count;
				r[index].Add(item);
			}
			return r;
		}

		public static void ForEachMultithreaded<T>(this IEnumerable<T> collections, Action<T> handler, int count)
		{
			var tasks = collections.Divide(count);
			List<Thread> threads = new List<Thread>();
			for (int i = 0; i < tasks.Count; i++)
			{
				int taskIndex = i;
				var t = new Thread(() =>
				{
					var items = tasks[taskIndex];
					foreach (var item in items)
						handler(item);
				});
				threads.Add(t);
			}
			foreach (var t in threads)
				t.Start();
			foreach (var t in threads)
				t.Join();
		}

		public static bool IsSameContent<T>(this IEnumerable<T> a, IEnumerable<T> b)
		{
			if(a.Count() != b.Count())
				return false;
			foreach (var item in a)
				if(!b.Contains(item))
					return false;
			return true;
		}

		/// <summary>
		/// https://stackoverflow.com/a/57058345
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="source"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static IEnumerable<T[]> Combinations<T>(this IEnumerable<T> source)
		{
			if (null == source)
				throw new ArgumentNullException(nameof(source));

			T[] data = source.ToArray();

			return Enumerable
			  .Range(0, 1 << (data.Length))
			  .Select(index => data
				 .Where((v, i) => (index & (1 << i)) != 0)
				 .ToArray());
		}

		public static string AsYesNo(this bool value) => value ? "yes" : "no";
		public static int AsDigit(this bool value) => value ? 1 : 0;
	}
}
