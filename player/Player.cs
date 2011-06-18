using System;
using System.Collections.Generic;
using System.IO;
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
			var cmds = s.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			return cmds.Select(Move.Parse);
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
			Action<string> addPlan = cmd => { s = s + cmd + Environment.NewLine; };
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
	}

	public class Player
	{
		public Primitives p;
		public World w = new World();

		public Player()
		{
			p = new Primitives(w);
		}

		public void HisMove(Move m)
		{
			w.OpponentTurn(m);
		}

		public IEnumerable<Move> MyMoves()
		{
			int idx = 0;
			while (true)
			{
				foreach (Move move in p.TaranEmAll())
				{
					//if (++idx % 200 == 0)
					//{
					//    yield return MakeMyTurn(new Move(4, Funcs.Revive));
					//    yield return MakeMyTurn(new Move(4, Funcs.Zero));
					//}
					yield return MakeMyTurn(move);
				}
			}
		}

		private Move MakeMyTurn(Move move)
		{
			w.MyTurn(move);
			return move;
		}

		private Move M(string move)
		{
			return Move.Parse(move);
		}
	}

	public static class Extensions
	{
		public static void Each<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach (T item in items)
			{
				action(item);
			}
		}
	}
}