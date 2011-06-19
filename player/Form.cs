namespace Contest
{
	public class Form
	{
		public static string DelayFun(string f)
		{
			return string.Format("K({0})", f);
		}

		public static string DelayApplication(string f, string arg, bool funIsDelayed, bool argIsDelayed)
		{
			if (!funIsDelayed) f = DelayFun(f);
			if (!argIsDelayed) arg = DelayFun(arg);
			return string.Format("S ({0}) ({1}) ", f, arg);
		}

		/// <summary>
		/// CreateDelayedAttacker(string targetSlot, string damageSlot) (_) -> attacks (get (targetSlot)) (get (targetSlot)) (get (damageSlot))
		/// </summary>
		public static string CreateDelayedAttacker(string targetSlot, string damageSlot)
		{
			var dellayedGetTarget = DelayApplication("get", targetSlot, false, false);
			string attack_I_I = DelayApplication("attack (zero)", dellayedGetTarget, false, true);
			var damage = string.Format("get ({0})", damageSlot);
			string attack_I_I_D = DelayApplication(attack_I_I, damage, true, false);
			return attack_I_I_D;
		}

		public static string AddSelfReproducing(int slotNo, string delayedValue)
		{
			return  string.Format("S(K(S ({0})(get)))(K({1}))", delayedValue, slotNo.ToForm());
		}

		public static string CreateHealer(string targetSlot, string damageSlot, string protoSlot)
		{
			var delayedGetTarget = DelayApplication("get", targetSlot, false, false);
			var healing = DelayApplication("S (help) (I)", delayedGetTarget, false, true);
			var getDamage = string.Format("get({0})", damageSlot);
			healing = DelayApplication(healing, getDamage, true, false);
			return AddCycling(healing, protoSlot);
		}

		public static string AddCycling(string payload, string protoSlot)
		{
			var cycling = DelayApplication(payload, "S (get) (I)", true, true);
			cycling = DelayApplication(cycling, protoSlot, false, false);
			return cycling;
		}

		public static string CreateHealer255(int healerSlotNo, string damageSlot, string getTargetSlotFrom)
		{
			var target = string.Format("get({0})", getTargetSlotFrom);
			var damage = string.Format("dbl(get({0}))", damageSlot);
			var source = 1.ToForm();
			var healing = string.Format("help ({0}) ({1})", source, target);
			healing = DelayApplication(healing, damage, false, false);
			return AddSelfReproducing(healerSlotNo, healing);
		}
	}
}