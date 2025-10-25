using UnityEngine;

namespace AgriCore.Helpers;

/// <summary>
///     Class helping for UI elements
/// </summary>
public static class UiHelper
{
	private const string Template = Constants.TITLE_PATH + "/PlayButton";

	/// <summary>
	///     Adds a button to the title screen
	/// </summary>
	/// <param name="x">X grid position</param>
	/// <param name="y">Y grid position</param>
	/// <param name="button">Button created</param>
	/// <returns>Was successfully created</returns>
	public static bool AddTitleButton(int x, int y, out ColoredButton? button)
	{
		button = null;

		// Get template
		GameObject? template = GameObject.Find(Template);

		if (template == null) return false;

		// Create button
		GameObject? newBtn = Object.Instantiate(
			template,
			template.transform.parent
		);

		// If not found, clean
		if (!newBtn.TryGetComponent(out button)) {
			Object.Destroy(newBtn);
			return false;
		}

		// Configure button
		newBtn.GetComponent<RectTransform>().localPosition += new Vector3(
			(newBtn.GetComponent<RectTransform>().rect.width + Constants.MAIN_TITLE_OFFSET_X) * x,
			-Constants.MAIN_TITLE_OFFSET_Y * y
		);

		return true;
	}

	/// <summary>
	///     Sets active the title screen
	/// </summary>
	/// <param name="isActive"></param>
	public static void SetActiveTitle(bool isActive) => GameObject.Find(Constants.TITLE_PATH)?.SetActive(isActive);
}