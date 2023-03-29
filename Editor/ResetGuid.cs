/* 重新生成GUID 修改mate 重新绑定引用关系
 * @Author: Chao_Li
 * @Date:2022-11-14
 */
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetGuid
{
    public static void RegenerateGuids()
    {
        if (EditorSettings.serializationMode != SerializationMode.ForceText)
        {
            if (EditorUtility.DisplayDialog("重新生成所有资源的GUID", "序列化模式需要改为Text，是否更换？", "更换"))
            {
                EditorSettings.serializationMode = SerializationMode.ForceText;
                AssetDatabase.Refresh();
            }
        }
        else
        {
            if (SceneManager.GetActiveScene().GetRootGameObjects().Length > 1)
            {
                EditorUtility.DisplayDialog("重新生成所有资源的GUID", "非空场景，不能操作", "确认");
            }
            else
            {
                if (EditorUtility.DisplayDialog("重新生成所有资源的GUID", "切记打开一个空场景在进行该操作!!!，需要遗忘的文件夹自行填写至IgnoreList！！", "确认操作"))
                {
                    try
                    {
                        AssetDatabase.StartAssetEditing();

                        string path = Path.GetFullPath(".") + Path.DirectorySeparatorChar + "Assets";
                        UnityGuidRegenerator regenerator = new UnityGuidRegenerator(path);
                        regenerator.RegenerateGuids();
                    }
                    finally
                    {
                        AssetDatabase.StopAssetEditing();
                        EditorUtility.ClearProgressBar();
                        AssetDatabase.Refresh();
                    }
                }
            }
        }
    }
}

internal class UnityGuidRegenerator
{
    private static readonly string[] kDefaultFileExtensions = {
            "*.meta",
            "*.mat",
            "*.anim",
            "*.prefab",
            "*.unity",
            "*.asset",
            "*.guiskin",
            "*.fontsettings",
            "*.controller",
            "*.spriteatlas",
            "*.ttf",
            "*.TTF",
            "*.wav",
            "*.mp3",
            "*.ogg"
        };

    public static string[] ignoreList = new string[]{
            "Editor",
            "Plugins",
            // sdk
            "AppsFlyer",
            "ExternalDependencyManager",
            "Firebase",
            "Editor Default Resources",
            "DataEyeAnalytics",
            "MaxSdk",
            "DoTween",
            "SDK",
        };

    private readonly string _assetsPath;

    List<string> prefabPathLst = new List<string>();
    List<string> scenePathLst = new List<string>();

    public UnityGuidRegenerator(string assetsPath)
    {
        _assetsPath = assetsPath;
    }

