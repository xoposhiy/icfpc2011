using System;
using NUnit.Framework;

namespace Contest
{
	[TestFixture]
	public class HealerTests
	{
		[Test]
		public void ShowHealerPlan()
		{
			var healerPlan = Primitives.GetHealerPlan(3, "succ(dbl(succ(zero)))");
			Console.WriteLine(healerPlan);
			Console.WriteLine(healerPlan.SplitByLineFeeds().Length);
		}
	}
}