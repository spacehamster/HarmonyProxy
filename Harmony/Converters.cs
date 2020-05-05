extern alias Harmony20;

using System.Linq;

namespace Harmony
{
	internal static class Converters
	{
		internal static Patches ToHarmony12(this Harmony20.HarmonyLib.Patches patches)
		{
			var postfixes = patches.Postfixes.Select(p => p.ToHarmony12());
			var prefixes = patches.Prefixes.Select(p => p.ToHarmony12());
			var transpilers = patches.Transpilers.Select(p => p.ToHarmony12());
			return new Patches(postfixes.ToArray(), prefixes.ToArray(), transpilers.ToArray());
		}
		internal static Patch ToHarmony12(this Harmony20.HarmonyLib.Patch patch)
		{
			return new Patch(patch.PatchMethod, patch.index, patch.owner, 
				patch.priority, patch.before, patch.after);
		}
		internal static Harmony20.HarmonyLib.CodeInstruction ToHarmony20CodeInstruction(this CodeInstruction codeInstruction)
		{
			var result = new Harmony20.HarmonyLib.CodeInstruction(codeInstruction.opcode, codeInstruction.operand)
			{
				labels = codeInstruction.labels
			};
			return result;
		}
		internal static Harmony20.HarmonyLib.HarmonyPatchType ToHarmony20(this HarmonyPatchType type)
		{
			return (Harmony20.HarmonyLib.HarmonyPatchType)type;
		}
	}
}
