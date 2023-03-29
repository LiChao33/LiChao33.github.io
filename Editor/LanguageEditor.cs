using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class LanguageEditor : Editor
{
    [MenuItem("Tools/LanguageConfig")]
    public static void Excute() {
        string filepath = "Excel/LanguageExcelBase.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        LanguageConfig config = Resources.Load<LanguageConfig>("Data/LanguageConfig");
        config.data_cache = new List<LanguageData>();
        int row = datatable.Rows.Count;
        for (int i = 1; i < row; i++)
        {
            var temp = datatable.Rows[i];
            LanguageData data = new LanguageData();
            data.code = temp[0].ToString();
            data.lang_cache = new List<string>();
            for (Country_Type id = Country_Type.中文; id <= Country_Type.法文; id++)
            {
                data.lang_cache.Add(temp[(int)id + 1].ToString());
            }
            config.data_cache.Add(data);
        }
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}