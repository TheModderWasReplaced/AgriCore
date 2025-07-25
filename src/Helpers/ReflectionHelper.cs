using System;
using System.Reflection;
using HarmonyLib;

namespace AgriCore.Helpers;

/// <summary>
/// Class helping with the reflection of classes
/// </summary>
public static class ReflectionHelper
{
    /// <summary>
    /// Finds the lambda method within the given method
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    public static MethodInfo GetLambdaMethod(Type type, string methodName)
    {
        MethodInfo? lambda = null;

        foreach (var types in type.GetNestedTypes(BindingFlags.NonPublic))
        {
            foreach (var method in types.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                if (!method.Name.Contains("b__"))
                    continue;
                
                if (!method.Name.Contains(methodName))
                    continue;
                
                lambda = method;
                break;
            }
            
            if (lambda != null)
                break;
        }

        if (lambda == null)
            throw new NullReferenceException($"Could not find the lambda for '{methodName}'.");

        return lambda;
    }

    /// <summary>
    /// Finds the local field with the given name in the given type
    /// </summary>
    /// <exception cref="NullReferenceException"></exception>
    public static FieldInfo GetLocalField(Type type, string fieldName)
    {
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        
        var displayClass = AccessTools.FirstInner(type, t => t.GetField(fieldName, flags) != null);
        var field = displayClass?.GetField(fieldName, flags);

        if (field == null)
            throw new NullReferenceException($"Could not find the local field '{fieldName}' in the class '{type.Name}'.");

        return field;
    }
}