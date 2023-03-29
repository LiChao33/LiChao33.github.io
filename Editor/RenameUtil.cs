
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
using System.Security.Cryptography;
using System.Text;
using System.Net;

public static class RenameUtil
{
    static int fileSumCount = 0;
    static int progress = 0;
    static string info = string.Empty;

    [MenuItem("Assets/Rename(ѡ���ļ��м����������ļ�����������Դ)")]
    static void RenameFolder()
    {
        List<string> lstPaths = new List<string>();
        List<DirectoryInfo> lstDir = new List<DirectoryInfo>();

        if (Selection.assetGUIDs != null)
        {
            for (int i = 0; i < Selection.assetGUIDs.Length; i++)
            {
                DirectoryInfo direction = new DirectoryInfo(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]));
                //string str = direction.FullName.Substring(direction.FullName.IndexOf("Assets"));
                //string strDir = AssetDatabase.RenameAsset(str, Guid.NewGuid().ToString("N").Remove(UnityEngine.Random.Range(5, 8)));
                //DirectoryInfo direction1 = new DirectoryInfo(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]));
                lstPaths.Add(AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[i]));
                lstDir.Add(direction);
            }
        }
        if (lstDir == null)
        {
            Debug.LogError("δѡ���ļ��У�");
            return;
        }

        //��ʼ�����ļ���
        fileSumCount = 0;
        progress = 0;
        info = string.Empty;

        int index = 1;
        List<string> lstName = new List<string>();

        for (int i = 0; i < lstDir.Count; i++)
        {
            FileInfo[] files = lstDir[i].GetFiles("*", SearchOption.AllDirectories);
            // fileSumCount += files.Length;

            if (files != null)
            {
                for (int j = 0; j < files.Length; j++)
                {
                    lstName.Add(files[j].Name);
                }
            }

        }
        //RunProgressBar();

        for (int i = 0; i < lstDir.Count; i++)
        {
            FileInfo[] files = lstDir[i].GetFiles("*", SearchOption.AllDirectories);
            if (files != null)
            {
                for (int j = 0; j < files.Length; j++)
                {
                    progress++;
                    //������ʾ
                    ShowProgress((j + 1) / files.Length, files.Length, j + 1, files[j].Name);
                    if (files[j].Name.EndsWith(".mp3") || files[j].Name.EndsWith(".wav") || files[j].Name.EndsWith(".ogg") || files[j].Name.EndsWith(".anim")
                        || files[j].Name.EndsWith(".controller") || files[j].Name.EndsWith(".playable")|| files[j].Name.EndsWith(".asset")|| files[j].Name.EndsWith(".prefab") || files[j].Name.EndsWith(".xlsx")|| files[j].Name.EndsWith(".png") || files[j].Name.EndsWith(".jpg")) //������ͼƬ
                    {
                        if (files[j].Name.Contains(lstDir[i].Name))
                        {
                            Debug.LogError($"{files[j].Name} �Ѿ�����������");
                        }
                        else
                        {
                            string path = string.Format("{0}/{1}", lstPaths[i], files[j].Name);
                            string fileName = files[j].Name.Replace(".mp3", "").Replace(".wav", "").Replace(".ogg", "").Replace(".anim", "")
                                .Replace(".controller", "").Replace(".playable", "").Replace(".asset", "").Replace(".prefab", "").Replace(".xlsx", "").Replace(".jpg", "").Replace(".png", "").Replace(" ", "");
                            if (CheckString(fileName))
                            {
                                fileName = GetTranslationFromBaiduFanyi(appId, fileName, Language.zh, Language.en).Replace(" ", "");
                                Debug.LogError(fileName);
                            }
                            string newName = string.Format("{0}_{1}", lstDir[i].Name, fileName);

                            if (!CheckName(newName, lstName, () =>
                            {
                                index++;
                                newName = string.Format("{0}_{1}", lstDir[i].Name, fileName);
                            },
                            () => { lstName.Add(newName); }))
                            {
                                CheckName(newName, lstName, () =>
                                {
                                    index++;
                                    newName = string.Format("{0}_{1}", lstDir[i].Name, fileName);
                                }, () => { lstName.Add(newName); });
                            }

                            AssetDatabase.RenameAsset(path, newName);
                            Debug.Log($"{files[j].Name} Rename Success!");
                        }
                    }
                }
            }
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static bool CheckName(string name, List<string> lstName, Action yes, Action no)
    {
        if (lstName == null) { return false; }
        if (lstName.Contains(name))
        {
            if (yes != null)
            {
                yes();
            }
            return true;
        }
        else
        {
            if (no != null)
            {
                no();
            }
            return false;
        }
    }

    static string[] m_chaos = new string[] {
            ".jpg", ".png",  " ",
            "-", "*", "!", "@", "#", "$", "%", "^", "&", "(", ")", "+", "=", "|",
            ";", "<", ">", "?", "/", ":", "{", "}", "[", "]", "_","��","��","��","��"
        };

    static string RemoveChaosLetter(this string name)
    {
        for (int i = 0; i < m_chaos.Length; i++)
        {
            name = name.Replace(m_chaos[i], "");
        }
        return name;
    }
    public static void ShowProgress(float val, int total, int cur, string picname)
    {
        EditorUtility.DisplayProgressBar(string.Format("Rename {0} ing...", picname), string.Format("wait({0}/{1}) ", cur, total), val);
    }

    // �ж� ��ǰ�ַ��Ƿ�Ϊ����
    private static bool IsChinese(char c)
    {
        return c >= 0x4E00 && c <= 0x9FA5;
    }

    public static bool CheckString(string str)
    {
        char[] ch = str.ToCharArray();
        if (str != null)
        {
            for (int i = 0; i < ch.Length; i++)
            {
                if (IsChinese(ch[i]))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static string appId = "20191126000360543";
    private static string password = "TJv1Z17G5cUtfRUtQrJK";
    private static string GetTranslationFromBaiduFanyi(string id, string q, Language from, Language to, string language = "Ӣ��")
    {

        string jsonResult = String.Empty;
        //Դ����
        string languageFrom = from.ToString().ToLower();
        //Ŀ������
        string languageTo = to.ToString().ToLower();
        //�����
        string randomNum = System.DateTime.Now.Millisecond.ToString();
        //md5����
        string md5Sign = GetMD5WithString(appId + q + randomNum + password);
        //url
        string url = String.Format("http://api.fanyi.baidu.com/api/trans/vip/translate?q={0}&from={1}&to={2}&appid={3}&salt={4}&sign={5}",
            q,
            languageFrom,
            languageTo,
            appId,
            randomNum,
            md5Sign
            );
        WebClient wc = new WebClient();
        try
        {
            jsonResult = wc.DownloadString(url);
        }
        catch
        {
            jsonResult = string.Empty;
        }
        //���תjson
        TranslationResult temp = LitJson.JsonMapper.ToObject<TranslationResult>(jsonResult);
        string tar = "";
        if (null != temp)
        {
            if (string.IsNullOrEmpty(language))
            {
                for (int i = 0; i < temp.trans_result.Length; i++)
                {
                    string str = @"{""Key"":" + id + @",""Content"":" + @"""" + temp.trans_result[i].dst + @"""" + "},";
                }
            }
            else
            {
                for (int i = 0; i < temp.trans_result.Length; i++)
                {
                    string str = language + "|" + temp.trans_result[i].dst;
                    Debug.LogError($"Translation Results{str}");
                    tar = temp.trans_result[i].dst;
                }
            }
        }
        return tar;
    }

    private static string GetMD5WithString(string input)
    {
        if (input == null)
        {
            return null;
        }
        MD5 md5Hash = MD5.Create();
        //�������ַ���ת��Ϊ�ֽ����鲢�����ϣ����  
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        //����һ�� Stringbuilder ���ռ��ֽڲ������ַ���  
        StringBuilder sBuilder = new StringBuilder();
        //ѭ��������ϣ���ݵ�ÿһ���ֽڲ���ʽ��Ϊʮ�������ַ���  
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        //����ʮ�������ַ���  
        return sBuilder.ToString();
    }
}
public class Translation
{
    public string src { get; set; }
    public string dst { get; set; }
}

public enum Language
{
    //�ٶȷ���API�����ṩ�˶������ԣ�����ֻ���˼���
    zh,
    en,
    spa,
    fra,
    th,
    ara,
    ru,
    pt,
    de,
    el,
    vie,
    cht,
    yue,
}

public class TranslationResult
{
    //�����룬�������޷���������
    public string Error_code { get; set; }
    public string Error_msg { get; set; }
    public string from { get; set; }
    public string to { get; set; }
    public string Query { get; set; }
    //������ȷ�����صĽ��
    //�����������ԭ���ǰٶȷ���֧�ֶ�����ʻ����ı��ķ��룬�ڷ��͵��ֶ�q���û��з���\n���ָ�
    public Translation[] trans_result { get; set; }
}
