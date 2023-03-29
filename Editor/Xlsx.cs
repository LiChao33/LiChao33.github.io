using Excel;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class Xlsx
{
    private static Xlsx instance = null;

    public static Xlsx get() {
        if (instance == null) {
            instance = new Xlsx();
        }
        return instance;
    }

    public DataTable read_xlsx(string filename) {
        string fullname = Utils.Get_File_Path() + "/" + filename;
        if (!File.Exists(fullname)) {
            Debug.LogError(fullname + "文件不存在");
            return null;
        }
        FileStream fs = new FileStream(fullname, FileMode.Open, FileAccess.Read);
        IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
        DataSet dataSet = reader.AsDataSet();
        reader.Dispose();
        return dataSet.Tables[0];
    }
}
