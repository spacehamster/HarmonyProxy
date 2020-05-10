using Harmony;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace HarmonyTests.ApiTests
{
	[TestFixture]
	class PatchProcessorTests
	{
		[Test]
		public void TestMultiple()
		{
			var targetMethods = new List<MethodBase>()
			{
				AccessTools.Method(typeof(PatchProcessorClass1), nameof(PatchProcessorClass1.TestA)),
				AccessTools.Method(typeof(PatchProcessorClass1), nameof(PatchProcessorClass1.TestB))
			};
			var postfix = new HarmonyMethod(typeof(PatchProcessorPatchClass1), nameof(PatchProcessorPatchClass1.Postfix));
			var harmony = HarmonyInstance.Create("test");
			var processor = new Harmony.PatchProcessor(harmony, targetMethods, postfix: postfix);
			processor.Patch();
			Assert.AreEqual("Patched", PatchProcessorClass1.TestA(), "Test A");
			Assert.AreEqual("Patched", PatchProcessorClass1.TestB(), "Test B");
		}
		[Test]
		public void TestPriority()
		{
			var targetMethod = AccessTools.Method(typeof(PatchProcessorClass2), nameof(PatchProcessorClass2.TestA));
			var patches = new List<HarmonyMethod>() {
				new HarmonyMethod(typeof(PatchProcessorPatchClass2), nameof(PatchProcessorPatchClass2.Postfix)),
				new HarmonyMethod(typeof(PatchProcessorPatchClass3), nameof(PatchProcessorPatchClass3.Postfix)),
				new HarmonyMethod(typeof(PatchProcessorPatchClass4), nameof(PatchProcessorPatchClass4.Postfix)),
				new HarmonyMethod(typeof(PatchProcessorPatchClass5), nameof(PatchProcessorPatchClass5.Postfix)),
			};
			patches[0].prioritiy = 3;
			patches[1].prioritiy = 1;
			patches[2].prioritiy = 2;
			patches[3].prioritiy = 4;
			var harmony = HarmonyInstance.Create("test");
			patches.Do(p => harmony.Patch(targetMethod, postfix: p));
			PatchProcessorClass2.TestA();
			var expected = new List<string>() {
				nameof(PatchProcessorPatchClass5),
				nameof(PatchProcessorPatchClass2),
				nameof(PatchProcessorPatchClass4),
				nameof(PatchProcessorPatchClass3)
			};
			Assert.AreEqual(expected, PatchProcessorClass2.Order, "Test A");
		}
	}
}
