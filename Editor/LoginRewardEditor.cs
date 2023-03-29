using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class LoginRewardEditor : Editor
{
    [MenuItem("Tools/LoginRewardConfig")]
    public static void Excute() {
        string filepath = "Excel/登录宝箱.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        LoginRewardConfig_FN config = Resources.Load<LoginRewardConfig_FN>("Data/LoginRewardConfig");
        config.data_cache = new List<LoginRewardData_FN>();
        int row = datatable.Rows.Count;
        for (int i = 2; i < row; i++)
        {
            var temp = datatable.Rows[i];
            LoginRewardData_FN data = new LoginRewardData_FN();
            string flag = temp[0].ToString();
            if (flag == "") continue;
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
            data.min_count = float.Parse(temp[2].ToString());
            data.max_count = float.Parse(temp[3].ToString());
            config.data_cache.Add(data);
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}
