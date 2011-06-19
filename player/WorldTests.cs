using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using System.Linq;

namespace Contest
{
	[TestFixture]
	public class WorldTests : Form
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
		public void FormFromNum()
		{

			for (int i = 0; i < 10;i++)
				Console.WriteLine(i + " = " + i.ToForm());
		}

		[Test]
		public void Bug()
		{
			world.RunMyForm(0, "S (S) (I)");
			world.RunMyPlan("0 I");
		}

		[Test]
		public void HeallerBug()
		{
			world.me[1].value = new Num(0);
			world.me[2].value = new Num(8192);
			var healer = CreateHealer(1.ToForm(), 2.ToForm(), 0.ToForm());
			var p = ThePlan.MakePlan(0, healer);
			foreach (var line in Primitives.ToMoves(p).Select(m => m.ToOrgString()))
			{
				Console.Write(line);
			}
			Console.WriteLine();
			Run(p);
			Run("0 zero");
			Console.WriteLine(world.me[0]);
		}		
		
		[Test]
		public void HeallerBug2()
		{
			var healer = AddCycling("inc", "zero");
			var p = ThePlan.MakePlan(0, healer);
			foreach (var line in Primitives.ToMoves(p).Select(m => m.ToOrgString()))
			{
				Console.Write(line);
			}
			Run(p);
			Run("0 zero");
			Console.WriteLine(world.me[0]);
		}

		[Test]
		public void Healer()
		{
			var healer = CreateHealer(1.ToForm(), 2.ToForm(), 0.ToForm());
			var healerPlan = ThePlan.MakePlan(0, healer);
			Console.WriteLine(healerPlan.SplitByLineFeeds().Length);
			Console.WriteLine(healerPlan);
			AddPlan("1 zero");//target = 0
			world.me[1].value = new Num(0);
			world.me[2].value = new Num(8192);
			AddPlan(healerPlan);//healer constructed in slot 0
			AddPlan("7 zero");
			AddPlan("get 7");//clone healer
			AddPlan("7 zero");//run healer
			Run(plan);
		}

		[Test]
		public void Healer255()
		{
			var healer = CreateHealer255(9, 2.ToForm(), 8.ToForm());
			var healerPlan = ThePlan.MakePlan(9, healer);
			Console.WriteLine(healerPlan.SplitByLineFeeds().Length);
			Console.WriteLine(healerPlan);
			AddPlan(ThePlan.MakePlan(8, 255.ToForm()));//set target
			AddPlan(ThePlan.MakePlan(2, 2048.ToForm()));//set damage
			AddPlan(healerPlan);//healer constructed in slot 9
			AddPlan("9 zero");//run healer
			Run(plan);
		}
		
		[Test]
		public void Attacker()
		{
			
			var attacker = AddSelfReproducing(2, CreateDelayedAttacker("zero", "succ(zero)"));

			var attackerPlan = ThePlan.MakePlan(2, attacker);
			Console.WriteLine(attackerPlan.SplitByLineFeeds().Length);
			AddPlan(attackerPlan);
			AddPlan("0 zero");
			AddPlan("succ 0");
			world.me[0].vitality = 65535;
			AddPlan(ThePlan.MakePlan(1, (2*4096).ToForm()));
			AddPlan("2 zero");
			AddPlan("2 zero");
			AddPlan("2 zero");
			AddPlan("2 zero");
			Run(plan);
		}

		[Test]
		public void TestPlan()
		{
			Console.WriteLine(ThePlan.MakePlan(0, "S( K ( dec)) ( K ( get (succ (succ(zero))) ) )").SplitByLineFeeds().Count());
		}

		[Test]
		public void MassRevive()
		{
			var hostingSlotNo = 10;
			//var payload = "S(S(revive inc)(inc))";
			var payload = "revive";
			//plan = ThePlan.MakePlan(hostingSlotNo, AddIncCycling(hostingSlotNo, payload));
			plan = ThePlan.MakePlan(hostingSlotNo, AddSelfReproducing(hostingSlotNo, Form.DelayApplication(Form.Repeat("revive", "inc", 50), "zero", false, false)));
			Console.WriteLine(plan.SplitByLineFeeds().Count());
			Console.WriteLine(plan);

			Run(plan);
			for (int i = 0; i < 255; i++)
			{
				world.me[i].vitality = 0;
			}
			world.me[0].vitality = 0;
			world.me[hostingSlotNo].vitality = 10;
			Run(hostingSlotNo + " zero");
			Run(hostingSlotNo + " zero");
			Run(hostingSlotNo + " zero");
			Run(hostingSlotNo + " zero");
			Run(hostingSlotNo + " zero");
		}

		[Test]
		public void UberZombie()
		{
			world.opponent[0].vitality = 65535;
			world.opponent[255].vitality = 0; //dead opponent slot
			world.me[2].value = new Num(8192); //damage

			var uberZombiePayload = CreateUberZombiePayload(2);
			plan = ThePlan.MakePlan(3, uberZombiePayload);
			world.RunMyPlan(plan, silent:true);
			Console.WriteLine(plan.SplitByLineFeeds().Count());
			Console.WriteLine(plan);
			return;

			var zombie4 = Create4Zombie(5, 3, 0);
			plan = ThePlan.MakePlan(5, zombie4);
			Console.WriteLine(plan.SplitByLineFeeds().Count());
			Console.WriteLine(plan);
			Run(plan);
			Run("5 zero");
			world.OpponentTurn(new Move(Funcs.I, 0));
			Console.WriteLine(world.ToString());
			return;
		}

		[Test]
		public void TestNum()
		{
			Console.WriteLine(ThePlan.MakePlan(0, 255.ToForm()).SplitByLineFeeds().Count());
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
			var lines = s.SplitByLineFeeds();
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
			var lines = s.SplitByLineFeeds();
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