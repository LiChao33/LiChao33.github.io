using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class ModleRewardEditor : Editor
{
    [MenuItem("Tools/ModleRewardConfig")]
    public static void Excute() {
        string filepath = "Excel/关卡奖励.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        ModleRewardConfig_FN config = Resources.Load<ModleRewardConfig_FN>("Data/BG_ModleRewardConfig");
        config.data_cache = new List<ModleRewardData_FN>();
        int row = datatable.Rows.Count;
        for (int i = 1; i < row; i++)
        {
            var temp = datatable.Rows[i];
            ModleRewardData_FN data = new ModleRewardData_FN();
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
            else if (flag.Contains("钥匙"))
            {
                data.id = item_id.keys;
            }
            else if (flag.Contains("体力"))
            {
                data.id = item_id.power;
            }
            data.min_count = float.Parse(temp[1].ToString());
            data.max_count = float.Parse(temp[2].ToString());
            data.weight = int.Parse(temp[3].ToString());
            data.grid_pos = -1;
            config.data_cache.Add(data);
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}
