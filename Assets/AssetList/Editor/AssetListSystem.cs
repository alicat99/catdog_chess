using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetListSystem : AssetPostprocessor
{
    static List<AssetList> updated = new List<AssetList>();

    static void OnPostprocessAllAssets(
        string[] importedAssets,
        string[] deletedAssets,
        string[] movedAssets,
        string[] movedFromPath)
    {
        foreach (string t in importedAssets)
            OnImported(t);

        foreach (string t in deletedAssets)
            OnDelected(t);

        foreach (string t in movedFromPath)
            OnDelected(t);

        foreach (string t in movedAssets)
            OnImported(t);

        foreach (AssetList assetList in updated)
            assetList.ResetList();
        updated.Clear();
    }

    static void OnImported(string filePath)
    {
        string parentFolderPath = filePath.Substring(0, filePath.LastIndexOf('/'));
        string parentparentPath = parentFolderPath.Substring(0, parentFolderPath.LastIndexOf('/'));

        AssetList targetList = FindAssetList(parentparentPath, parentFolderPath);

        if (!targetList) return;

        updated.Add(targetList);
    }

    static void OnDelected(string filePath)
    {
        string parentFolderPath = filePath.Substring(0, filePath.LastIndexOf('/'));
        string parentparentPath = parentFolderPath.Substring(0, parentFolderPath.LastIndexOf('/'));

        AssetList targetList = FindAssetList(parentparentPath, parentFolderPath);

        if (!targetList) return;

        updated.Add(targetList);
    }

    static AssetList FindAssetList(string fromPath, string targetPath)
    {
        AssetList targetList = null;
        string[] files = Directory.GetFiles(fromPath, "*.asset", SearchOption.TopDirectoryOnly);
        foreach (string t in files)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<AssetList>(t);

            if (asset == null) continue;
            AssetList assetList = (AssetList)asset;

            if (assetList.targetPath == targetPath)
            {
                targetList = assetList;
                break;
            }
        }

        return targetList;
    }
}