    public void RegenerateGuids(string[] regeneratedExtensions = null)
    {

        GetAllReplacePath();

        if (regeneratedExtensions == null)
        {
            regeneratedExtensions = kDefaultFileExtensions;
        }

        List<string> filesPaths = new List<string>();
        foreach (string extension in regeneratedExtensions)
        {
            filesPaths.AddRange(
                Directory.GetFiles(_assetsPath, extension, SearchOption.AllDirectories)
                );
        }

        Dictionary<string, string> guidOldToNewMap = new Dictionary<string, string>();
        Dictionary<string, List<string>> guidsInFileMap = new Dictionary<string, List<string>>();

        HashSet<string> ownGuids = new HashSet<string>();

        int counter = 0;
        foreach (string filePath in filesPaths)
        {
            var relpath = "Assets" + filePath.Substring(Application.dataPath.Length);
            UnityEngine.Debug.Log("relpath:" + relpath);
            var ignore = false;
            foreach (var item in ignoreList)
            {
                if (relpath.Contains(item))
                {
                    ignore = true;
                    break;
                }
            }
            if (ignore)
            {
                continue;
            }


            if (!EditorUtility.DisplayCancelableProgressBar("Scanning Assets folder", MakeRelativePath(_assetsPath, filePath),
                counter / (float)filesPaths.Count))
            {
                string contents = File.ReadAllText(filePath);

                IEnumerable<string> guids = GetGuids(contents);
                bool isFirstGuid = true;
                foreach (string oldGuid in guids)
                {
                    if (isFirstGuid && Path.GetExtension(filePath) == ".meta")
                    {
                        ownGuids.Add(oldGuid);
                        isFirstGuid = false;
                    }
                    if (!guidOldToNewMap.ContainsKey(oldGuid))
                    {
                        string newGuid = Guid.NewGuid().ToString("N");
                        guidOldToNewMap.Add(oldGuid, newGuid);
                    }

                    if (!guidsInFileMap.ContainsKey(filePath))
                        guidsInFileMap[filePath] = new List<string>();

                    if (!guidsInFileMap[filePath].Contains(oldGuid))
                    {
                        guidsInFileMap[filePath].Add(oldGuid);
                    }
                }

                counter++;
            }
            else
            {
                UnityEngine.Debug.LogWarning("GUID regeneration canceled");
                return;
            }
        }

        counter = -1;
        int guidsInFileMapKeysCount = guidsInFileMap.Keys.Count;
        foreach (string filePath in guidsInFileMap.Keys)
        {
            EditorUtility.DisplayProgressBar("Regenerating GUIDs", MakeRelativePath(_assetsPath, filePath), counter / (float)guidsInFileMapKeysCount);
            counter++;

            string contents = File.ReadAllText(filePath);
            foreach (string oldGuid in guidsInFileMap[filePath])
            {
                if (!ownGuids.Contains(oldGuid))
                    continue;

                string newGuid = guidOldToNewMap[oldGuid];
                if (string.IsNullOrEmpty(newGuid))
                    throw new NullReferenceException("newGuid == null");

                contents = contents.Replace("guid: " + oldGuid, "guid: " + newGuid);
            }
            File.WriteAllText(filePath, contents);
        }
        ChangeRefrence(ownGuids, guidOldToNewMap, prefabPathLst);
        ChangeRefrence(ownGuids, guidOldToNewMap, scenePathLst);

        EditorUtility.ClearProgressBar();
    }

    void GetAllReplacePath()
    {
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets" });
        foreach (var item in prefabGUIDs)
            prefabPathLst.Add(AssetDatabase.GUIDToAssetPath(item));
        string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new string[] { "Assets" });
        foreach (var item in sceneGUIDs)
            scenePathLst.Add(AssetDatabase.GUIDToAssetPath(item));
    }

    void ChangeRefrence(HashSet<string> guid, Dictionary<string, string> newGuid, List<string> pathLst)
    {
        string searchText;
        foreach (var itemPath in pathLst)
        {
            string fullPath = Application.dataPath + itemPath.Replace("Assets", "");
            string msg = File.ReadAllText(fullPath);
            bool isHave = false;
            foreach (var oldGuid in guid)
            {
                searchText = string.Format("guid: {0}", oldGuid);
                if (msg.Contains(searchText))
                {
                    msg = msg.Replace(oldGuid, newGuid[oldGuid]);
                    isHave = true;
                }
            }
            if (isHave)
                File.WriteAllText(fullPath, msg);
        }
    }

    private static IEnumerable<string> GetGuids(string text)
    {
        const string guidStart = "guid: ";
        const int guidLength = 32;
        int textLength = text.Length;
        int guidStartLength = guidStart.Length;
        List<string> guids = new List<string>();

        int index = 0;
        while (index + guidStartLength + guidLength < textLength)
        {
            index = text.IndexOf(guidStart, index, StringComparison.Ordinal);
            if (index == -1)
                break;

            index += guidStartLength;
            string guid = text.Substring(index, guidLength);
            index += guidLength;

            if (IsGuid(guid))
            {
                guids.Add(guid);
            }
        }

        return guids;
    }

    private static bool IsGuid(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            if (
                !((c >= '0' && c <= '9') ||
                  (c >= 'a' && c <= 'z'))
                )
                return false;
        }

        return true;
    }

    private static string MakeRelativePath(string fromPath, string toPath)
    {
        Uri fromUri = new Uri(fromPath);
        Uri toUri = new Uri(toPath);

        Uri relativeUri = fromUri.MakeRelativeUri(toUri);
        string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

        return relativePath;
    }
}
