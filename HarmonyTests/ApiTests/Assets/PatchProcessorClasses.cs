using System.Collections.Generic;
using System.Reflection;

namespace HarmonyTests.ApiTests
{
	public class PatchProcessorClass1
	{
		public static string TestA()
		{
			return MethodBase.GetCurrentMethod().Name;
		}
		public static string TestB()
		{
			return MethodBase.GetCurrentMethod().Name;
		}
	}

	public class PatchProcessorPatchClass1
	{
		public static void Postfix(ref string __result)
		{
			__result = "Patched";
		}
	}

	public class PatchProcessorClass2
	{
		public static List<string> Order = new List<string>();
		public static string TestA()
		{
			return MethodBase.GetCurrentMethod().Name;
		}
	}

	public class PatchProcessorPatchClass2
	{
		public static void Postfix()
		{
			PatchProcessorClass2.Order.Add(nameof(PatchProcessorPatchClass2));
		}
	}

	public class PatchProcessorPatchClass3
	{
		public static void Postfix()
		{
			PatchProcessorClass2.Order.Add(nameof(PatchProcessorPatchClass3));
		}
	}

	public class PatchProcessorPatchClass4
	{
		public static void Postfix()
		{
			PatchProcessorClass2.Order.Add(nameof(PatchProcessorPatchClass4));
		}
	}

	public class PatchProcessorPatchClass5
	{
		public static void Postfix()
		{
			PatchProcessorClass2.Order.Add(nameof(PatchProcessorPatchClass5));
		}
	}
}
