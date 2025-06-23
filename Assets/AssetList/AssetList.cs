using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CreateAssetMenu(fileName = "newAssetList", menuName = "ScriptableObject/AssetList", order = 1001)]
public class AssetList : ScriptableObject
{
    public string targetPath;
    public Object[] data;

#if UNITY_EDITOR
    public virtual void ResetList()
    {
        List<Object> list = new List<Object>();

        if (!Directory.Exists(targetPath))
        {
            data = null;
            return;
        }

        string[] files = Directory.GetFiles(targetPath, "*", SearchOption.TopDirectoryOnly);
        foreach (string t in files)
        {
            AddAsset(list, t);
        }

        data = list.ToArray();

        string path = AssetDatabase.GetAssetPath(this);
        string targetPathName = Path.GetFileName(targetPath);
        if (Path.GetFileName(path) != targetPathName)
            AssetDatabase.RenameAsset(path, GetName(targetPathName));

        EditorUtility.SetDirty(this);
    }

    protected virtual void AddAsset(List<Object> list, string path)
    {
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);
        if (asset != null)  list.Add(asset);
    }
#endif

    protected virtual string GetName(string targetPathName)
    {
        return targetPathName + " ";
    }
}
