using System;

namespace Contest
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length == 0) RunInteractive();
			//else RunBattle();
		}

		private static Move ReadMoveInteractive()
		{
			try
			{
				return Move.Parse(Console.ReadLine());
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				return ReadMoveInteractive();
			}
		}


		private static void RunInteractive()
		{
			var world = new World();
			while (true)
			{
				var move = ReadMoveInteractive();
				Console.WriteLine(move.ToString());
				world.MyTurn(move);
				Console.WriteLine(world.ToString(true));
			}
		}
	}
}
