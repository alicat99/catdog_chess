using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ScriptableObjectList", menuName = "ScriptableObject/ScriptableObjectList", order = 1000)]
public class ScriptableObjectList : TypeAssetList<ScriptableObject>
{
    protected override string GetName(string targetPathName)
    {
        return targetPathName + " (ScriptableObject)";
    }
}
