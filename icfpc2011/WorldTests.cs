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
				Console.WriteLine(world.ToString(turn));
				var move = RunLine(line, only || turn);
				if (only) world.MyTurn(move);
				else
				{
					if (turn) world.MyTurn(move);
					else world.OpponentTurn(move);
				}
				turn = !turn;
			}
		}

		private static Move RunLine(string line, bool turn)
		{
			Console.WriteLine((turn ? "me" : "he") + "> " + line);
			var parts = line.Split(' ');
			Move move;
			if (parts[0][0] >= '0' && parts[0][0] <= '9')
			{
				move = new Move(int.Parse(parts[0]), Funcs.Parse(parts[1]));
			}
			else
				move = new Move(Funcs.Parse(parts[0]), int.Parse(parts[1]));
			return move;
		}
	}
}