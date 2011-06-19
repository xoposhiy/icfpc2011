using System;
using System.Collections.Generic;
using System.Linq;

namespace Contest
{
	public class Primitives : Funcs
	{
		private readonly World w;

		public Primitives(World w)
		{
			this.w = w;
		}

		public IEnumerable<Move> Attack0()
		{
			// S (get) (S dec I) : zero
			// S (S (dec) (put)) (get) : zero
			yield return new Move(0, Get);
			yield return new Move(S, 0);
			yield return new Move(K, 0);
			yield return new Move(S, 0);
			yield return new Move(K, 0);
			yield return new Move(S, 0);
			yield return new Move(0, S);
			yield return new Move(0, Dec);
			yield return new Move(0, I);
			yield return new Move(0, Zero);
		}

		public IEnumerable<Move> AttackMany()
		{
			yield return new Move(0, K);
			yield return new Move(0, Get);
			yield return new Move(S, 0);
			yield return new Move(K, 0);
			yield return new Move(S, 0);
			yield return new Move(0, K);
			yield return new Move(0, Zero);
			yield return new Move(S, 0);
			yield return new Move(K, 0);
			yield return new Move(S, 0);
			yield return new Move(K, 0);
			yield return new Move(S, 0);
			yield return new Move(0, S);
			yield return new Move(0, Dec);
			yield return new Move(0, Succ);
			yield return new Move(0, Zero);
		}

		public IEnumerable<Move> ToMoves(string s)
		{
			return s.SplitByLineFeeds().Select(Move.Parse);
		}

		public static string killemallBootstrap = @"
put 0
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
K 0
S 0
0 get
K 0
S 0
0 I
K 0
S 0
0 succ
K 0
S 0
0 I
K 0
S 0
0 succ
K 0
S 0
0 I
0 zero
";
		public IEnumerable<Move> KillEmAll()
		{
			yield return new Move(2, Zero);
			for (var targetSlot = 0; targetSlot <= 255; targetSlot++)
			{
				var bootstrapping = ToMoves(killemallBootstrap + LoopSuffix);
				foreach (var move in bootstrapping)
					yield return move;
				while (w.opponent[255 - targetSlot].vitality > 0)
				{
					foreach (var move in ToMoves(Fire))
						yield return move;
					Log(w.ToString(false));
				}
				yield return new Move(Succ, 2);
			}
		}

		public IEnumerable<Move> TaranEmAll()
		{
			var moves = ToMoves(
				Repeat("dec", 330, 0) +
				@"
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
");
			foreach (var move in moves)
				yield return move;
			yield return new Move(1, Zero);
			for (var targetSlot = 0; targetSlot <= 255; targetSlot++)
			{
				while (w.opponent[255 - targetSlot].vitality > 0)
					yield return new Move(0, Zero);
				yield return new Move(Succ, 1);
			}
		}

		public string Repeat(string payload, int count, int slotNo)
		{
			string s = "";
			Action<string> addPlan = cmd => { s = s + cmd + "\n"; };
			addPlan(slotNo + " " + payload);
			addPlan("S " + slotNo);
			addPlan(slotNo + " put");
			for (int i = 1; i < count; i++)
			{
				addPlan("S " + slotNo);
				addPlan(slotNo + " " + payload);
			}
			return s;
		}

		private void Log(string message)
		{
			//File.AppendAllText("world.txt", message+ Environment.NewLine);
		}

		public static string Fire = @"
1 zero
get 1
1 zero
";

		public static string LoopSuffix = @"
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
";

		public IEnumerable<Move> SetSlot2To8192()
		{
			yield return new Move(2, Zero);
			yield return new Move(Succ, 2); //1
			for (int i = 0; i < 13; i++)
				yield return new Move(Dbl, 2);
		}

		public IEnumerable<Move> CreateZombies()
		{
			//v[2] = 8192
			foreach (var m in CreateZombie(4, "dbl(dbl(succ(zero)))")) yield return m;
			yield return new Move(Dbl, 2); //16384
			foreach (var m in CreateZombie(5, "succ(dbl(dbl(succ(zero))))")) yield return m;
			yield return new Move(Dbl, 2); //32768
			foreach (var m in CreateZombie(6, "dbl(succ(dbl(succ(zero))))")) yield return m;
		}

		private IEnumerable<Move> CreateZombie(int zombieSlotNo, string zombieSlot)
		{
			const string damageSlot = "dbl(succ(zero))"; //2
			var payload = string.Format("S(K(help(zero)(zero)))(K(get({0})))", damageSlot);
			var zombie = string.Format("S(K(zombie (zero))) ( K({0}) )", payload);
			var replicatingZombie = string.Format("S(K(S ({0})(get)))(K({1}))", zombie, zombieSlot);
			return ToMoves(ThePlan.MakePlan(zombieSlotNo, replicatingZombie));
		}

		public IEnumerable<Move> RunHealer()
		{
			//v[3] = Healer
			yield return new Move(7, Zero);
			yield return new Move(Succ, 7);
			yield return new Move(Succ, 7);
			yield return new Move(Succ, 7);	//slot[7]=3
			yield return new Move(Get, 7);	//slot[7]=slot[3]
			yield return new Move(7, Zero);	//run!
		}

		public IEnumerable<Move> CreateHealer()
		{
			return CreateHealer(3, "succ(dbl(succ(zero)))");
		}

		public IEnumerable<Move> CreateHealer(int healerSlotNo, string healerSlot)
		{
			return ToMoves(GetHealerPlan(healerSlotNo, healerSlot));
		}

		public static string GetHealerPlan(int healerSlotNo, string healerSlot)
		{
			//v[2] = 8192
			const string damageSlot = "dbl(succ(zero))"; //2
			var healer = string.Format("S(K(help(zero)(zero)))(K(get({0})))", damageSlot);
			healer = string.Format("S ({0}) (S(get)(I))", healer);
			healer = string.Format("S (K({0})) (K({1}))", healer, healerSlot);
			return ThePlan.MakePlan(healerSlotNo, healer);
		}
	}
}