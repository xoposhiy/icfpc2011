using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Contest
{
	public class Player
	{
		public Primitives p;
		public World w = new World();

		public Player()
		{
			//Debugger.Launch();
			p = new Primitives(w);
		}

		public void HisMove(Move m)
		{
			w.OpponentTurn(m);
		}

		public IEnumerable<Move> MyMoves()
		{
			foreach (Move move in p.CreateZombies())
				yield return MakeMyTurn(move);
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
						Log("before zombie: w.opponent[0] = " + w.opponent[0]);
						if (w.opponent[0].vitality >= 32768) yield return MakeMyTurn(new Move(6, Funcs.Zero));
						else if (w.opponent[0].vitality >= 16384) yield return MakeMyTurn(new Move(5, Funcs.Zero));
						else if (w.opponent[0].vitality >= 8192)
						{
							yield return MakeMyTurn(new Move(4, Funcs.Zero));
							Log("after zombie: w.opponent[0] = " + w.opponent[0]);
						}
					}
					yield return MakeMyTurn(move);
				}
			}
		}

		private void Log(string message)
		{
			//File.AppendAllText("zombies.txt", message+ Environment.NewLine);
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