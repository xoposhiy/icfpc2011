using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

		public static IEnumerable<Move> ToMoves(this int num, int slotNo)
		{
			if (num == 0) yield break;
			yield return new Move(Funcs.Succ, slotNo);
			var moves = new List<Move>();
			while (num > 1)
			{
				if (num % 2 == 1)
					moves.Add(new Move(Funcs.Succ, slotNo));
				moves.Add(new Move(Funcs.Dbl, slotNo));
				num /= 2;
			}
			foreach (var m in Enumerable.Reverse(moves))
				yield return m;
		}

		public static string[] SplitByLineFeeds(this string s)
		{
			return s.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}