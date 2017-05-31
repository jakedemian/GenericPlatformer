using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities {

    public static float getDistanceBetweenTwoPoints(Vector2 a, Vector2 b) {
        return Mathf.Sqrt(Mathf.Pow(a.x - b.x, 2) + Mathf.Pow(a.y - b.y, 2));
    }

    public static void ClearConsole() {
        var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
        var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        clearMethod.Invoke(null, null);
    }
}
