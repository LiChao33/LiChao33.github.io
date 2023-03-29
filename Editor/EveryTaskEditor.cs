using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class EveryTaskEditor : Editor
{
    [MenuItem("Tools/EveryTaskConfig")]
    public static void Excute()
    {
        string filepath = "Excel/每日任务.xlsx";
        DataTable datatable = Xlsx.get().read_xlsx(filepath);
        if (datatable == null)
        {
            Debug.LogError("读取 " + filepath + " 失败");
            return;
        }
        EveryTaskConfig_FN config = Resources.Load<EveryTaskConfig_FN>("Data/EveryTaskConfig");
        config.data_cache = new List<EveryTaskData_FN>();
        int row = datatable.Rows.Count;
        for (int i = 0; i < 7; i++) {
            EveryTaskData_FN taskdata = new EveryTaskData_FN();
            taskdata.task_cache = new List<EveryTaskStepData_FN>();
            for (int j = 1;j<=5;j++) {
                var temp = datatable.Rows[i*5+j];
                EveryTaskStepData_FN stepdata = new EveryTaskStepData_FN();
                stepdata.tasktype = temp[0].ToString();
                stepdata.taskdescrib = temp[1].ToString();
                stepdata.condition = int.Parse(temp[2].ToString());
                stepdata.reward = float.Parse(temp[3].ToString());
                string flag = temp[4].ToString();
                if (flag.Contains("cash"))
                {
                    stepdata.rewardtype = item_id.cash;
                }
                else if (flag.Contains("gold"))
                {
                    stepdata.rewardtype = item_id.gold;
                }
                else if (flag.Contains("amzn"))
                {
                    stepdata.rewardtype = item_id.card_amazon;
                }
                taskdata.task_cache.Add(stepdata);
            }
            config.data_cache.Add(taskdata);
        }
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();
    }
}
