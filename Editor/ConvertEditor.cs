using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class ConvertEditor : Editor
{
    [MenuItem("Tools/ConvertConfig")]
    public static void Excute() { 
        string filepath = "Excel/物品兑换规则.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        ConvertConfig_FN config = Resources.Load<ConvertConfig_FN>("Data/ConvertConfig");
        config.data_cache = new List<ConvertData_FN>();
        int row = datatable.Rows.Count;
        for (int i = 1; i < row; i++)
        {
            var temp = datatable.Rows[i];
            ConvertData_FN data = new ConvertData_FN();
            string flag = temp[0].ToString();
            if (flag.Contains("美金"))
            {
                data.id = item_id.cash;
            }
            else if (flag.Contains("亚马逊卡"))
            {
                data.id = item_id.card_amazon;
            }
            else
            {
                data.id = item_id.sonytv + (i - 1);
            }
            data.limit_count = int.Parse(temp[2].ToString());
            config.data_cache.Add(data);
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}
