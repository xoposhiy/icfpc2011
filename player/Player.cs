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

		private const int HealerPrototypeSlot = 0;
		private const int HealerTargetSlot = 1;
		private const int HealerDamageSlot = 2;
		private const int AttackerTargetSlot = 3;
		private const int AttackerDamageSlot = 4;
		
		private const int ZombieSlot = 5;
		private const int HealerHomeSlot = 20;
		private const int AttackerHomeSlot = 21;

		public IEnumerable<Move> MyMoves()
		{
			var healer_and_zombie_damage = 4096;
			foreach (var move in p.SetSlotToPowerOf2(HealerDamageSlot, healer_and_zombie_damage))
				yield return MakeMyTurn(move);
			foreach (var move in p.SetSlotToPowerOf2(AttackerDamageSlot, 4*4096))
				yield return MakeMyTurn(move);

			//init healer in slot3
			foreach (var move in p.CreateHealer())
				yield return MakeMyTurn(move);

			if (w.me[0].vitality < 32768)
			{
				foreach (var m in p.RunHealer())
					yield return MakeMyTurn(m);
			}

			foreach (var move in p.CreateZombie(ZombieSlot, HealerDamageSlot))
				yield return MakeMyTurn(move);

			while (true)
			{
				foreach (var move in p.TaranEmAll())
				{
					if (w.me[0].vitality < 32768)
					{
						foreach (var m in p.RunHealer())
							yield return MakeMyTurn(m);
					}
					if (w.opponent[255].vitality == 0)
					{
						Log("before zombie: w.opponent[0] = " + w.opponent[0]);
						if (w.opponent[0].vitality >= healer_and_zombie_damage)
						{
							yield return MakeMyTurn(new Move(ZombieSlot, Funcs.Zero));
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

	public static partial class Extensions
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