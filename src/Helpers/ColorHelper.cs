using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AgriCore.Helpers;

/// <summary>
///     Class helping to add syntax colors to the game
/// </summary>
public static class ColorHelper
{
	private static readonly Dictionary<string, string> ColorPerGroup = [];

	private static readonly Dictionary<string, List<string>> PatternsPerGroup = [];

	/// <summary>
	///     Adds a pattern for a given color
	/// </summary>
	/// <param name="pattern">REGEX pattern to respect</param>
	/// <param name="color">Color to use in HEX</param>
	/// <param name="group">Name of the REGEX group</param>
	/// <returns>Success of the addition</returns>
	public static bool Add(string pattern, string color, string group)
	{
		if (!IsValidColor(color)) {
			Log.Warning($"'{color}' is not a valid HEX code and will not be put on the pattern '{pattern}'.");
			return false;
		}

		if (string.IsNullOrEmpty(group)) {
			Log.Warning($"Cannot add the pattern '{pattern}' to an empty group.");
			return false;
		}

		ColorPerGroup[group] = color;

		if (!PatternsPerGroup.TryGetValue(group, out List<string>? patterns)) {
			PatternsPerGroup[group] = [
				pattern,
			];
			return true;
		}

		patterns.Add(pattern);
		return true;
	}

	/// <summary>
	///     Compiles the registered patterns into a regex string
	/// </summary>
	public static string GetRegexString()
	{
		var patternGroups = new List<string>();

		// ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
		foreach (KeyValuePair<string, List<string>> group in PatternsPerGroup) {
			string? groupName = group.Key;
			string  patterns  = string.Join("|", group.Value);

			patternGroups.Add($"(?<{groupName}>(?:{patterns}))");
		}

		return string.Join("|", patternGroups);
	}

	/// <summary>
	///     Adds the color to the given text depending on the match
	/// </summary>
	public static string? GetColoredText(Match match)
	{
		// ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
		foreach (KeyValuePair<string, string> group in ColorPerGroup) {
			string? groupName = group.Key;

			if (!match.Groups[groupName].Success) continue;

			return $"<color={group.Value}>{match.Value}</color>";
		}

		return null;
	}

	private static bool IsValidColor(string color)
	{
		// Should start with #
		if (!color.StartsWith("#")) return false;

		// Wrong length
		if (color.Length != 7) return false;

		// Check if string is valid int
		return int.TryParse(
			color.Substring(1), NumberStyles.HexNumber, null,
			out _
		);
	}
}