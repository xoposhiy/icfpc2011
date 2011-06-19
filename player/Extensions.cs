using System;

namespace Contest
{
	public static partial class Extensions
	{
		public static string ToForm(this int num)
		{
			if (num == 0) return "zero";
			string prefix = "";
			string suffix = "";
			while (num > 1)
			{
				if (num % 2 == 1)
				{
					prefix = prefix + "succ(";
					suffix = suffix + ")";
				}
				num /= 2;
				if (num > 0)
				{
					prefix = prefix + "dbl(";
					suffix = suffix + ")";
				}
			}
			return prefix + "succ(zero)" + suffix;
		}

		public static string[] SplitByLineFeeds(this string s)
		{
			return s.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}