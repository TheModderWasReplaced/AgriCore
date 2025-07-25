using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using AgriCore.Helpers;
using HarmonyLib;

namespace AgriCore.Patches;

[HarmonyPatch(typeof(CodeUtilities))]
internal static class CodeUtilities_Patches
{
    [HarmonyPatch(nameof(CodeUtilities.SyntaxColor2), typeof(string), typeof(string), typeof(int)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> SyntaxColor2_HarmonyTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        var code = new List<CodeInstruction>(instructions);
        
        var insertionIndex = -1;
        for (var i = 0; i < code.Count - 1; i++)
        {
            var instruction = code[i];
            
            if (instruction.opcode != OpCodes.Stloc_2)
                continue;
            
            if (code[i + 1].opcode != OpCodes.Ldarg_0)
                continue;

            insertionIndex = i + 1;
            break;
        }

        if (insertionIndex == -1)
        {
            Log.Warning($"Failed to transpile the code at '{nameof(CodeUtilities.SyntaxColor2)}'.");
            return code;
        }

        var injected = new CodeInstruction[]
        {
            new(OpCodes.Call, AccessTools.Method(typeof(ColorHelper), nameof(ColorHelper.GetRegexString))),
            new(OpCodes.Ldstr, "|"),
            new(OpCodes.Ldloc_2),
            new(OpCodes.Call, AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string), typeof(string)])),
            new(OpCodes.Stloc_2)
        };
        
        code.InsertRange(insertionIndex, injected);

        return code;
    }
    
    public static IEnumerable<CodeInstruction> ParseGroupToColor(IEnumerable<CodeInstruction> instructions, ILGenerator il)
    {
        var code = new List<CodeInstruction>(instructions);
        
        var insertionIndex = -1;
        
        for (var i = 0; i < code.Count - 4; i++)
        {
            var instruction = code[i];

            if (instruction.opcode != OpCodes.Ldloc_0)
                continue;
            
            if (code[i + 3].opcode != OpCodes.Ldstr)
                continue;
            
            if ((string)code[i + 3].operand != "comment")
                continue;
            
            insertionIndex = i;
            break;
        }

        if (insertionIndex == -1)
        {
            Log.Warning($"Failed to transpile the code at '{nameof(CodeUtilities.SyntaxColor2)}'.");
            return code;
        }

        var resultLocal = il.DeclareLocal(typeof(string));
        var continueLabel = il.DefineLabel();
        var matchField = ReflectionHelper.GetLocalField(typeof(CodeUtilities), "m");

        var injected = new CodeInstruction[]
        {
            new(OpCodes.Ldloc_0),
            new(OpCodes.Ldfld, matchField),
            new(OpCodes.Call, AccessTools.Method(typeof(ColorHelper), nameof(ColorHelper.GetColoredText), [typeof(Match)])),
            new(OpCodes.Stloc, resultLocal.LocalIndex),
            new(OpCodes.Ldloc, resultLocal.LocalIndex),
            new(OpCodes.Brfalse_S, continueLabel),
            new(OpCodes.Ldloc, resultLocal.LocalIndex),
            new(OpCodes.Ret)
        };

        (injected[0].labels, code[insertionIndex].labels) = (code[insertionIndex].labels, injected[0].labels);
        code[insertionIndex].labels.Add(continueLabel);

        code.InsertRange(insertionIndex, injected);
        
        return code;
    }
}