using System;

namespace Contest
{
	public static partial class Extensions
	{
		public static string[] SplitByLineFeeds(this string s)
		{
			return s.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}