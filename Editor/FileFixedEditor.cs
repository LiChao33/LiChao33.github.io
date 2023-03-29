using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Fixed;

public class FileFixedEditor : Editor
{
    private static Dictionary<string, FileInfo> PngDic = new Dictionary<string, FileInfo>();
    private static Dictionary<string, FileInfo> MetaDic = new Dictionary<string, FileInfo>();
    private static int RandomCoeff = 3;

    public static void Fixed(bool isRename)
    {
        if (Selection.assetGUIDs != null)
        {
            if (Selection.assetGUIDs.Length != 1)
            {
                EditorUtility.DisplayDialog("错误", "只能选择单个文件夹。", "确认");
                return;
            }
            DirectoryInfo dir = new DirectoryInfo(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]));
            DirectoryInfo[] deleteDir = dir.GetDirectories();
            int dirLength = 0;
            SetDirection(new List<DirectoryInfo>() { dir }, ref dirLength);
            CreatFiles(dir.FullName + "/", dirLength * UnityEngine.Random.Range(RandomCoeff, RandomCoeff * 2), isRename);
            DeleteNullFiles(deleteDir);
            AssetDatabase.Refresh();
        }
    }

    private static void SetDirection(List<DirectoryInfo> dirs, ref int dirLength)
    {
        List<DirectoryInfo> dir = new List<DirectoryInfo>();
        dirLength += dirs.Count;
        for (int i = 0; i < dirs.Count; i++)
        {
            if (dirs[i].GetDirectories().Length != 0)
            {
                dir.AddRange(dirs[i].GetDirectories().ToList());
            }
            var files = dirs[i].GetFiles();
            for (int j = 0; j < files.Length; j++)
            {
                string name = dirs[i].Name + "/" + files[j].Name.Substring(0, files[j].Name.IndexOf('.'));
                if (files[j].Extension.Equals(".meta"))
                {
                    if (MetaDic.ContainsKey(name))
                        Debug.LogError("有重复名称资源：" + name);
                    else
                        MetaDic.Add(name, files[j]);
                }
                else
                {
                    if (PngDic.ContainsKey(name))
                        Debug.LogError("有重复名称资源：" + name);
                    else
                        PngDic.Add(name, files[j]);
                }
            }
        }
        if (dir.Count != 0)
        {
            SetDirection(dir, ref dirLength);
            return;
        }
    }


    private static void CreatFiles(string path, int num, bool isRename = false)
    {
        List<DirectoryInfo> dirs = new List<DirectoryInfo>();
        for (int i = 0; i < num; i++)
        {
            dirs.Add(Directory.CreateDirectory(path + Guid.NewGuid().ToString("N").Remove(UnityEngine.Random.Range(5, 8))));
        }
        Thread.Sleep(1000);
        foreach (var data in PngDic)
        {
            var index = UnityEngine.Random.Range(0, num);
            if (isRename)
            {
                string str = Guid.NewGuid().ToString("N").Remove(UnityEngine.Random.Range(5, 8));
                data.Value.MoveTo(dirs[index].FullName + "/" + str + data.Value.Name);
                MetaDic[data.Key].MoveTo(dirs[index].FullName + "/" + str + MetaDic[data.Key].Name);
            }
            else
            {
                data.Value.MoveTo(dirs[index].FullName + "/" + data.Value.Name);
                MetaDic[data.Key].MoveTo(dirs[index].FullName + "/" + MetaDic[data.Key].Name);
            }
        }
    }

    private static void DeleteNullFiles(DirectoryInfo[] deleteDir)
    {
        for (int i = deleteDir.Length - 1; i >= 0; i--)
        {
            Debug.LogError(deleteDir[i].FullName);
            FileInfo file = new FileInfo(deleteDir[i].FullName + ".meta");
            file.Delete();
            deleteDir[i].Delete(true);
        }
    }
}
