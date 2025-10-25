using System;

namespace AgriCore.API.Attributes;

/// <summary>
///     Information useful for FarmHelper
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class FarmInfoAttribute : Attribute
{
	/// <summary>
	///     The name of the author of this plugin.
	/// </summary>
	public string? Author { get; }

	/// <summary>
	///     URL of the page of this plugin
	/// </summary>
	public string? Url { get; }

	/// <summary>
	///     Informs FarmHelper on certain information for this plugin
	/// </summary>
	/// <param name="author">Name of the author of this plugin</param>
	/// <param name="url">URL of the page of this plugin</param>
	public FarmInfoAttribute(string? author = null, string? url = null)
	{
		Author = author;
		Url    = url;
	}
}