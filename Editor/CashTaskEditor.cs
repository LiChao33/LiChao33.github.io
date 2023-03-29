using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;


public class CashTaskEditor : Editor
{
    [MenuItem("Tools/CashTaskConfig")]
    public static void Excute() {
        string filepath = "Excel/现金任务.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        CashTaskConfig_FN config = Resources.Load<CashTaskConfig_FN>("Data/CashTaskConfig");
        config.data_cache = new List<CashTaskData_FN>();
        int row = datatable.Rows.Count;
        for (int i = 1; i < row; i++)
        {
            var temp = datatable.Rows[i];
            CashTaskData_FN data = new CashTaskData_FN();
            data.taskid = i - 1;
            data.task_type = temp[0].ToString();
            if (data.task_type.Contains("first")) {
                data.condition = 0;
            } else if (data.task_type.Contains("money")) {
                data.condition = int.Parse(data.task_type.Substring("money".Length));
            }
            else if (data.task_type.Contains("gold"))
            {
                data.condition = int.Parse(data.task_type.Substring("gold".Length));
            }
            else if (data.task_type.Contains("AMZN"))
            {
                if (data.task_type.Contains("解锁"))
                {
                    data.condition = 0;
                }
                else {
                    data.condition = int.Parse(data.task_type.Substring("AMZN".Length));
                }
            }

            data.cash_count = int.Parse(temp[1].ToString());
            data.task_list = new List<CashTaskStepData_FN>();
            for (int j = 2; j < 10; j += 2)
            {
                CashTaskStepData_FN cdata = new CashTaskStepData_FN();
                cdata.task_count = int.Parse(temp[j].ToString());
                cdata.task_time = int.Parse(temp[j + 1].ToString()) * 3600;
                data.task_list.Add(cdata);
            }
            data.stone_id = (item_id)(int.Parse(temp[10].ToString()));
            config.data_cache.Add(data);
        }

        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}
