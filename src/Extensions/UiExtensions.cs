using System;
using AgriCore.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace AgriCore.Extensions;

/// <summary>
///     Class helping for creating UI
/// </summary>
public static class UiExtensions
{
	/// <summary>
	///     Adds an input field prefab to this parent
	/// </summary>
	public static TMP_InputField AddInputField(this Object parent) =>
		parent.Add("InputField").GetComponent<TMP_InputField>();

	/// <summary>
	///     Adds a panel prefab to this parent
	/// </summary>
	public static GameObject AddPanel(this Object parent) => parent.Add("Panel");

	/// <summary>
	///     Adds a scrollbar prefab to this parent
	/// </summary>
	public static Scrollbar AddScrollbar(this Object parent) => parent.Add("Scrollbar").GetComponent<Scrollbar>();

	/// <summary>
	///     Adds a scroll view prefab to this parent
	/// </summary>
	public static ScrollRect AddScrollView(this Object parent) => parent.Add("ScrollView").GetComponent<ScrollRect>();

	/// <summary>
	///     Adds a button prefab to this parent
	/// </summary>
	public static ColoredButton AddButton(this Object parent) => parent.Add("Button").GetComponent<ColoredButton>();

	/// <summary>
	///     Fetches the requested asset and creates a copy
	/// </summary>
	private static GameObject Add(this Object parent, string name)
	{
		var asset = AssetHelper.LoadAsset<GameObject>(Constants.UI_BUNDLE, name);

		if (asset == null) throw new NullReferenceException($"No asset named '{name}' was found.");

		Transform? transform = parent switch {
			Component o    => o.transform,
			GameObject obj => obj.transform,
			_              => null,
		};

		return Object.Instantiate(asset, transform);
	}
}