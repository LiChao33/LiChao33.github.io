using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class ShopTableEditor : Editor
{
    [MenuItem("Tools/ShopTableConfig")]
    public static void Excute() {
        string filepath = "Excel/商城转盘.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        ShopTableConfig_FN config = Resources.Load<ShopTableConfig_FN>("Data/ShopTableConfig");
        config.data_cache = new List<ShopTableData_FN>();
        int row = datatable.Rows.Count;
        for (int i = 1; i < row; i++)
        {
            var temp = datatable.Rows[i];
            ShopTableData_FN data = new ShopTableData_FN();
            string flag = temp[1].ToString();
            if (flag.Contains("金币"))
            {
                data.id = item_id.gold;
            }
            else if (flag.Contains("现金"))
            {
                data.id = item_id.cash;
            }
            else if (flag.Contains("实物碎片"))
            {
                data.id = item_id.entity;
            }
            data.get_count = float.Parse(temp[2].ToString());
            data.weight = int.Parse(temp[3].ToString());
            config.data_cache.Add(data);
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}
