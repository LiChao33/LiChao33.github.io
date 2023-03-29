using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CommonEditor : Editor
{
    public static string GetExcelPath(string _scriptName)
    {
        string[] path = AssetDatabase.FindAssets(_scriptName);
        foreach (var item in path)
        {
            Debug.LogError(AssetDatabase.GUIDToAssetPath(path[0]));
        }
        if (path.Length > 1)
        {
            Debug.LogError("有同名文件" + _scriptName + "获取路径失败,确保查找Excel的唯一性");
            return null;
        }
        //将字符串中得脚本名字和后缀统统去除掉
        string _path = AssetDatabase.GUIDToAssetPath(path[0]);
        _path = Application.dataPath + _path.Remove(_path.LastIndexOf("Assets"), 6);
        return _path;
    }

    public static int FindObjectNumber(string _fileName)
    {
        string[] path = AssetDatabase.FindAssets(_fileName);
        if (path.Length >1)
        {
            Debug.LogError(string.Format("存在相同的“{0}”文件,分别是以下两个文件。", _fileName));
            foreach (var item in path)
            {
                Debug.LogError(AssetDatabase.GUIDToAssetPath(item));
            }
        }

        return path.Length;
    }

    public static string FindSpecifyScriptPath(string _scriptName, bool _containsAssets = false)
    {
        string[] names = AssetDatabase.FindAssets(string.Format("{0} t:Script", _scriptName));
        if (names.Length > 1)
        {
            Debug.LogError("------------------------------------------------------------");
            Debug.LogError("存在两个相同的脚本名称，你可以尝试加上命名空间：" + _scriptName);
            foreach (var item in names)
            {
                Debug.LogError(AssetDatabase.GUIDToAssetPath(item));

            }
            Debug.LogError("------------------------------------------------------------");
            return string.Empty;
        }
        string _scriptPath = _containsAssets ? AssetDatabase.GUIDToAssetPath(names[0]) : AssetDatabase.GUIDToAssetPath(names[0]).Replace("Assets/", string.Empty);
        return _scriptPath;
    }

    [MenuItem("Tools/ClearSave")]
    public static void ClearSave()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log("清除存档");
    }
}
