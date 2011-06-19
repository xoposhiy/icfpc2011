using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Contest
{
	public class Player
	{
		public Primitives p;
		public World w = new World();

		public Player()
		{
			p = new Primitives(w);
//			Debugger.Launch();
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
		private const int AttackerHomeSlot = 6;
		private const int HealerHomeSlot = 7;

		private const int HealerAndZombieDamage = 2*4096;
		private const int AttackerDamage = 4*4096;

		public IEnumerable<Move> MyMoves()
		{
			return DoMyMoves().Select(MakeMyTurn);
		}

		public IEnumerable<Move> DoMyMoves()
		{
			foreach (var move in p.SetSlotToPowerOf2(HealerDamageSlot, HealerAndZombieDamage))
				yield return move;
			foreach (var move in p.SetSlotToPowerOf2(AttackerDamageSlot, AttackerDamage))
				yield return move;

			foreach (var move in p.CreateHealer(HealerPrototypeSlot, HealerTargetSlot, HealerDamageSlot))
				yield return move;

			foreach (var move in HealAllIfNeeded()) yield return move;

//			foreach (var move in p.CreateZombie(ZombieSlot, HealerDamageSlot))
//				yield return move;

			while (true)
			{
				foreach (var move in p.AttackEmAll(AttackerHomeSlot, AttackerTargetSlot, AttackerDamageSlot))
				{
					bool wasMove = false;
					if (move != null)
					{
						yield return move;
						wasMove = true;
					}
//					foreach (var zm in MayBeFireZombie()) yield return zm;
					foreach (var hm in HealAllIfNeeded())
					{
						wasMove = true;
						yield return hm;
					}
					if (!wasMove) yield return new Move(Funcs.I, 0); //�� �������. ������ ����� ���, ����� �� �������� Time Limit!
				}
			}
		}

		private IEnumerable<Move> HealAllIfNeeded()
		{
			//foreach (var move in HealPatientIfNeeded(0)) yield return move;
			//foreach (var move in HealPatientIfNeeded(255)) yield return move;
			for (int patient = 0; patient < 8; patient++)
			{
				foreach (var move in HealPatientIfNeeded(patient)) yield return move;
			}
		}

		private IEnumerable<Move> HealPatientIfNeeded(int patient)
		{
			if (w.me[patient].vitality <= 32768 && w.me[patient].vitality > HealerAndZombieDamage)
			{
				foreach (var m in p.SetSlotTo(HealerTargetSlot, patient)) yield return m;
				foreach (var m in p.RunHealer(HealerHomeSlot)) yield return m;
			}
		}


		private IEnumerable<Move> HealZeroIfNeeded()
		{
			if (w.me[0].vitality < 32768) //TODO �������� ��������: "� ��� ������-�����-����, ������-����-���� � ������-������-���� ����!"
				foreach (var m in p.RunHealer(HealerHomeSlot)) yield return m;
		}

		private IEnumerable<Move> MayBeFireZombie()
		{
			// ������� ������������: ���� ������ � 255, � 0-�� ���������� ����� � ��� �����-���� ���
			if (w.opponent[255].vitality == 0 && w.opponent[0].vitality >= HealerAndZombieDamage&& w.me[ZombieSlot].vitality > 0)
				yield return new Move(ZombieSlot, Funcs.Zero);
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