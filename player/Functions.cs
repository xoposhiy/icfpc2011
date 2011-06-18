using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contest
{
	public static class Funcs
	{
		public static Value[] allFuncs;
		static Funcs()
		{
			allFuncs = new Value[15];
			allFuncs[0] = Zero;
			allFuncs[1] = S;
			allFuncs[2] = K;
			allFuncs[3] = I;
			allFuncs[4] = Succ;
			allFuncs[5] = Dbl;
			allFuncs[6] = Get;
			allFuncs[7] = Put;
			allFuncs[8] = Inc;
			allFuncs[9] = Dec;
			allFuncs[10] = Attack;
			allFuncs[11] = Help;
			allFuncs[12] = Revive;
			allFuncs[13] = Copy;
			allFuncs[14] = Zombie;
		}
		public static Value Parse(string s)
		{
			s = s.ToLower();
			if (s == "zero") return Zero;
			if (s == "s") return S;
			if (s == "k") return K;
			if (s == "i") return I;
			if (s == "succ") return Succ;
			if (s == "dbl") return Dbl;
			if (s == "get") return Get;
			if (s == "put") return Put;
			if (s == "inc") return Inc;
			if (s == "dec") return Dec;
			if (s == "help") return Help;
			if (s == "attack") return Attack;
			if (s == "copy") return Copy;
			if (s == "revive") return Revive;
			if (s == "zombie") return Zombie;
			throw new Exception("Unknown function " + s);
		}

		public static readonly Zero Zero = new Zero();
		public static readonly I I = new I();
		public static readonly Succ Succ = new Succ();
		public static readonly Dbl Dbl = new Dbl();
		public static readonly Get Get = new Get();
		public static readonly Put Put = new Put();
		public static readonly S S = new S();
		public static readonly K K = new K();
		public static readonly Inc Inc = new Inc();
		public static readonly Dec Dec = new Dec();
		public static readonly Attack Attack = new Attack();
		public static readonly Help Help = new Help();
		public static readonly Copy Copy = new Copy();
		public static readonly Revive Revive = new Revive();
		public static readonly Zombie Zombie = new Zombie();
	}

	public class Zero : Num
	{
		public Zero() : base(0)
		{
		}
		
		public override string ToString()
		{
			return "zero";
		}
	}

	public class I : Function
	{
		public I()
			: base(1, "I")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			return args.First();
		}
	}

	public class Succ : Function
	{
		public Succ()
			: base(1, "succ")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			var arg = args.First();
			if (arg is Num) return new Num(Math.Min(((Num)arg).num + 1, 65535));
			throw new GameError("Call succ with not a num: " + arg);
		}
	}

	public class Dbl : Function
	{
		public Dbl()
			: base(1, "dbl")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			var arg = args.First();
			if (arg is Num) return new Num(Math.Min(2 * ((Num)arg).num, 65535));
			throw new GameError("Call dbl with not a num: " + arg);
		}
	}
	
	public class Get: Function
	{
		public Get()
			: base(1, "get")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			return me[args.First().AsSlot()].value;
		}
	}

	public class Put : Function
	{
		public Put()
			: base(1, "put")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			return Funcs.I;
		}
	}

	public class S : Function
	{
		public S()
			: base(3, "S")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			var f = args[0];
			var g = args[1];
			var x = args[2];
			var left = new Application(f, x).Reduce(me, opponent, ref applicationsDone);
			var right = new Application(g, x).Reduce(me, opponent, ref applicationsDone);
			var res = new Application(left, right).Reduce(me, opponent, ref applicationsDone);
			return res;
		}
	}

	public class K : Function
	{
		public K()
			: base(2, "K")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			return args[0];
		}
	}
	
	//TODO Zombie!
	public class Inc : Function
	{
		public Inc()
			: base(1, "inc")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			var slot = args.First().AsSlot();
			var vitality = me[slot].vitality;
			if (vitality > 0 || vitality < 65535)
				me[slot].vitality = vitality + 1;
			return Funcs.I;
		}
	}

	public class Dec : Function
	{
		public Dec()
			: base(1, "dec")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			var slot = 255 - args.First().AsSlot();
			var vitality = opponent[slot].vitality;
			if (vitality > 0)
				opponent[slot].vitality = vitality - 1;
			return Funcs.I;
		}
	}

	public class Attack : Function
	{
		public Attack()
			: base(3, "attack")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			var proponentSlot = args[0].AsSlot();
			var opponentSlot = 255 - args[1].AsSlot();
			var n = args[2].AsNum();
			if (me[proponentSlot].vitality < n) throw new GameError("not enough vitality " + me[proponentSlot].vitality + " for " + this);
			me[proponentSlot].vitality -= n;
			if (opponent[opponentSlot].vitality > 0)
			{
				opponent[opponentSlot].vitality -= n*9/10;
				if (opponent[opponentSlot].vitality < 0) opponent[opponentSlot].vitality = 0;
			}
			return Funcs.I;
		}
	}

	public class Help : Function
	{
		public Help()
			: base(3, "help")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			var fromSlot = args[0].AsSlot();
			var toSlot = args[1].AsSlot();
			var n = args[2].AsNum();
			if (me[fromSlot].vitality < n) throw new GameError("not enough vitality " + me[fromSlot].vitality + " for " + this);
			me[fromSlot].vitality -= n;
			if (me[toSlot].vitality > 0)
			{
				me[toSlot].vitality += n * 11 / 10;
				if (me[toSlot].vitality > 65535) me[toSlot].vitality = 65535;
			}
			return Funcs.I;
		}
	}

	public class Copy : Function
	{
		public Copy()
			: base(1, "copy")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			return opponent[args[0].AsSlot()].value;
		}
	}

	public class Revive : Function
	{
		public Revive()
			: base(1, "revive")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			var proponentSlot = args[0].AsSlot();
			if (me[proponentSlot].vitality <= 0)
				me[proponentSlot].vitality = 1;
			return Funcs.I;
		}
	}
	public class Zombie : Function
	{
		public Zombie()
			: base(2, "zombie")
		{
		}

		public override Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			var opponentSlot = 255 - args[0].AsSlot();
			var x = args[0];
			if (opponent[opponentSlot].vitality > 0) throw new GameError("Slot is not dead! " + this);
			opponent[opponentSlot].value = x;
			opponent[opponentSlot].vitality = -1;
			return Funcs.I;
		}
	}

	public abstract class Value
	{
		protected Value(int argsNeeded, Func<string> toString)
		{
			ArgsNeeded = argsNeeded;
			this.toString = toString;
		}

		public int AsNum()
		{
			var n = this as Num;
			if (n == null) throw new GameError("Not a num " + this);
			return n.num;
		}

		public int AsSlot()
		{
			var n = AsNum();
			if (n < 0 || n > 255) throw new GameError("Not a slot " + this);
			return n;
		}

		public int ArgsNeeded;
		private readonly Func<string> toString;

		public override string ToString()
		{
			return toString();
		}
	}

	public class Application : Value
	{
		public Application(Value f, Value arg)
			: base(f.ArgsNeeded - 1, () => string.Format("{0}({1})", f, arg))
		{
			this.f = f;
			this.arg = arg;
		}

		public readonly Value f;
		public readonly Value arg;

		public Value Reduce(Slot[] me, Slot[] opponent, ref int applicationsDone)
		{
			if (ArgsNeeded > 0) return this;
			var fun = f;
			var args = new List<Value> { arg };
			while (fun is Application)
			{
				var app = (Application)fun;
				args.Insert(0, app.arg);
				fun = app.f;
			}
			if (!(fun is Function)) throw new GameError("cant apply not a function " + fun);
			var res = ((Function)fun).Reduce(me, opponent, args.ToArray(), ref applicationsDone);
			if (!(res is Num) && res.ArgsNeeded == 0) throw new Exception("Bug!");
			return res;
		}
	}

	public class Num : Value
	{
		public Num(int num)
			: base(0, num.ToString)
		{
			this.num = num;
			if (num > 65535 || num < -1) throw new Exception("Bug!");
		}

		public readonly int num;
	}

	public abstract class Function : Value
	{
		// Порядок применения функций. Для отладки.
		public static StringBuilder r = new StringBuilder();

		protected Function(int argsNeeded, string name)
			: base(argsNeeded, () => name)
		{
		}

		public Value Reduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone)
		{
			if (args.Length != ArgsNeeded) throw new Exception("Bug in code!");
			if (applicationsDone >= 1000) throw new GameError("Too many applications");
			applicationsDone++;
			//r.Append(" " + ToString());
			var res = DoReduce(me, opponent, args, ref applicationsDone);
			return res;
		}

		public abstract Value DoReduce(Slot[] me, Slot[] opponent, Value[] args, ref int applicationsDone);

	}
}
