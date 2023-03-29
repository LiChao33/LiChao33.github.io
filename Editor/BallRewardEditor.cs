using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class BallRewardEditor : Editor
{
    [MenuItem("Tools/BallRewardConfig")]
    public static void Excute() {
        string filepath = "Excel/气球惊喜礼包.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        BallRewardConfig_FN config = Resources.Load<BallRewardConfig_FN>("Data/BallRewardConfig");
        config.data_cache = new List<BallRewardData_FN>();
        int row = datatable.Rows.Count;
        for (int i = 1; i < row; i++)
        {
            var temp = datatable.Rows[i];
            BallRewardData_FN data = new BallRewardData_FN();
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
            else if (flag.Contains("实物碎片"))
            {
                data.id = item_id.entity;
            }
            data.weight = int.Parse(temp[1].ToString());
            data.get_count = float.Parse(temp[2].ToString());
            config.data_cache.Add(data);
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}

