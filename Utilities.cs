using System.Text.RegularExpressions;

namespace Multfinite.Utilities
{
	public static partial class Utilities
	{
		public static string RemoveRootDirectoryInFilename(this string fileName, string rootDirectory)
		{
			int length = fileName.IndexOf(rootDirectory) + rootDirectory.Length + 1;
			fileName.Remove(0, length);
			return fileName;
		}

		public static DirectoryInfo GetSubdirectory(this DirectoryInfo dir, string subdir, bool create = false)
		{
			if (string.IsNullOrWhiteSpace(subdir))
				return dir;
			var d = dir.GetDirectories().ToList().Find((x) => x.Name.ToLower() == subdir.ToLower());
			if (d == null)
				if (create)
					d = dir.CreateSubdirectory(subdir);
				else throw new DirectoryNotFoundException(Path.Combine(dir.FullName, subdir));
			return d;
		}

		public static DirectoryInfo GetDirectory(this string dir, DirectoryInfo rootDir = null, bool create = false)
		{
			if (rootDir == null)
				rootDir = new DirectoryInfo(Environment.CurrentDirectory);
			if (string.IsNullOrWhiteSpace(dir))
				return rootDir;
			var d = Path.IsPathRooted(dir) ? new DirectoryInfo(dir) : rootDir.GetSubdirectory(dir, create);
			if (!d.Exists)
				d.Create();
			return d;
		}

		public static void SplitFileRegex(this string src, out string dir, out Regex regex)
		{
			if(src.Contains('/'))
			{
				var separatorIndex = src.LastIndexOf('/');
				dir = src.Substring(0, separatorIndex);
				var r = src.Substring(separatorIndex + 1, src.Length - (separatorIndex + 1));
				regex = new Regex(r, RegexOptions.IgnoreCase);
			}
			else
			{
				dir = "";
				regex = new Regex(src);
			}
		}

		public static List<FileInfo> GetFiles(this DirectoryInfo dir, Regex pattern)
		{
			var files = new List<FileInfo>();
			foreach (var file in dir.GetFiles())
				if (pattern.IsMatch(file.Name))
					files.Add(file);
			return files;
		}

		public static string[] GetFiles(this string dir, bool includeSubdirs)
		{
			List<string> files = new List<string>();
			files.AddRange(Directory.GetFiles(dir));
			if (includeSubdirs)
			{
				string[] dirs = Directory.GetDirectories(dir);
				foreach (string subdir in dirs)
				{
					files.AddRange(GetFiles(subdir, true));
				}
			}

			for (int i = 0; i < files.Count; i++)
				files[i] = files[i].RemoveRootDirectoryInFilename(dir);

			return files.ToArray();
		}

		public static List<string> ReadText(this IEnumerable<FileInfo> files)
		{
			var data = new List<string>();
			foreach (var file in files)
				data.Add(File.ReadAllText(file.FullName));
			return data;
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

		public static string AsYesNo(this bool value) => value ? "yes" : "no";
		public static int AsDigit(this bool value) => value ? 1 : 0;

		public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0) return min;
			else if (val.CompareTo(max) > 0) return max;
			else return val;
		}
		public static bool CheckClamp<T>(this T val, T min, T max) where T : IComparable<T>
		{
			if (val.CompareTo(min) < 0) return false;
			else if (val.CompareTo(max) > 0) return false;
			else return true;
		}

		public static bool IsInhertInterface<T>(this T val, Type interfaceType) => val.GetType().GetInterfaces().Contains(interfaceType);

		public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
		public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
	}

	public class Pair<K, V>
	{
		public K Key;
		public V Value;

		public Pair() { }
		public Pair(K key, V value)
		{
			Key = key;
			Value = value;
		}
	}
}
