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

		public static string Repeat(string payload, int count)
		{
			string s = payload;
			for (int i = 1; i < count; i++)
				s = string.Format("S({0})({1})", s, payload);
			return s;
		}

		public static string CreateUberZombie(int zombieSlot, int zombieDamageSlot)
		{
			var damage = string.Format("get({0})", zombieDamageSlot.ToForm());
			var payload = DelayApplication("help(zero)(zero)", damage, false, false);
			var payload4 = Repeat(payload, 4);
			var zombie = string.Format("S(K(zombie (zero))) ( K({0}) )", payload4);
			return string.Format("S(K(S ({0})(get)))(K({1}))", zombie, zombieSlot.ToForm());
		}

		public static string CreateUberZombiePayload(int zombieDamageSlot)
		{
			var damage = string.Format("get({0})", zombieDamageSlot.ToForm());
			var payload = DelayApplication("help(zero)(zero)", damage, false, false);
			return Repeat(payload, 1);
		}

		public static string MakeDelayed0Zombie(string payload)
		{
			return string.Format("S(K(zombie (zero))) ( K({0}) )", payload);
		}

		public static string Create4Zombie(int zombieSlot, int payloadSlot)
		{
			var payload = string.Format("get({0})", payloadSlot.ToForm());
			var payload4 = Repeat(payload, 4);
			var delayedZombie = MakeDelayed0Zombie(payload4);
			return AddSelfReproducing(zombieSlot, delayedZombie);
		}

		/// <summary>
		/// AddIncCycling(hostingSlotNo, payload) (n) -> payload(n), payload(n+1), ...
		/// </summary>
		public static string AddIncCycling(int slotWithHostingSlotNo, string payload)
		{
			var hostingSlotNo = "get(" + slotWithHostingSlotNo.ToForm() + ")";
			var delayGetSource = DelayApplication("get", hostingSlotNo, false, false);
			return string.Format("S({0})(S({1})(succ))", payload, delayGetSource);
		}
	}
}