using AgriCore.Helpers;
using HarmonyLib;
// ReSharper disable InconsistentNaming

namespace AgriCore.Patches;

[HarmonyPatch(typeof(Localizer))]
internal static class Localizer_Patches
{
	[HarmonyPatch(nameof(Localizer.Localize), typeof(string))]
	[HarmonyPrefix]
	private static bool Localize_Prefix(string key, ref string __result)
	{
		string? value = LocalizerHelper.GetText(Localizer.Lang, key);

		if (value == null) return true;

		__result = value;
		return false;
	}
}