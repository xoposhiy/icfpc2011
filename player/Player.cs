using System;
using System.Collections.Generic;

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

		public IEnumerable<Move> AttackAlive()
		{
			yield break;
		}
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
			while (true)
			{
				foreach (Move move in p.AttackMany()) yield return move;
			}
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