using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using System.Linq;

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

		[Test]
		public void TestLoop2()
		{
			string s = @"
0 get
S 0
K 0
S 0
K 0
S 0
0 S
0 inc
0 I
0 zero";
			Run(s);
		}


		[Test]
		public void UberTest()
		{
			var r = new Random(321654987);
			var cmds = Funcs.allFuncs.Select(f => f == Funcs.Zero ? "zero" : f.ToString()).ToArray();
			var inp = new StringBuilder();
			var inp2 = new StringBuilder();
			foreach (var cmd in cmds)
			{
				Console.WriteLine(cmd);
			}
			var ourOut = new StringBuilder();
			for (int i = 0; i < 1000; i++)
			{
				var cmd = cmds[r.Next(cmds.Length-1)];
				var slot = r.Next(256);
				Move m;
				if (r.Next(2) == 1)
				{
					inp.AppendFormat("1\n{0}\n{1}\n", cmd, slot);
					inp2.AppendFormat("{0} {1}\r\n", cmd, slot);
				}
				else
				{
					inp.AppendFormat("2\n{0}\n{1}\n", slot, cmd);
					inp2.AppendFormat("{0} {1}\r\n", slot, cmd);
				}
				ourOut.AppendLine(world.ToString(true));
			}
			Console.WriteLine();
			Console.WriteLine(world.ToString(true));
			File.WriteAllText(@"..\..\..\tests\nozombies.txt", inp.ToString());
			var contents = inp2.ToString();
			File.WriteAllText(@"..\..\..\tests\inp_my.txt", contents);
			Run(contents);
			File.WriteAllText(@"..\..\..\tests\out_my.txt", world.ToString(true));
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
			//Console.WriteLine(Function.r);
			Console.WriteLine(world);
		}
	}
}