using System.Linq;

namespace Harmony
{
	internal static class Converters
	{
		internal static Patches ToHarmony12(this HarmonyLib.Patches patches)
		{
			var prefixes = patches.Prefixes.Select(p => p.ToHarmony12());
			var postfixes = patches.Postfixes.Select(p => p.ToHarmony12());
			var transpilers = patches.Transpilers.Select(p => p.ToHarmony12());
			return new Patches(prefixes.ToArray(), postfixes.ToArray(), transpilers.ToArray());
		}
		internal static Patch ToHarmony12(this HarmonyLib.Patch patch)
		{
			return new Patch(patch.PatchMethod, patch.index, patch.owner, 
				patch.priority, patch.before, patch.after);
		}
		internal static HarmonyLib.HarmonyMethod ToHarmony20(this HarmonyMethod method)
		{
			return new HarmonyLib.HarmonyMethod(method.method)
			{
				priority = method.prioritiy,
				before = method.before,
				after = method.after
			};
		}
		internal static HarmonyLib.CodeInstruction ToHarmony20CodeInstruction(this CodeInstruction codeInstruction)
		{
			var result = new HarmonyLib.CodeInstruction(codeInstruction.opcode, codeInstruction.operand)
			{
				labels = codeInstruction.labels
			};
			foreach(var block in codeInstruction.blocks)
			{
				result.blocks.Add(block.ToHarmony20());
			}
			return result;
		}
		internal static HarmonyLib.ExceptionBlock ToHarmony20(this Harmony.ILCopying.ExceptionBlock block)
		{
			var blockType = (HarmonyLib.ExceptionBlockType)block.blockType;
			return new HarmonyLib.ExceptionBlock(blockType, block.catchType);
		}
		internal static HarmonyLib.HarmonyPatchType ToHarmony20(this HarmonyPatchType type)
		{
			return (HarmonyLib.HarmonyPatchType)type;
		}
	}
}
