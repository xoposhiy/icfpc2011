using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using System.Linq;

namespace Contest
{
	[TestFixture]
	public class WorldTests
	{
		private World world;
		private string plan;

		[SetUp]
		public void SetUp()
		{
			world = new World();
			plan = "";
		}

	
		[Test]
		public void TestPlan()
		{
			Console.WriteLine(ThePlan.MakePlan(0, "S( K ( dec)) ( K ( get (succ (succ(zero))) ) )"));
		}
		
		[Test]
		public void T()
		{
			Console.WriteLine(ThePlan.MakePlan(0, "S(K(dec))(K(zero))"));
		}
		[Test]
		public void T2()
		{
			Run(@"
2 zero
0 S
K 0
S 0
0 K
K 0
S 0
0 I
0 dec
K 0
S 0
0 K
K 0
S 0
0 I
0 zero
K 0
S 0
0 I
S 0
K 0
S 0
K 0
S 0
0 S
0 get
0 I
1 zero
get 1
1 zero
1 zero
get 1
1 zero
1 zero
get 1
1 zero
1 zero
get 1
1 zero
");
		}

		public string Repeat(string payload, int count, int slotNo)
		{
			string s = "";
			Func<string, string> AddPlan = cmd => s + cmd + Environment.NewLine;
			AddPlan(slotNo + " " + payload);
			AddPlan("S " + slotNo);
			AddPlan(slotNo + " put");
			for (int i = 1; i < count; i++)
			{
				AddPlan("S " + slotNo);
				AddPlan(slotNo + " " + payload);
			}
			return s;
		}

		public void Apply(string f, int targetSlot, int slot)
		{
			//S (S (K get) (K targetSlot))
			
		}

		public void Loop(int slotNo)
		{
			AddPlan("K " + slotNo);
			AddPlan("S " + slotNo);
			AddPlan("K " + slotNo);
			AddPlan("S " + slotNo);
			AddPlan(slotNo + " S");
			AddPlan(slotNo + " get");
			AddPlan(slotNo + " I");
			AddPlan(slotNo + " zero");
		}

		[Test]
		public void TestDoubleCycle()
		{
			plan += Repeat("dec", 5, 0);
			Loop(0);
			Run(plan);
			Console.WriteLine(plan);
		}

		[Test]
		public void TestPlan2()
		{
			AddPlan("0 zero");
			AddPlan("succ 0");
			AddPlan("dbl 0");
			AddPlan("1 zero");
			AddPlan("succ 1");
			AddPlan("dbl 1");
			AddPlan("dbl 1");
			AddPlan("dbl 1");
			AddPlan("dbl 1");
			AddPlan("dbl 1");
			AddPlan("dbl 1");
			AddPlan("dbl 1");
			AddPlan("dbl 1");
			AddPlan("dbl 1");
			AddPlan(ThePlan.MakePlan(3, "(help (get ( succ(zero))) (get ( succ(zero))) (get ( succ (zero)))) "));
			Run(plan);
			Console.WriteLine("Plan length: " + plan.Split('\n').Count());
		}

		private void AddPlan(string s)
		{
			plan += s + "\r\n";
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
		public void TestTara()
		{
			Console.WriteLine(ThePlan.MakePlanForTail(0, "S (K (dec))"));
			Console.WriteLine();
			Console.WriteLine(ThePlan.MakePlanForTail(0, "S (K(get)) (succ)"));
			Console.WriteLine("!");
			plan += Repeat("dec", 200, 0);
			Run(plan + @"
K 0
S 0
K 0
S 0
K 0
S 0
0 S
K 0
S 0
0 I
K 0
S 0
0 K
K 0
S 0
0 I
0 get
K 0
S 0
0 I
0 succ
S 0
0 get

1 zero
succ 1 //target cell

0 zero
0 zero
0 zero
0 zero

succ 1 //target cell

0 zero
0 zero
0 zero
");
		}
		[Test]
		public void TestTaran()
		{
			string s = @"
0 S
0 inc
0 inc
" +
Repeat(@"S 0
0 inc
", 331) +
@"S 0
0 get
0 zero
";
			Run(s);
			File.WriteAllText("toRun.txt", ConvertToOrgFormat(s));
		}

		[Test]
		public void TestHelp()
		{
			string s = @"
0 zero
succ 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
1 help
1 zero
1 zero
K 1
S 1
1 get
1 zero";
			Run(s);
			File.WriteAllText("toRun.txt", ConvertToOrgFormat(s));
		}

		[Test]
		public void TestHelp1()
		{
			string s = @"
0 zero
succ 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
dbl 0
1 help
1 zero
1 zero
K 1
S 1
1 get
1 zero";
			Run(s);
			File.WriteAllText("toRun.txt", ConvertToOrgFormat(s));
		}

		private string ConvertToOrgFormat(string s)
		{
			var res = "";
			var lines = s.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var line in lines)
			{
				var move = Move.Parse(line);
				res += move.ToOrgString();
			}
			return res;
		}

		private string Repeat(string s, int count)
		{
			var sb = new StringBuilder();
			for (int i = 0; i < count; i++)
				sb.Append(s);
			return sb.ToString();
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