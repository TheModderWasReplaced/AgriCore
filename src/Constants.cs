// ReSharper disable InconsistentNaming

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace AgriCore;

/// <summary>
///     Contains useful constants to ease the code.
/// </summary>
public static class Constants
{
	// --- MOD INFO ---
	public const string GUID = "org.warpersan.farmhelper";

	// ---

	public const float MAIN_TITLE_OFFSET_X = 15f;
	public const float MAIN_TITLE_OFFSET_Y = 62.68f;

	// --- PATHS ---
	public const string MENU_PATH  = "OverlayUI/MenuCanvas/Menu";
	public const string TITLE_PATH = MENU_PATH + "/TitlePage";

	// ---

	// --- LANG ---
	public const string ANY_LANGUAGE = "ANY";
	public const string ENGLISH      = "EN";

	// ---

	// --- ASSETS ---
	private const string RESOURCES_PATH = nameof(AgriCore) + ".Resources";
	public const  string UI_BUNDLE      = RESOURCES_PATH + ".Bundles.uibundle.assets";

	public const string ICON_LINK = RESOURCES_PATH + ".Images.icon-link.png";

	// ---
}