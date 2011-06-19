using System;
using NUnit.Framework;
using System.Linq;

namespace Contest
{
	[TestFixture]
	public class SetValueTests : Form
	{
		private Player p;
		[SetUp]
		public void SetUp()
		{
			p = new Player();
		}

		[Test]
		public void Test()
		{
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 255; j++)
					CheckValue(new Num(i), j);
			}
			for (int i = 0; i < 255; i++)
			{
				foreach (var f in Funcs.allFuncs)
				{
					CheckValue(f, i);
				}
			}
		}
		[Test]
		public void Test255()
		{
			for (int j = 0; j < 255; j++)
				CheckValue(new Num(j), 255);
			for (int i = 0; i < 255; i++)
			{
				foreach (var f in Funcs.allFuncs)
				{
					CheckValue(f, i);
				}
			}
		}
		[Test]
		public void Test255_2()
		{
			CheckValue(new Num(123), 255);
		}

		private void CheckValue(Value oldValue, int value)
		{
			p.w.me[0].value = oldValue;
			foreach (var m in p.p.SetSlotTo(0, value).Select(m => m.ToString()))
			{
				//Console.WriteLine(m);
				p.w.RunMyPlan(m, true);
			}
			//Console.WriteLine();
			Assert.AreEqual(value, p.w.me[0].value.AsNum(), oldValue.ToString() + " " + value);
		}
	}
}