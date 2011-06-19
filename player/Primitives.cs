using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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


		public IEnumerable<Move> AttackEmAll(int attackerSlot, int targetSlot, int damageSlot)
		{
			foreach (var m in CreateAttackerIfNeeded(attackerSlot, targetSlot.ToForm(), damageSlot.ToForm()))
				yield return m;
			yield return new Move(Put, targetSlot);
			yield return new Move(targetSlot, Zero);
			for (var target = 0; target <= 255; target++)
			{
				while (w.opponent[255 - target].vitality > 0)
				{
					if (w.me[0].vitality < 32768) yield return null; // Не угробить бы себя...
					yield return new Move(attackerSlot, Zero);
				}
				yield return new Move(Succ, targetSlot);
			}
		}

		private bool attackerCreated = false;
		private IEnumerable<Move> CreateAttackerIfNeeded(int slotNo, string targetSlot, string damageSlot)
		{
			if (attackerCreated) return new Move[0];
			attackerCreated = true;
			var attack_I_I_D = Form.CreateDelayedAttacker(targetSlot, damageSlot);
			var plan = ThePlan.MakePlan(slotNo, Form.AddSelfReproducing(slotNo, attack_I_I_D));
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

		public IEnumerable<Move> RunHealer(int healerHomeSlot)
		{
			//slot[0] = Healer
			//slot[healerHomeSlot] = I
			yield return new Move(healerHomeSlot, Zero);
			yield return new Move(Get, healerHomeSlot);		//slot[healerHomeSlot] = slot[0]
			yield return new Move(healerHomeSlot, Zero);	//run!
		}

		public IEnumerable<Move> CreateHealer(int healerProtoSlot, int healerTargetSlot, int healerDamageSlot)
		{
			var healer = Form.CreateHealer(healerTargetSlot.ToForm(), healerDamageSlot.ToForm(), healerProtoSlot.ToForm());
			var plan = ThePlan.MakePlan(healerProtoSlot, healer);
			return ToMoves(plan);
		}
	}
}