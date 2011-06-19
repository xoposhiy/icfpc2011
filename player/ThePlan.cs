using System;
using System.Collections.Generic;
using System.Linq;

namespace Contest
{
	public class Term
	{
		
	}

	public class Name : Term
	{
		public Name(string name)
		{
			this.name = name;
		}
		
		public override string ToString()
		{
			return name;
		}

		public string name;
	}
	
	public class App : Term
	{
		public App(Term left, Term right)
		{
			this.left = left;
			this.right = right;
		}

		public override string ToString()
		{
			return left + " (" + right + ")";
		}

		public Term left, right;

	}

	public class PlanItem
	{
		public bool ToLeft;
		public string Card;
	}



	public class ThePlan
	{
		public static PlanItem[] MakePlan(Term t)
		{
			return MakePlan(new PlanItem[0], t);
		}

		public static PlanItem[] MakePlan(PlanItem[] plan, Term t)
		{
			if (t is Name) return plan.Concat(Term(t)).ToArray();
			var app = (App)t;
			if (plan.Length == 0)
			{
				if (app.left is App)
				{
					return MakePlan(MakePlan(plan, app.left), app.right);
				}
				else
				{
					var name = (Name)app.left;
					return MakePlan(plan, app.right).Concat(new[] { new PlanItem { Card = name.name, ToLeft = true } }).ToArray();
				}
			}
			return 
				MakePlan(
					MakePlan(
						plan.Concat(SK()).ToArray(), 
						app.left)
						.Concat(SK())
						.Concat(Term("I")).ToArray(), 
						app.right);
		}

		public static string MakePlanForTail(int slotNo, string tail)
		{
			var plan = MakePlan(new PlanItem[]{null}, Parse(tail)).Skip(1).ToArray();
			return FormatPlan(slotNo, plan);
		}

		public static string MakePlan(int slotNo, string form)
		{
			var plan = MakePlan(Parse(form));
			return FormatPlan(slotNo, plan);
		}

		private static string FormatPlan(int slotNo, PlanItem[] plan)
		{
			var s = "";
			foreach (var i in plan)
			{
				s += (i.ToLeft ? (i.Card + " " + slotNo) : (slotNo + " " + i.Card)) + "\r\n";
			}
			return s;
		}

		public static IEnumerable<PlanItem> Term(Term t)
		{
			yield return new PlanItem { ToLeft = false, Card = (((Name)t).name) };
		}

		public static IEnumerable<PlanItem> Term(string name)
		{
			yield return new PlanItem { ToLeft = false, Card = name };
		}

		public static IEnumerable<PlanItem> SK()
		{
			yield return new PlanItem { ToLeft = true, Card = "K" };
			yield return new PlanItem { ToLeft = true, Card = "S" };
		}

		public static Term Parse(string s)
		{
			s = s.Replace("(", " ( ");
			s = s.Replace(")", " ) ");
			var stack = new Stack<Term>();
			var items = s.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);
			foreach (var item in items)
			{
				if (item == ")")
				{
					var arg = stack.Pop();
					var fun = stack.Pop();
					stack.Push(new App(fun, arg));
				}
				else if (item != "(")
				{
					stack.Push(new Name(item));
				}
			}
			if (stack.Count != 1) throw new Exception(stack.Count.ToString());
			return stack.Pop();
		}
		
	}
}