using Harmony;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyTests.ApiTests
{
	[TestFixture]
	class PatchInfoTest
	{
		public static string TestA()
		{
			return MethodBase.GetCurrentMethod().Name;
		}
		public static string TestB()
		{
			return MethodBase.GetCurrentMethod().Name;
		}
		static bool PatchARan;
		public static void PatchA(ref string __result)
		{
			PatchARan = true;
			__result = "Patched";
		}
		static bool PatchBRan;
		public static void PatchB(ref string __result)
		{
			PatchBRan = true;
			__result = "Patched";
		}
		static bool PatchCRan;
		public static void PatchC()
		{
			PatchCRan = true;
		}
		static bool PatchDRan;
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);
			var call = new CodeInstruction(opcode: OpCodes.Call, AccessTools.Method(typeof(PatchInfoTest), nameof(PatchD)));
			codes.Insert(0, call);
			return codes;
		}
		public static void PatchD()
		{
			PatchDRan = true;
		}
		static void ApplyPatch(string owner, MethodBase target, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
		{
			var instance = HarmonyInstance.Create(owner);
			var processor = new Harmony.PatchProcessor(instance, new List<MethodBase> { target }, prefix: prefix, postfix:postfix, transpiler:transpiler);
			processor.Patch();
		}
		[Test]
		public void TestPatchInfo()
		{
			var targetMethod = AccessTools.Method(typeof(PatchInfoTest), nameof(TestA));

			var patchA = new HarmonyMethod(typeof(PatchInfoTest), nameof(PatchA));
			ApplyPatch("OwnerA", targetMethod, postfix: patchA);

			var patchB = new HarmonyMethod(typeof(PatchInfoTest), nameof(PatchB));
			ApplyPatch("OwnerB", targetMethod, postfix: patchB);

			var patchC = new HarmonyMethod(typeof(PatchInfoTest), nameof(PatchC));
			ApplyPatch("OwnerC", targetMethod, prefix: patchC);

			var patchD = new HarmonyMethod(typeof(PatchInfoTest), nameof(Transpiler));
			ApplyPatch("OwnerD", targetMethod, transpiler: patchD);

			Assert.IsFalse(PatchARan, "Patch A flag set");
			Assert.IsFalse(PatchBRan, "Patch B flag set");
			Assert.IsFalse(PatchCRan, "Patch C flag set");
			Assert.IsFalse(PatchDRan, "Patch D flag set");
			Assert.AreEqual("Patched", TestA(), "Test A");
			Assert.IsTrue(PatchARan, "Ran Patch A");
			Assert.IsTrue(PatchBRan, "Ran Patch B");
			Assert.IsTrue(PatchCRan, "Ran Patch C");
			Assert.IsTrue(PatchDRan, "Ran Patch D");

			var instance = HarmonyInstance.Create("test");
			var patchedMethods = instance.GetPatchedMethods().ToArray();
			Assert.AreEqual(1, patchedMethods.Length, "Patch count");

			var patches = instance.GetPatchInfo(patchedMethods.First());
			var patchOwners = patches.Owners.OrderBy(k => k).ToArray();
			Assert.AreEqual(new string[] { "OwnerA", "OwnerB", "OwnerC", "OwnerD" }, patchOwners, "Patch Owners");
			Assert.AreEqual(2, patches.Postfixes.Count(), "Patch Count");
			Assert.AreEqual("OwnerA", patches.Postfixes.First().owner, "Postfix Owner");
			Assert.AreEqual("OwnerB", patches.Postfixes.Last().owner, "Postfix Owner");
			Assert.AreEqual("OwnerC", patches.Prefixes.First().owner, "Prefix Owner");
			Assert.AreEqual("OwnerD", patches.Transpilers.First().owner, "Postfix Owner");
		}
	}
}
