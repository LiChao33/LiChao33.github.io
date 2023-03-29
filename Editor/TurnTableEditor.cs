using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class TurnTableEditor : Editor
{
    [MenuItem("Tools/TurnTableConfig")]
    public static void Excute() {
        string filepath = "Excel/AssetDataDailExcel.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        TurnTableConfig_FN config = Resources.Load<TurnTableConfig_FN>("Data/TurnTableConfig");

        config.commo_data_cache = new List<TurnTableData_FN>();
        config.stone_data_cache = new List<TurnTableData_FN>();
        config.data_cache = new List<TurnTableData_FN>();
        int row = datatable.Rows.Count;
        for (int i = 1; i < row; i++)
        {
            var temp = datatable.Rows[i];
            TurnTableData_FN data = new TurnTableData_FN();
            string flag = temp[0].ToString();
            if (flag.Contains("starYellow"))
            {
                data.id = item_id.yellow;
            }
            else if (flag.Contains("starBlue"))
            {
                data.id = item_id.blue;
            }
            else if (flag.Contains("starRed"))
            {
                data.id = item_id.red;
            }
            else if (flag.Contains("gold"))
            {
                data.id = item_id.gold;
            }
            else if (flag.Contains("cash"))
            {
                data.id = item_id.cash;
            }
            else if (flag.Contains("amzn"))
            {
                data.id = item_id.card_amazon;
            }
            data.get_count = float.Parse(temp[2].ToString());
            data.weight = int.Parse(temp[3].ToString());
            if (data.id >= item_id.yellow)
            {
                config.stone_data_cache.Add(data);
            }
            else
            {
                config.commo_data_cache.Add(data);
            }
            config.data_cache.Add(data);
        }
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}
