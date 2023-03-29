using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class PowerEditor : Editor
{
    [MenuItem("Tools/PowerConfig")]
    public static void Excute() {
        string filepath = "Excel/商城.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        PowerConfig_FN config = Resources.Load<PowerConfig_FN>("Data/PowerConfig");
        config.data_cache = new List<PowerData_FN>();
        int row = datatable.Rows.Count;
        for (int i = 1; i < row; i++)
        {
            var temp = datatable.Rows[i];
            PowerData_FN data = new PowerData_FN();
            string flag = temp[0].ToString();
            if (flag.Contains("体力"))
            {
                data.id = item_id.energy;
            }
            //else if (flag.Contains("现金"))
            //{
            //    data.id = item_id.id_cash;
            //}
            //else if (flag.Contains("亚马逊卡"))
            //{
            //    data.id = item_id.id_amazon;
            //}
            //else if (flag.Contains("实物碎片"))
            //{
            //    data.id = item_id.id_entity;
            //}
            data.initcount = int.Parse(temp[1].ToString());
            data.addcount = int.Parse(temp[2].ToString());
            data.updatetime = int.Parse(temp[3].ToString());
            data.limitcount = int.Parse(temp[4].ToString());
            data.subcount = int.Parse(temp[5].ToString());
            config.data_cache.Add(data);
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}
