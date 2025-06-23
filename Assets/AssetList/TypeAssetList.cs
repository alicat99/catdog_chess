using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TypeAssetList<T> : AssetList
    where T : Object
{
#if UNITY_EDITOR
    protected override void AddAsset(List<Object> list, string path)
    {
        Object asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset != null) list.Add(asset);
    }
#endif

    protected override string GetName(string targetPathName)
    {
        return targetPathName + " (TypeAssetList)";
    }
}
