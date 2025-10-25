using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AgriCore.Helpers;

/// <summary>
///     Class helping with the handling of parameters
/// </summary>
internal static class ParamHelper
{
	/// <summary>
	///     List of every type and their Python equivalent
	/// </summary>
	private static readonly Dictionary<Type, Type> TypeConverts = new() {
		[typeof(sbyte)]         = typeof(PyNumber),
		[typeof(byte)]          = typeof(PyNumber),
		[typeof(char)]          = typeof(PyNumber),
		[typeof(short)]         = typeof(PyNumber),
		[typeof(ushort)]        = typeof(PyNumber),
		[typeof(int)]           = typeof(PyNumber),
		[typeof(uint)]          = typeof(PyNumber),
		[typeof(long)]          = typeof(PyNumber),
		[typeof(ulong)]         = typeof(PyNumber),
		[typeof(double)]        = typeof(PyNumber),
		[typeof(float)]         = typeof(PyNumber),
		[typeof(decimal)]       = typeof(PyNumber),
		[typeof(bool)]          = typeof(PyBool),
		[typeof(string)]        = typeof(PyString),
		[typeof(GridDirection)] = typeof(PyGridDirection),
	};

	/// <summary>
	///     Filters the given parameters to only keep the parameters that are parsable
	/// </summary>
	/// <param name="parameterInfos"></param>
	/// <returns></returns>
	public static ParameterInfo[] FilterParsableParameters(params ParameterInfo[] parameterInfos)
	{
		var parsableTypes = new List<ParameterInfo>();

		foreach (ParameterInfo parameterInfo in parameterInfos) {
			Type? type = parameterInfo.ParameterType;

			if (type.IsArray) type = type.GetElementType();

			if (type != null && TypeConverts.ContainsKey(type)) parsableTypes.Add(parameterInfo);
		}

		return parsableTypes.ToArray();
	}

	/// <summary>
	///     Checks if the given object is the same type as the given property
	/// </summary>
	public static bool IsSameType(Type type, IPyObject other)
	{
		if (TypeConverts.TryGetValue(type, out Type? convert) && convert.IsInstanceOfType(other)) return true;

		if (type.IsArray && other is PyList list) {
			Type? elementType = type.GetElementType();
			if (elementType == null) return false;

			foreach (IPyObject? subOther in list.list)
				if (!IsSameType(elementType, subOther))
					return false;

			return true;
		}

		return false;
	}

	/// <summary>
	///     Adds the optional parameters that were not specified
	/// </summary>
	/// <param name="params">Original parameters</param>
	/// <param name="pInfos">Parameter information for each parameter</param>
	public static void AddOptionalParameters(List<IPyObject> @params, ParameterInfo[] pInfos)
	{
		for (int i = @params.Count; i < pInfos.Length; i++) {
			if (!pInfos[i].IsOptional) break;

			if (!TypeConverts.TryGetValue(pInfos[i].ParameterType, out Type? convertedType)) continue;

			@params.Add(
				(IPyObject)Activator.CreateInstance(convertedType, pInfos[i].DefaultValue)
			);
		}
	}

	/// <summary>
	///     Compiles the additional parameters into a <c>params</c> parameter
	/// </summary>
	/// <param name="params">Original parameters</param>
	/// <param name="pInfos">Parameter information for each parameter</param>
	public static void CompileParamsParameters(List<IPyObject> @params, ParameterInfo[] pInfos)
	{
		if (pInfos.Length == 0) return;

		ParameterInfo lastParam           = pInfos[pInfos.Length - 1];
		var           paramArrayAttribute = lastParam.GetCustomAttribute<ParamArrayAttribute>();

		if (paramArrayAttribute == null || !lastParam.ParameterType.IsArray) return;

		if (@params.Count == 0) return;

		if (@params.Count < pInfos.Length - 1) return;

		var extraParams = new IPyObject[@params.Count - pInfos.Length + 1];

		for (int i = @params.Count - 1; i >= pInfos.Length - 1; i--) {
			extraParams[i - pInfos.Length + 1] = @params[i];
			@params.RemoveAt(i);
		}

		if (extraParams.Length == 1 && extraParams[0] is PyList list)
			@params.Add(list);
		else
			@params.Add(new PyList(extraParams.ToList()));
	}

	/// <summary>
	///     Converts the given value into the given type
	/// </summary>
	/// <param name="value">Value to convert</param>
	/// <param name="wanted">Type to convert to</param>
	private static object? ConvertParameter(object value, Type wanted)
	{
		// If value already correct type, return value
		if (wanted.IsInstanceOfType(value)) return value;

		// Try to convert value to wanted type
		return value switch {
			PyBool boolean            => boolean.num != 0,                       // PyBool -> bool
			PyNumber number           => Convert.ChangeType(number.num, wanted), // PyNumber -> number
			PyString text             => text.str,                               // PyString -> string
			PyGridDirection direction => direction.dir,                          // PyGridDirection -> GridDirection
			_                         => null,
		};
	}

	/// <summary>
	///     Converts the given parameters into the parameters required for the function
	/// </summary>
	/// <param name="params">Original parameters</param>
	/// <param name="types">Types of the function</param>
	/// <param name="globals">Global values for accessible from every function</param>
	public static object[] ConvertParameters(
		List<IPyObject>  @params,
		Type[]           types,
		params object?[] globals
	)
	{
		var                        arguments  = new object[types.Length];
		List<IPyObject>.Enumerator paramsEnum = @params.GetEnumerator();

		for (var i = 0; i < types.Length; i++) {
			Type type = types[i];

			// Check for global
			object? globalValue = globals.OfType<object>().FirstOrDefault(global => global.GetType() == type);

			if (globalValue != null) {
				arguments[i] = globalValue;
				continue;
			}

			if (paramsEnum.Current == null) continue;

			object? value = null;

			// Check for array
			if (type.IsArray) {
				Type? elementType = type.GetElementType();

				if (elementType != null && paramsEnum.Current is PyList list) {
					var tempArgs = Array.CreateInstance(elementType, list.list.Count);

					for (var j = 0; j < tempArgs.Length; j++) {
						tempArgs.SetValue(
							ConvertParameter(list.list[j], elementType),
							j
						);
					}

					value = tempArgs;
				}
			}

			// Check for conversion
			else if (TypeConverts.ContainsKey(type)) value = ConvertParameter(paramsEnum.Current, type);

			arguments[i] = value ?? paramsEnum.Current;
		}

		paramsEnum.Dispose();

		return arguments;
	}
}