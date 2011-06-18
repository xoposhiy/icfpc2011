using System;
using System.Text;

namespace Contest
{
	public class GameError : Exception
	{
		public GameError(string message)
			: base(message)
		{

		}
	}

	public class Move
	{
		public static Move Parse(string line)
		{
			var parts = line.Split(' ');
			if (parts[0][0] >= '0' && parts[0][0] <= '9')
				return new Move(Int32.Parse(parts[0]), Funcs.Parse(parts[1]));
			return new Move(Funcs.Parse(parts[0]), Int32.Parse(parts[1]));
		}

		public Move(int slot, Value card)
		{
			this.slot = slot;
			this.card = card;
			card_to_slot = false;
		}

		public Move(Value card, int slot)
		{
			this.slot = slot;
			this.card = card;
			card_to_slot = true;
		}

		public bool card_to_slot;
		public int slot;
		public Value card;
		public override string ToString()
		{
			return card_to_slot ? (card + " [" + slot + "]") : (slot + " [" + card + "]");
		}

		public string ToOrgString()
		{
			var s = "";
			s += card_to_slot ? "1" : "2";
			s += "\n";
			s += card_to_slot ? card.ToString() : slot.ToString();
			s += "\n";
			s += card_to_slot ? slot.ToString() : card.ToString();
			s += "\n";
			return s;
		}
	}

	public class Slot
	{
		public int vitality;
		public Value value;
	}

	public class World
	{
		public Slot[] opponent;
		//public bool opponentHasZombies = false;
		public Slot[] me;
		//public bool IHaveZombies = false;
		public int turnNumber;

		public World()
		{
			opponent = new Slot[256];
			me = new Slot[256];
			for (int i = 0; i <= 255; i++)
			{
				opponent[i] = new Slot{value = Funcs.I, vitality = 10000};
				me[i] = new Slot{value = Funcs.I, vitality = 10000};
			}
			turnNumber = 0;
		}

		public override string ToString()
		{
			return string.Format("me:\r\n{0}\r\nopponent:\r\n{1}", SlotsToString(me), SlotsToString(opponent));
		}

		private static string SlotsToString(Slot[] slots)
		{
			var sb = new StringBuilder();
			for (int i = 0; i <= 255; i++)
			{
				if (slots[i].vitality != 10000 || slots[i].value != Funcs.I)
					sb.AppendFormat("{0}={{{1},{2}}}\r\n", i, slots[i].vitality, slots[i].value);
			}
			string s = sb.ToString();
			return s.TrimEnd();
		}

		public Value MyTurn(Move move)
		{
			return RegisterMove(me, opponent, move);
		}

		public static Value RegisterMove(Slot[] me, Slot[] opponent, Move move)
		{
			if (move.card_to_slot)
				return Apply(me, opponent, move.card, me[move.slot].value, move.slot);
			return Apply(me, opponent, me[move.slot].value, move.card, move.slot);
		}

		private static Value Apply(Slot[] me, Slot[] opponent, Value left, Value right, int resultSlot)
		{
			try
			{
				if (left.ArgsNeeded <= 0) throw new GameError("incorrect application: " + left + " " + right);
				var applicationsDone = 0;
				var res = new Application(left, right).Reduce(me, opponent, ref applicationsDone);
				me[resultSlot].value = res;
			}
			catch (GameError e)
			{
				Log(e.Message);
				me[resultSlot].value = Funcs.I;
			}
			return me[resultSlot].value;
		}

		private static void Log(string message)
		{
			Console.WriteLine(message);
		}

		public Value OpponentTurn(Move move)
		{
			return RegisterMove(opponent, me, move);
		}

		public string ToString(bool isMe)
		{
			return isMe ? SlotsToString(me) : SlotsToString(opponent);
		}
	}
}