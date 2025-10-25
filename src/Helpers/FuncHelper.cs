using System;
using System.Collections.Generic;
using System.Reflection;
using AgriCore.API.Attributes;

namespace AgriCore.Helpers;

/// <summary>
///     Class helping with the addition of new built-in functions
/// </summary>
public static class FuncHelper
{
	/// <summary>
	///     Adds the given callback as a built-in function
	/// </summary>
	/// <param name="name">Name of the function to call</param>
	/// <param name="callback">Code to run when executing the function</param>
	/// <param name="color">Color of the text used for the function</param>
	/// <returns>Succeeded to add the function</returns>
	private static bool Add(
		string                                                    name,
		Func<List<IPyObject>, Simulation, Execution, int, double> callback,
		string?                                                   color = null
	)
	{
		var newFunction = new PyFunction(
			name,
			callback
		);

		BuiltinFunctions.functionList.Add(newFunction);
		Farm.startUnlocks.Add(name.ToLower());

		return color == null || ColorHelper.Add(name + @"(?=\(.*?)", color, $"func_helper_{name}_color");
	}

	/// <summary>
	///     Adds a built-in function from the given method
	/// </summary>
	/// <param name="name">Name of the function to call</param>
	/// <param name="callback">Code to run when executing the function</param>
	/// <param name="color">Color of the text used for the function</param>
	/// <returns>Succeeded to add the function</returns>
	public static bool AddMethod(string name, Delegate callback, string? color = null)
	{
		ParameterInfo[] parameters                           = callback.Method.GetParameters();
		var             types                                = new Type[parameters.Length];
		for (var i = 0; i < parameters.Length; i++) types[i] = parameters[i].ParameterType;

		ParameterInfo[] parsableParameters = ParamHelper.FilterParsableParameters(parameters);

		return Add(
			name, MethodHandler, color
		);

		double MethodHandler(
			List<IPyObject> @params,
			Simulation      simulation,
			Execution       execution,
			int             droneId
		)
		{
			ParamHelper.AddOptionalParameters(@params, parsableParameters);
			ParamHelper.CompileParamsParameters(@params, parsableParameters);

			// Too many parameters
			if (@params.Count != parsableParameters.Length)
				throw ErrorHelper.WrongNumberOfArguments(name, parsableParameters, @params);

			// Wrong types
			for (var i = 0; i < parsableParameters.Length; i++) {
				IPyObject? param     = @params[i];
				Type       paramType = parsableParameters[i].ParameterType;

				if (!ParamHelper.IsSameType(paramType, param)) {
					throw ErrorHelper.WrongArgumentType(
						name, paramType, param,
						i + 1
					);
				}
			}

			object[] arguments = ParamHelper.ConvertParameters(
				@params,
				types,
				simulation,
				execution
			);

			// Call the method
			object? result = callback.DynamicInvoke(arguments);

			// Return delay
			return result as double? ?? Execution.ACTION_OPS;
		}
	}

	/// <summary>
	///     Adds a built-in function from the given method
	/// </summary>
	/// <param name="callback">Code to run when executing the function</param>
	/// <returns>Succeeded to add the function</returns>
	/// <remarks>
	///     The given method must have the attribute <see cref="PyFunctionAttribute" /> in order to be accepted here
	/// </remarks>
	public static bool AddMethod(Delegate callback)
	{
		MethodInfo methodInfo = callback.Method;
		var        attribute  = methodInfo.GetCustomAttribute<PyFunctionAttribute>();

		if (attribute == null) {
			Log.Warning($"'{methodInfo.Name}' does not contain the attribute '{nameof(PyFunctionAttribute)}'.");
			return false;
		}

		return AddMethod(
			attribute.Name,
			callback,
			attribute.Color
		);
	}
}