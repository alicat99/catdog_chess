using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateScriptableObjectInstence : MonoBehaviour
{
    [MenuItem("Assets/Create/ScriptableObject/CreateInstence")]
    static void Create()
    {
        UnityEngine.Object activeObject = Selection.activeObject;
        if (activeObject == null) return;
        if (activeObject.GetType() == typeof(MonoScript))
        {
            try
            {
                MonoScript script = (MonoScript)activeObject;
                Type t = script.GetClass();
                if (typeof(ScriptableObject).IsAssignableFrom(t))
                {
                    var asset = ScriptableObject.CreateInstance(t);
                    string p = AssetDatabase.GetAssetPath(activeObject.GetInstanceID());
                    p = p.Substring(0, p.LastIndexOf('/'));
                    AssetDatabase.CreateAsset(asset, p + "/" + script.name + ".asset");
                    AssetDatabase.Refresh();
                }
                return;
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
                return;
            }
        }
        return;
    }

    [MenuItem("Assets/Create/ScriptableObject/CreateInstence", isValidateFunction: true)]
    static bool ValidateCreate()
    {
        UnityEngine.Object activeObject = Selection.activeObject;
        if (activeObject == null) return false;
        if(activeObject.GetType() == typeof(MonoScript))
        {
            try
            {
                MonoScript script = (MonoScript)activeObject;
                if (typeof(ScriptableObject).IsAssignableFrom(script.GetClass()))
                    return true;
            }
            catch
            {
                return false;
            }
        }
        return false;
    }
}
