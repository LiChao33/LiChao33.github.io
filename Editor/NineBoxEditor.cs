using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class NineBoxEditor : Editor
{
    [MenuItem("Tools/NineBoxConfig")]
    public static void Excute() {
        string filepath = "Excel/nineboxdata.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        NineBoxConfig_FN config = Resources.Load<NineBoxConfig_FN>("Data/NineBoxConfig");
        config.data_cache = new List<NineBoxData_FN>();
        int row = datatable.Rows.Count;
        for (int i = 1; i < row; i++)
        {
            var temp = datatable.Rows[i];
            NineBoxData_FN data = new NineBoxData_FN();
            string flag = temp[0].ToString();
            if (flag.Contains("金币"))
            {
                data.id = item_id.gold;
            }
            else if (flag.Contains("现金"))
            {
                data.id = item_id.cash;
            }
            else if (flag.Contains("亚马逊卡"))
            {
                data.id = item_id.card_amazon;
            }
            data.weight = int.Parse(temp[1].ToString());
            data.get_count = float.Parse(temp[2].ToString());
            config.data_cache.Add(data);
        }
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();

    }
}
