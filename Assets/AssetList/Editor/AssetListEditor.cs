using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AssetList), true)]
public class AssetListEditor : Editor
{
    SerializedProperty propertyData;

    AssetList targetComponent;

    public void OnEnable()
    {
        serializedObject.Update();
        propertyData = serializedObject.FindProperty(nameof(AssetList.data));

        targetComponent = (AssetList)target;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button("Select Folder"))
        {
            string path = AssetDatabase.GetAssetPath(targetComponent);
            string parentPath = path.Substring(0, path.LastIndexOf('/'));

            string folderPath = EditorUtility.OpenFolderPanel("Select Target Folder", parentPath, "");
            if (!string.IsNullOrEmpty(folderPath))
            {
                folderPath = "Assets" + folderPath.Substring(Application.dataPath.Length);

                targetComponent.targetPath = folderPath;
            }

            targetComponent.ResetList();
            serializedObject.Update();
        }

        EditorGUILayout.Space(16);

        GUI.enabled = false;
        EditorGUILayout.PropertyField(propertyData);
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }
}
