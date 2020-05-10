using Harmony.ILCopying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	public static class MethodPatcher
	{
		// special parameter names that can be used in prefix and postfix methods
		//
		public static string INSTANCE_PARAM = "__instance";
		public static string ORIGINAL_METHOD_PARAM = "__originalMethod";
		public static string RESULT_VAR = "__result";
		public static string STATE_VAR = "__state";
		public static string PARAM_INDEX_PREFIX = "__";
		public static string INSTANCE_FIELD_PREFIX = "___";

		// in case of trouble, set to true to write dynamic method to desktop as a dll
		// won't work for all methods because of the inability to extend a type compared
		// to the way DynamicTools.CreateDynamicMethod works
		//
		static readonly bool DEBUG_METHOD_GENERATION_BY_DLL_CREATION = false;

		// for fixing old harmony bugs
		[UpgradeToLatestVersion(1)]
		public static DynamicMethod CreatePatchedMethod(MethodBase original, List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers)
		{
			throw new NotImplementedException("MethodPatcher.CreatePatchedMethod not supported");
		}

		public static DynamicMethod CreatePatchedMethod(MethodBase original, string harmonyInstanceID, List<MethodInfo> prefixes, List<MethodInfo> postfixes, List<MethodInfo> transpilers)
		{
			throw new NotImplementedException("MethodPatcher.CreatePatchedMethod not supported");
		}
	}
}