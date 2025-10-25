using System;
using System.Linq;
using AgriCore.API.Attributes;
using AgriCore.Helpers;

// ReSharper disable UnusedMember.Local

namespace AgriCore;

internal static class ModHelperFunctions
{
	internal static void LoadAll()
	{
		FuncHelper.AddMethod(Pow);
		FuncHelper.AddMethod(Floor);
		FuncHelper.AddMethod(Round);
		FuncHelper.AddMethod(Avg);

		LocalizerHelper.Add(
			"code_tooltip_pow", """
			                    `pow(x, y)`
			                    Returns the number `x` raised to the power `y`.

			                    Examples:
			                    `pow(2, 2)   # 4`
			                    `pow(5, 10)  # 9765625 `
			                    `pow(10, -2) # 0.01`

			                    Takes the time of `1` operations to execute.
			                    """
		);
		LocalizerHelper.Add(
			"code_tooltip_floor", """
			                      `floor(x)`
			                      Returns the largest integral value less than or equal to `x`.

			                      Examples:
			                      `floor(1.5) # 1`
			                      `floor(1)   # 1`
			                      `floor(2.9) # 2`
			                      `floor(0.9) # 0`

			                      Takes the time of `1` operations to execute.
			                      """
		);
		LocalizerHelper.Add(
			"code_tooltip_round", """
			                      `round(x)`
			                      Returns the nearest integer from `x`.

			                      Examples:
			                      `round(1.5)  # 2`
			                      `round(2.1)  # 2`

			                      Takes the time of `1` operations to execute.
			                      """
		);
	}

	[PyFunction("pow", "#33b5aa")]
	private static double Pow(Execution execution, double a, double b)
	{
		double result = Math.Pow(a, b);
		execution.MainState.ReturnValue = new PyNumber(result);
		return Execution.OPERATION_OPS;
	}

	[PyFunction("floor", "#33b5aa")]
	private static double Floor(Execution execution, double x)
	{
		double result = Math.Floor(x);
		execution.MainState.ReturnValue = new PyNumber(result);
		return Execution.OPERATION_OPS;
	}

	[PyFunction("round", "#33b5aa")]
	private static double Round(Execution execution, double x)
	{
		double result = Math.Round(x);
		execution.MainState.ReturnValue = new PyNumber(result);
		return Execution.OPERATION_OPS;
	}

	[PyFunction("avg", "#33b5aa")]
	private static double Avg(
		Execution       execution,
		double          a,
		double          b,
		params double[] args
	)
	{
		double total = (a + b + args.Sum()) / (args.Length + 2);

		execution.MainState.ReturnValue = new PyNumber(total);
		return Execution.OPERATION_OPS;
	}
}