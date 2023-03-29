using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class RewardItemEditor : Editor
{
    [MenuItem("Tools/EntityConfig")]
    public static void Excute() {
        string filepath = "Excel/奖励碎片个数权重.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        EntityConfig_FN config = Resources.Load<EntityConfig_FN>("Data/EntityConfig");
        config.data_cache = new List<EntityData_FN>();
        int row = datatable.Rows.Count;
        for (int i = 1; i < row; i++)
        {
            var temp = datatable.Rows[i];
            EntityData_FN data = new EntityData_FN();
            data.id = item_id.sonytv + (i - 1);
            data.item_name = temp[0].ToString();
            data.max_count = int.Parse(temp[1].ToString());
            data.weight = int.Parse(temp[2].ToString());
            data.limit_count = int.Parse(temp[3].ToString());
            data.get_count = int.Parse(temp[4].ToString());
            data.code = temp[5].ToString();
            config.data_cache.Add(data);
        }
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}
