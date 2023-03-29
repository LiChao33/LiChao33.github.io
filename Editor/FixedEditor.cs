using Fixed;
using UnityEditor;

public class FixedEditor : Editor
{
    [MenuItem("Assets/�����ļ���/���Ҳ�������")]
    public static void FixedRename()
    {
        FileFixed.Fixed(true);
    }

    [MenuItem("Assets/�����ļ���/ֻ�Ǵ���")]
    public static void Fixed()
    {
        FileFixed.Fixed(false);
    }

    [MenuItem("Assets/RenameDirectory(ѡ���ļ��м����������ļ���)")]
    static void RenameDll()
    {
        Rename.RenameOne();
    }

    [MenuItem("Tools/��������GUID")]
    public static void RegenerateGuids()
    {
        ResetGUID.RegenerateGuids();
    }
}
