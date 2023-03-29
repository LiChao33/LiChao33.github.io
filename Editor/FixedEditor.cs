using Fixed;
using UnityEditor;

public class FixedEditor : Editor
{
    [MenuItem("Assets/打乱文件夹/打乱并重命名")]
    public static void FixedRename()
    {
        FileFixed.Fixed(true);
    }

    [MenuItem("Assets/打乱文件夹/只是打乱")]
    public static void Fixed()
    {
        FileFixed.Fixed(false);
    }

    [MenuItem("Assets/RenameDirectory(选中文件夹即可重命名文件夹)")]
    static void RenameDll()
    {
        Rename.RenameOne();
    }

    [MenuItem("Tools/重新生成GUID")]
    public static void RegenerateGuids()
    {
        ResetGUID.RegenerateGuids();
    }
}
