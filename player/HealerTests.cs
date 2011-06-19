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
			var healerPlan = GetHealerPlan0("succ(dbl(succ(zero)))");
			Console.WriteLine(healerPlan);
			Console.WriteLine(healerPlan.SplitByLineFeeds().Length);
		}

		public static string GetHealerPlan0(string healerSlot)
		{
			const string damageSlot = "dbl(succ(zero))"; //2
			var healer = string.Format("S(K(help(zero)(zero)))(K(get({0})))", damageSlot);
			healer = string.Format("S ({0}) (S(get)(I))", healer);
			healer = string.Format("S (K({0})) (K({1}))", healer, healerSlot);
			return healer;
		}
	}
}