using System;
using System.Collections.Generic;
using System.ComponentModel;
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


		public IEnumerable<Move> Attacker()
		{
			var targetSlot = "succ(zero)";
			var damageSlot = "succ(succ(zero))";
			int attackerSlot = 0;
			foreach (var m in CreateAttackerIfNeeded(attackerSlot, targetSlot, damageSlot))
			{
				yield return m;
			}
			if (w.me[0].vitality >= 32768)
			{
				//TODO Attack somebody
			}
		}

		private bool attackerCreated = false;
		private IEnumerable<Move> CreateAttackerIfNeeded(int slotNo, string targetSlot, string damageSlot)
		{
			if (attackerCreated) return new Move[0];
			var attack_I_I_D = Form.CreateDelayedAttacker(targetSlot, damageSlot);
			var plan = ThePlan.MakePlan(slotNo, attack_I_I_D);
			return ToMoves(plan);
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

		public IEnumerable<Move> SetSlotToPowerOf2(int slotNo, int valuePower2)
		{
			yield return new Move(slotNo, Zero);
			yield return new Move(Succ, slotNo);
			for (int i = 1; i < valuePower2; i *= 2)
				yield return new Move(Dbl, slotNo);
		}

		public IEnumerable<Move> CreateZombie(int zombieSlotNo, int damageSlot)
		{
			var payload = string.Format("S(K(help(zero)(zero)))(K(get({0})))", damageSlot.ToForm());
			var zombie = string.Format("S(K(zombie (zero))) ( K({0}) )", payload);
			var replicatingZombie = string.Format("S(K(S ({0})(get)))(K({1}))", zombie, zombieSlotNo.ToForm());
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

	public class Form
	{
		public static string DelayFun(string f)
		{
			return string.Format("K({0})", f);
		}

		public static string DelayApplication(string f, string arg, bool funIsDelayed, bool argIsDelayed)
		{
			if (!funIsDelayed) f = DelayFun(f);
			if (!argIsDelayed) arg = DelayFun(arg);
			return string.Format("S ({0}) ({1}) ", f, arg);
		}

		/// <summary>
		/// CreateDelayedAttacker(string targetSlot, string damageSlot) (_) -> attacks (get (targetSlot)) (get (targetSlot)) (get (damageSlot))
		/// </summary>
		public static string CreateDelayedAttacker(string targetSlot, string damageSlot)
		{
			var dellayedGetTarget = DelayApplication("get", targetSlot, false, false);
			string attack_I_I = DelayApplication("S (attack) (I)", dellayedGetTarget, false, true);
			var delayedGetDamage = DelayApplication("get", damageSlot, false, false);
			string attack_I_I_D = DelayApplication(attack_I_I, delayedGetDamage, true, true);
			return attack_I_I_D;
		}

		
	}
}