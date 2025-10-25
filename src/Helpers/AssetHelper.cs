using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AgriCore.Helpers;

/// <summary>
///     Class helping to load bundles and assets
/// </summary>
public static class AssetHelper
{
	private static readonly Dictionary<string, AssetBundle> LoadedBundles = new();

	/// <summary>
	///     Loads the given bundle
	/// </summary>
	public static AssetBundle LoadBundle(string name)
	{
		if (LoadedBundles.TryGetValue(name, out AssetBundle? bundle)) return bundle;

		var     assembly = Assembly.GetCallingAssembly();
		Stream? stream   = assembly.GetManifestResourceStream(name);

		if (stream == null) throw new NullReferenceException($"No resource was found for '{name}'.");

		return LoadedBundles[name] = AssetBundle.LoadFromStream(stream);
	}

	/// <summary>
	///     Loads the given asset from the given bundle
	/// </summary>
	public static T LoadAsset<T>(string bundleName, string assetName) where T : Object =>
		LoadBundle(bundleName).LoadAsset<T>(assetName);

	/// <summary>
	///     Loads the given image and creates a texture from it
	/// </summary>
	public static Texture2D? GetTexture<T>(string name)
	{
		// Read bytes
		Stream? stream = typeof(T).Assembly.GetManifestResourceStream(name);

		if (stream == null) return null;

		using var memoryStream = new MemoryStream();
		stream.CopyTo(memoryStream);

		byte[] bytes = memoryStream.ToArray();

		// If no content found, skip
		if (bytes.Length == 0) return null;

		// Create texture
		var t = new Texture2D(1, 1);

		t.LoadImage(bytes);

		//t.LoadImage(bytes);

		return t;
	}
}