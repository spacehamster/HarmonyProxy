
using Harmony;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace HarmonyTests.ApiTests
{
	[TestFixture]
	class PatchProcessorTests
	{
		public int TestA()
		{
			return 0;
		}
		public int TestB()
		{
			return 0;
		}
		static void Postfix(ref int __result)
		{
			__result = 1;
		}
		[Test]
		public void TestMultiple()
		{
			var targetMethods = new List<MethodBase>()
			{
				AccessTools.Method(typeof(PatchProcessorTests), nameof(TestA)),
				AccessTools.Method(typeof(PatchProcessorTests), nameof(TestB))
			};
			var postfix = new HarmonyMethod(typeof(PatchProcessorTests), nameof(Postfix));
			var harmony = HarmonyInstance.Create("test");
			var processor = new Harmony.PatchProcessor(harmony, targetMethods, postfix: postfix);
			processor.Patch();
			Assert.AreEqual(1, TestA());
			Assert.AreEqual(1, TestB());

		}
	}
}
