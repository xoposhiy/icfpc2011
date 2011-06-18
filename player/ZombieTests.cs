using System;
using NUnit.Framework;

namespace Contest
{
	[TestFixture]
	public class ZombieTests
	{
		private World world;

		[SetUp]
		public void SetUp()
		{
			world = new World();
		}

		[Test]
		public void BaseFunctionality()
		{
			world.opponent[255].vitality = 0;
			Console.WriteLine(world.RunMyForm(0, "zombie (zero) (S)"));
			Assert.AreEqual(-1, world.opponent[255].vitality);
			Assert.AreEqual("S", world.opponent[255].value.ToString());
			world.OpponentTurn(new Move(20, Funcs.I));
			Assert.AreEqual("I", world.opponent[255].value.ToString());
			Assert.AreEqual(0, world.opponent[255].vitality);
			Assert.AreEqual(10000, world.opponent[0].vitality);
		}

		[Test]
		public void Inc()
		{
			PutZombieTo255("S(K(inc))(K(zero))");
			Assert.AreEqual(9999, world.opponent[0].vitality);
		}
		
		[Test]
		public void IncMin()
		{
			world.opponent[0].vitality = 0;
			PutZombieTo255("S(K(inc))(K(zero))");
			Assert.AreEqual(0, world.opponent[0].vitality);
		}

		[Test]
		public void Dec()
		{
			PutZombieTo255("S(K(dec))(K(zero))");
			Assert.AreEqual(10001, world.me[255].vitality);
		}

		[Test]
		public void DecMax()
		{
			world.me[255].vitality = 65535;
			PutZombieTo255("S(K(dec))(K(zero))");
			Assert.AreEqual(65535, world.me[255].vitality);
		}

		[Test]
		public void DecMin()
		{
			world.me[255].vitality = 0;
			PutZombieTo255("S(K(dec))(K(zero))");
			Assert.AreEqual(0, world.me[255].vitality);
		}

		[Test]
		public void Attack()
		{
			PutZombieTo255("S(K(attack(zero)(zero)))(K(succ(zero)))", "S(K(attack(zero)(zero)))(K(1))");
			Assert.AreEqual(9999, world.opponent[0].vitality);
			Assert.AreEqual(10000, world.me[255].vitality);
		}

		[Test]
		public void Attack2()
		{
			PutZombieTo255("S(K(attack(zero)(zero)))(K(succ(succ(zero))))", "S(K(attack(zero)(zero)))(K(2))");
			Assert.AreEqual(9998, world.opponent[0].vitality);
			Assert.AreEqual(10001, world.me[255].vitality);
		}

		[Test]
		public void Attack2Dead()
		{
			world.me[255].vitality = 0;
			PutZombieTo255("S(K(attack(zero)(zero)))(K(succ(succ(zero))))", "S(K(attack(zero)(zero)))(K(2))");
			Assert.AreEqual(9998, world.opponent[0].vitality);
			Assert.AreEqual(0, world.me[255].vitality);
		}

		[Test]
		public void Attack2FromDead()
		{
			world.opponent[0].vitality = 0;
			PutZombieTo255("S(K(attack(zero)(zero)))(K(succ(succ(zero))))", "S(K(attack(zero)(zero)))(K(2))");
			Assert.AreEqual(0, world.opponent[0].vitality);
			Assert.AreEqual(10000, world.me[255].vitality);
		}

		[Test]
		public void AttackMax()
		{
			world.me[255].vitality = 65535;
			PutZombieTo255("S(K(attack(zero)(zero)))(K(succ(succ(zero))))", "S(K(attack(zero)(zero)))(K(2))");
			Assert.AreEqual(9998, world.opponent[0].vitality);
			Assert.AreEqual(65535, world.me[255].vitality);
		}

		[Test]
		public void HelpII()
		{
			PutZombieTo255("S(K(help(zero)(zero)))(K(succ(succ(zero))))", "S(K(help(zero)(zero)))(K(2))");
			Assert.AreEqual(9996, world.opponent[0].vitality);
		}

		[Test]
		public void HelpIJ()
		{
			PutZombieTo255("S(K(help(zero)(succ(zero))))(K(succ(succ(zero))))", "S(K(help(zero)(1)))(K(2))");
			Assert.AreEqual(9998, world.opponent[0].vitality);
			Assert.AreEqual(9998, world.opponent[1].vitality);
		}

		[Test]
		public void HelpToDead()
		{
			world.opponent[1].vitality = 0;
			PutZombieTo255("S(K(help(zero)(succ(zero))))(K(succ(succ(zero))))", "S(K(help(zero)(1)))(K(2))");
			Assert.AreEqual(9998, world.opponent[0].vitality);
			Assert.AreEqual(0, world.opponent[1].vitality);
		}

		private void PutZombieTo255(string x, string xToCheck = null)
		{
			xToCheck = xToCheck ?? x;
			world.opponent[255].vitality = 0;
			Console.WriteLine(world.RunMyForm(0, string.Format("zombie (zero) (  {0} )", x)));
			Assert.AreEqual(-1, world.opponent[255].vitality);
			Assert.AreEqual(xToCheck, world.opponent[255].value.ToString());
			world.OpponentTurn(new Move(20, Funcs.I));
			Assert.AreEqual("I", world.opponent[255].value.ToString());
			Assert.AreEqual(0, world.opponent[255].vitality);
		}
	}
}