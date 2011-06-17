using System;
using NUnit.Framework;

namespace Icfpc2011
{
	[TestFixture]
	public class WorldTests
	{
		private World world;

		[SetUp]
		public void SetUp()
		{
			world = new World();
		}

		[Test]
		public void Test()
		{
			for (int i = 0; i < Funcs.allFuncs.Length; i++)
			{
				var res = world.MyTurn(new Move(i, Funcs.allFuncs[i]));
				Assert.AreEqual(Funcs.allFuncs[i], res);
			}
			Console.WriteLine(world);
		}
		
		[Test]
		public void TestBattle()
		{
			string s = @"
0 zero
0 inc
succ 0
0 zero
succ 0
0 dec
dbl 0
0 zero
inc 0
succ 0";
			Run(s, false);
			Console.WriteLine(world);
		}


	
		[Test]
		public void TestApplication()
		{
			string s = @"
0 help
0 zero
K 0
S 0
0 succ
0 zero
1 zero
succ 1
dbl 1
dbl 1
dbl 1
dbl 1
K 0
S 0
0 get
K 0
S 0
0 succ
0 zero";
			Run(s);
			Console.WriteLine(world);
		}
	
		[Test]
		public void TestApplication2()
		{
			string s = @"
0 help
0 zero
K 0
S 0
0 succ
0 zero
1 zero
succ 1
dbl 1
dbl 1
dbl 1
dbl 1
K 0
S 0
0 get
K 0
S 0
0 succ
0 zero";
			Run(s);
			Console.WriteLine(world);
		}
		
		[Test]
		public void TestLoop()
		{
			string s = @"
0 S
0 get
0 I
0 zero";
			Run(s);
		}

		private void Run(string s, bool only = true)
		{
			var lines = s.Split(new[]{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			var turn = true;
			foreach (var line in lines)
			{
				Console.WriteLine(world.ToString(only || turn));
				Console.WriteLine((only || turn ? "me" : "he") + "> " + line);
				var move = Move.Parse(line);
				if (only) world.MyTurn(move);
				else
				{
					if (turn) world.MyTurn(move);
					else world.OpponentTurn(move);
				}
				turn = !turn;
			}
		}
	}
}