using System;
using System.Collections.Generic;
using System.IO;

namespace Contest
{
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
			foreach (Move move in p.CreateZombies())
				yield return move;
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
					if (w.opponent[255].vitality == 0)
					{
						if (w.opponent[0].vitality >= 32768) yield return MakeMyTurn(new Move(6, Funcs.Zero));
						else if (w.opponent[0].vitality >= 16384) yield return MakeMyTurn(new Move(5, Funcs.Zero));
						else if (w.opponent[0].vitality >= 8192) yield return MakeMyTurn(new Move(4, Funcs.Zero));
					}
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