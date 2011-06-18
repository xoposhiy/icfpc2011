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

		public bool IsZombie { get { return vitality == -1; } }
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

		public void MyTurn(Move move)
		{
			RegisterMove(me, opponent, move);
		}

		public static void RegisterMove(Slot[] me, Slot[] opponent, Move move)
		{
			if (move.slot < 0 || move.slot > 255) return;
			if (move.card_to_slot)
				Apply(me, opponent, move.card, me[move.slot].value, move.slot);
			else
				Apply(me, opponent, me[move.slot].value, move.card, move.slot);
		}

		private static void Apply(Slot[] me, Slot[] opponent, Value left, Value right, int resultSlot)
		{
			RessurectZombies(me, opponent);
			try
			{
				if (left.ArgsNeeded <= 0) throw new GameError("incorrect application: " + left + " " + right);
				var applicationsDone = 0;
				var res = new Application(left, right).Reduce(me, opponent, ref applicationsDone, false);
				me[resultSlot].value = res;
			}
			catch (GameError e)
			{
				Log(e.Message);
				me[resultSlot].value = Funcs.I;
			}
		}

		private static void RessurectZombies(Slot[] me, Slot[] opponent)
		{
			for (int mySlot = 0; mySlot <= 255; mySlot++)
			{
				var currentMySlot = me[mySlot];
				if (!currentMySlot.IsZombie) continue;
				try
				{
					var zobieFunc = currentMySlot.value;
					if (zobieFunc.ArgsNeeded <= 0) throw new GameError("incorrect application of zombie func: " + zobieFunc);
					var applicationsDone = 0;
					new Application(zobieFunc, Funcs.I).Reduce(me, opponent, ref applicationsDone, true);
				}
				catch (GameError e)
				{
					Log(e.Message);
				}
				currentMySlot.vitality = 0;
				currentMySlot.value = Funcs.I;
			}
		}

		private static void Log(string message)
		{
			//Console.WriteLine(message);
		}

		public void OpponentTurn(Move move)
		{
			RegisterMove(opponent, me, move);
		}

		public string ToString(bool isMe)
		{
			return isMe ? SlotsToString(me) : SlotsToString(opponent);
		}


		public void RunMyPlan(string s)
		{
			var lines = s.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var line in lines)
			{
				MyTurn(Move.Parse(line));
				Console.WriteLine(ToString());
			}
		}

		public string RunMyForm(int slot, string form)
		{
			var plan = ThePlan.MakePlan(slot, form);
			RunMyPlan(plan);
			return plan;
		}
	}
}