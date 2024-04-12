using System.Collections;

namespace Multfinite.Utilities
{
	public static partial class Utilities
	{
		public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> copyFrom)
		{ foreach (var kvp in copyFrom) dictionary.Add(kvp); }

		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> copyFrom)
		{
			lock (collection) lock (copyFrom)
				{ foreach (var obj in copyFrom) collection.Add(obj); }
		}

		public static IDictionary<K, V> Merge<K, V>(this IDictionary<K, V> to, IDictionary<K, V> from)
		{
			foreach (var kvp in from)
				to.Set(kvp.Key, kvp.Value);
			return to;
		}

		public static bool IsSameContent<T>(this IEnumerable<T> a, IEnumerable<T> b)
		{
			if (a.Count() != b.Count())
				return false;
			foreach (var item in a)
				if (!b.Contains(item))
					return false;
			return true;
		}

		public static IDictionary<TKey, TValue> Set<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
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

		public static IDictionary<TKey, TValue> Counter<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue step)
			where TKey : notnull
		{
			if(dict.ContainsKey(key))
				dict[key] = (dynamic) dict[key] + step;
			else dict.Add(key, (dynamic) step);
			return dict;
		}

		public static List<List<T>> Divide<T>(this IEnumerable<T> src, int count)
		{
			var r = new List<List<T>>();
			for (int i = 0; i < count; i++)
				r.Add(new List<T>());
			for (int i = 0; i < src.Count(); i++)
			{
				var item = src.ElementAt(i);
				var index = i % count;
				r[index].Add(item);
			}
			return r;
		}

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

		public static bool Contains<T>(this IEnumerable<T> source, Predicate<T> seekDelegate, out T obj)
		{
			foreach (var item in source)
			{
				if (seekDelegate.Invoke(item))
				{
					obj = item;
					return true;
				}
			}
			obj = default(T);
			return false;
		}
		public static bool Contains<T>(this IEnumerable<T> source, Predicate<T> seekDelegate, out ICollection<T> objs)
		{
			objs = new List<T>();
			foreach (var item in source) { if (seekDelegate.Invoke(item)) objs.Add(item); }
			return objs.Count > 0;
		}

		public static ICollection<T> Sort<T>(this ICollection<T> source, IComparer<T> comparer)
		{
			if (source == null || source.Count() <= 1) return source;
			List<T> list = new List<T>(source);
			list.Sort(comparer);
			source.Clear();
			source.AddRange(list);
			return source;
		}

		public static ICollection<T> InitRange<T>(this ICollection<T> source, int count)
		{
			for (int i = 0; i < count; i++)
				source.Add(default(T));
			return source;
		}
		public static ICollection<T> InitRange<T>(this ICollection<T> source, int count, Func<int, T> initializer)
		{
			for (int i = 0; i < count; i++)
				source.Add(initializer(i));
			return source;
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

		public static IEnumerable<T> SubItems<T>(this IEnumerable<T> source, int startIndex, int count)
		{
			{
				int sourceCount = source.Count();
				if (sourceCount < (count - 1) + startIndex) throw new InvalidOperationException("Source collection count must be more than last copy element index(i.e. count + startIndex)");
			}

			List<T> buffer = new List<T>();
			{
				int i = 0;
				int c = 0;
				foreach (T item in source)
				{
					if (i >= startIndex && c < count)
					{
						buffer.Add(item);
						c++;
					}
					i++;
				}
			}
			return buffer;
		}

		public static Dictionary<T, int> CountEntries<T>(this IEnumerable<T> items)
		{
			var counts = new Dictionary<T, int>();
			foreach(var item in items)
			{
				if(counts.ContainsKey(item))
					counts[item]++;
				else
					counts[item] = 1;
			}
			return counts;
		}

		public static List<T> ToList<T>(this IEnumerator src)
		{
			var list = new List<T>();
			src.Reset();
			while (src.MoveNext())
				list.Add((T) src.Current);
			return list;
		}
	}
}
