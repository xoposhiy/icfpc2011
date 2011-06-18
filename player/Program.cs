using System;

namespace Contest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (args.Length == 0) RunInteractive();
			var player = new Player();
			if (args[0] == "1") player.HisMove(ReadMove());
			foreach (var m in player.MyMoves())
			{
				WriteMove(m);
				var hisMove = ReadMove();
				if (hisMove == null) break;
				player.HisMove(hisMove);

			}
		}

		private static void OutLine(string s)
		{
			Console.Write(s + "\n");
		}

		private static void WriteMove(Move m)
		{
			OutLine(m.card_to_slot ? "1" : "2");
			OutLine(m.card_to_slot ? m.card.ToString() : m.slot.ToString());
			OutLine(m.card_to_slot ? m.slot.ToString() : m.card.ToString());
		}

		private static Move ReadMove()
		{
			Console.ReadLine();
			var left = Console.ReadLine();
			var right = Console.ReadLine();
			if (left == null || right == null) return null;
			return Move.Parse(left + " " + right);
		}

		private static Move ReadMoveInteractive()
		{
			try
			{
				return Move.Parse(Console.ReadLine());
			}
			catch (Exception e)
			{
				OutLine(e.Message);
				return ReadMoveInteractive();
			}
		}

		private static void RunInteractive()
		{
			var world = new World();
			while (true)
			{
				Move move = ReadMoveInteractive();
				OutLine(move.ToString());
				world.MyTurn(move);
				OutLine(world.ToString(true));
			}
		}
	}
}