using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class NameEditor : Editor
{
    [MenuItem("Tools/NameConfig")]
    public static void Excute() {
        string filepath = "name_list";
        TextAsset textasset = Resources.Load<TextAsset>(filepath);
        NameConfig config = Resources.Load<NameConfig>("Data/NameConfig");
        config.data_cache = new List<string>();
        config.data_cache.AddRange(textasset.text.Split(','));
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}
