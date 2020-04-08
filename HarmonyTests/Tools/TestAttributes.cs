using NUnit.Framework;
using Harmony;
using HarmonyTests.Assets;
using System.Linq;

namespace HarmonyTests.Tools
{
	[TestFixture]
	public class Test_Attributes
	{
		[Test]
		public void TestAttributes()
		{
			var type = typeof(AllAttributesClass);
			var infos = type.GetHarmonyMethods();
			var info = HarmonyMethod.Merge(infos);
			Assert.IsNotNull(info);
			Assert.AreEqual(typeof(string), info.declaringType);
			Assert.AreEqual("foobar", info.methodName);
			Assert.IsNotNull(info.argumentTypes);
			Assert.AreEqual(2, info.argumentTypes.Length);
			Assert.AreEqual(typeof(float), info.argumentTypes[0]);
			Assert.AreEqual(typeof(string), info.argumentTypes[1]);
		}
	}
}