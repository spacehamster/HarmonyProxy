using Harmony;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace HarmonyTests.ApiTests
{
	[TestFixture]
	class PatchProcessorTests
	{
		public string TestA()
		{
			return MethodBase.GetCurrentMethod().Name;
		}
		public string TestB()
		{
			return MethodBase.GetCurrentMethod().Name;
		}
		public static void Postfix(ref string __result)
		{
			__result = "Patched";
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
			Assert.AreEqual("Patched", TestA(), "Test A");
			Assert.AreEqual("Patched", TestB(), "Test B");
		}
	}
}
