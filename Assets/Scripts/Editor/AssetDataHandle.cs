using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class AssetDataHandle
{
    [MenuItem("Assets/Make Asset Dirty")]
    static void BuildAllAssetBundles()
    {
#if UNITY_EDITOR
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            EditorUtility.SetDirty(obj);
        }
#endif

    }
    [MenuItem("Tools/ClearCache")]
    static void ClearCache()
    {
        PlayerPrefs.DeleteAll();
        Caching.ClearCache();
    }
}
