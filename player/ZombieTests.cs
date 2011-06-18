using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using System.Linq;

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
			Assert.AreEqual(9999, world.opponent[0].vitality);
		}

	
	}
}