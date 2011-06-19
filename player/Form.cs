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
			string attack_I_I = DelayApplication("S (attack) (I)", dellayedGetTarget, false, true);
			var delayedGetDamage = DelayApplication("get", damageSlot, false, false);
			string attack_I_I_D = DelayApplication(attack_I_I, delayedGetDamage, true, true);
			return attack_I_I_D;
		}

		
	}
}