using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AgriCore.Extensions;

/// <summary>
///     Extensions methods for <see cref="ColoredButton" />
/// </summary>
public static class ColoredButtonExtensions
{
	/// <summary>
	///     Sets the listener of this button to the given action
	/// </summary>
	/// <param name="button"></param>
	/// <param name="callback">Code to run upon a click</param>
	public static void SetListener(this ColoredButton button, UnityAction callback)
	{
		// Clear all click events
		button.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();

		// Add click event
		button.OnClick.AddListener(callback);
	}

	/// <summary>
	///     Gets the icon of this button
	/// </summary>
	/// <remarks>
	///     This only works for the buttons created from <see cref="UiExtensions.AddButton(Object)" />
	/// </remarks>
	public static Image? GetIcon(this ColoredButton button) =>
		button.transform.Find("Panel/Image")?.GetComponent<Image>();
}