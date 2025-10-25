using System;
using AgriCore.Helpers;
using BepInEx;
using UnityEngine;
using UnityEngine.UI;

namespace AgriCore.Extensions;

/// <summary>
///     Class helping for everything concerning images
/// </summary>
public static class ImageExtensions
{
	/// <inheritdoc cref="ImageExtensions.LoadSprite{T}(Image, string, Rect, Vector2, float)" />
	public static void LoadSprite<T>(
		this Image image,
		string     name,
		float      size,
		float      pixelsPerUnit = 100
	) where T : BaseUnityPlugin =>
		image.LoadSprite<T>(
			name, new Rect(Vector2.zero, Vector2.one * size), Vector2.zero,
			pixelsPerUnit
		);

	/// <summary>
	///     Tries to set the given asset to the image
	/// </summary>
	/// <param name="image"></param>
	/// <param name="name">Path of the resources from the root</param>
	/// <param name="rect"></param>
	/// <param name="pivot">Pivot of the sprite</param>
	/// <param name="pixelsPerUnit"></param>
	private static void LoadSprite<T>(
		this Image image,
		string     name,
		Rect       rect,
		Vector2    pivot,
		float      pixelsPerUnit
	) where T : BaseUnityPlugin
	{
		Texture2D? t = AssetHelper.GetTexture<T>(name);

		// If no texture found
		if (t == null) {
			Log.Warning($"No texture found for '{name}'.");
			image.sprite = null;
			return;
		}

		t.name = name;

		// If rect too big for texture
		if (t.height < rect.height || t.width < rect.width) {
			rect.height = Math.Min(t.height, rect.height);
			rect.width  = Math.Min(t.width, rect.width);
			Log.Debug($"Resized rect to fit: {rect.height}x{rect.width}");
		}

		// Create sprite
		image.sprite = Sprite.Create(
			t,
			rect,
			pivot,
			pixelsPerUnit
		);
	}
}