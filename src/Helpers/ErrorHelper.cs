using System;
using System.Collections.Generic;
using System.Reflection;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace AgriCore.Helpers;

/// <summary>
///     Class helping with the generation of errors
/// </summary>
public static class ErrorHelper
{
	private const string ERROR_PREFIX               = nameof(AgriCore) + "_error";
	public const  string WRONG_ARGUMENTS_ERROR      = ERROR_PREFIX + "_wrong_args_detailed";
	public const  string WRONG_ARGUMENT_COUNT_ERROR = ERROR_PREFIX + "_wrong_number_args_detailed";

	private static Exception MakeException(
		string          s,
		params object[] args
	) =>
		new ExecuteException(CodeUtilities.LocalizeAndFormat(s, args));

	/// <summary>
	///     Generates an exception for the wrong number of arguments given
	/// </summary>
	/// <param name="functionName">Name of the function</param>
	/// <param name="expectedArguments">Types of arguments expected</param>
	/// <param name="actualArguments">Arguments given</param>
	public static Exception WrongNumberOfArguments(
		string          functionName,
		ParameterInfo[] expectedArguments,
		List<IPyObject> actualArguments
	)
	{
		var expectedNames = new List<string>();
		var actualNames   = new string[actualArguments.Count];

		foreach (ParameterInfo expected in expectedArguments) {
			if (expected.IsOptional) continue;

			if (expected.GetCustomAttribute<ParamArrayAttribute>() != null) continue;

			expectedNames.Add(expected.ParameterType.Name);
		}

		for (var i = 0; i < actualArguments.Count; i++)
			actualNames[i] = CodeUtilities.ToNiceString(actualArguments[i], isSequenceElement: true);

		return MakeException(
			WRONG_ARGUMENT_COUNT_ERROR,
			functionName + "()",
			expectedNames.Count,
			string.Join(", ", expectedNames),
			string.Join(", ", actualNames)
		);
	}

	/// <summary>
	///     Generates an exception for the wrong type of argument given
	/// </summary>
	/// <param name="functionName">Name of the function</param>
	/// <param name="expectedArgument">Type of the argument expected</param>
	/// <param name="actualArgument">Argument given</param>
	/// <param name="index">Index of the argument</param>
	public static Exception WrongArgumentType(
		string    functionName,
		Type      expectedArgument,
		IPyObject actualArgument,
		int       index
	) =>
		MakeException(
			WRONG_ARGUMENTS_ERROR,
			functionName + "()",
			expectedArgument.Name,
			index,
			CodeUtilities.ToNiceString(actualArgument, isSequenceElement: true)
		);
}