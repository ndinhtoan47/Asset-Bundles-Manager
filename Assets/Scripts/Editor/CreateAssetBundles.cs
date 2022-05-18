using UnityEditor;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

public class CreateAssetBundles
{
    static Dictionary<string, string> versionControlData;

    static string assetBundleDirectory = @"/Bin/{0}/version/{1}/";

    [MenuItem("Assets/Build All AssetBundles In Game")]
    static void BuildAllAssetBundles()
    {
        string platform = EditorUserBuildSettings.activeBuildTarget.ToString();
        if (!Directory.Exists(Directory.GetCurrentDirectory() + string.Format(assetBundleDirectory, platform, Application.version)))
        {
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + string.Format(assetBundleDirectory, platform, Application.version));
        }
        BuildPipeline.BuildAssetBundles(Directory.GetCurrentDirectory() + string.Format(assetBundleDirectory, platform, Application.version), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        ClearGarbageFiles(Directory.GetCurrentDirectory() + string.Format(assetBundleDirectory, platform, Application.version));
    }

    [MenuItem("Assets/Build All AssetBundle In This Path")]
    static void BuidlAllAssetbundlesInThisPath()
    {
        var path = "";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path))
            {
                break;
            }
            break;
        }
        if (path.Length > 0)
        {
            BuildInPath(path, string.Empty, true);
        }
    }

    [MenuItem("Assets/Build All AssetBundle In This Folder")]
    static void BuidlAllAssetbundlesInThisFolder()
    {
        string path = "Assets";
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            break;
        }
        if (path.Length > 0)
        {
            BuildInPath(path, string.Empty, false);
        }

    }

    // [MenuItem("Assets/Caculate Md5 All File In this Path")]
    // static void CaculateMd5AllFileInthisPath()
    // {
    //     var version = new Dictionary<string, string>();
    //     var buildPath = Directory.GetCurrentDirectory() + string.Format(assetBundleDirectory, Utility.GetPlatFormName(), Application.version) + Path.AltDirectorySeparatorChar;
    //     var path = buildPath + "assets/";
    //     if (!Directory.Exists(path))
    //     {
    //         Directory.CreateDirectory(path);
    //     }

    //     var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
    //     for (int i = 0; i < files.Length; i++)
    //     {
    //         var name = (files[i].Replace(buildPath, string.Empty).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
    //         version.Add(name, Utility.CalculateMD5(files[i]));
    //     }
    //     var filePath = Directory.GetCurrentDirectory() + string.Format(assetBundleDirectory, Utility.GetPlatFormName(), Application.version) + "/remoteVersion.json";
    //     if (!File.Exists(filePath))
    //     {
    //         File.Create(filePath).Close();
    //     }
    //     File.WriteAllText(filePath, JsonConvert.SerializeObject(version));
    // }


    [MenuItem("Assets/Build Assetbundle Only this Selection")]
    static void BuidAssetBundleOnlySelection()
    {
        LoadVersion();
        var path = "";
        var obj = Selection.activeObject;
        string platform = EditorUserBuildSettings.activeBuildTarget.ToString();

        path = AssetDatabase.GetAssetPath(obj.GetInstanceID());
        if (path.Length > 0)
        {
            if (File.Exists(path) && Path.GetExtension(path) == ".prefab")
            {
                string buildPath = Directory.GetCurrentDirectory() + string.Format(assetBundleDirectory, platform, Application.version) + path.Replace(Path.GetFileName(path), string.Empty);
                buildPath = buildPath.Replace(" ", string.Empty).ToLower();
                string filePath = buildPath + Path.GetFileNameWithoutExtension(path) + ".bin";
                filePath = filePath.Replace(" ", string.Empty).ToLower();
                if (!Directory.Exists(buildPath))
                {
                    Directory.CreateDirectory(buildPath);
                }
                AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[1];
                assetBundleBuilds[0] = new AssetBundleBuild();
                assetBundleBuilds[0].assetBundleName = Path.GetFileNameWithoutExtension(path) + ".bin";
                assetBundleBuilds[0].assetNames = new string[1] { path };

                BuildPipeline.BuildAssetBundles(buildPath, assetBundleBuilds, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
                ClearGarbageFiles(buildPath);
            }
        }

        UpdateVersion();
    }

    static void UpdateVersion()
    {
        
        var path = Directory.GetCurrentDirectory() + string.Format(assetBundleDirectory, "platform", Application.version) + "version.json";
        if (!File.Exists(path))
        {
            File.Create(path);
        }
        File.WriteAllText(path, JsonConvert.SerializeObject(versionControlData));
        // CaculateMd5AllFileInthisPath();
    }


    static bool CompareMd5(string filePath)
    {
        filePath = filePath.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        if (versionControlData == null)
        {
            LoadVersion();
        }
        if (versionControlData.ContainsKey(filePath))
        {
            var md5 = "0";// Utility.CalculateMD5(filePath);
            if (versionControlData[filePath] == md5)
            {
                return true;
            }
            else
            {
                versionControlData[filePath] = md5;
            }
        }
        else
        {
            var md5 = "0"; //Utility.CalculateMD5(filePath);
            versionControlData.Add(filePath, md5);
        }
        return false;
    }

    static void ClearGarbageFiles(string path)
    {
        string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (Path.GetExtension(files[i]) != ".bin")
            {
                File.Delete(files[i]);

            }
        }
    }

    static void LoadVersion()
    {
        if (versionControlData == null)
        {
            var path = Directory.GetCurrentDirectory() + string.Format(assetBundleDirectory, "platform", Application.version) + "version.json";
            if (File.Exists(path))
            {
                versionControlData = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
                if (versionControlData == null)
                {
                    versionControlData = new Dictionary<string, string>();
                }
            }
            else
            {
                versionControlData = new Dictionary<string, string>();

                File.WriteAllText(path, JsonConvert.SerializeObject(versionControlData));
            }
        }
    }

    static void BuildInPath(string path, string folderName, bool recursive = true)
    {
        LoadVersion();
        bool canBuild = false;
        if (Directory.Exists(path))
        {
            string[] prefabPath = Directory.GetFiles(path, "*.prefab", SearchOption.TopDirectoryOnly);
            AssetBundleBuild[] assetBundleBuilds = new AssetBundleBuild[prefabPath.Length];
            string buildPath = Directory.GetCurrentDirectory() + string.Format(assetBundleDirectory, "platform", Application.version) + path;
            buildPath = buildPath.Replace(" ", string.Empty).ToLower();
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
            }

            for (int i = 0; i < prefabPath.Length; i++)
            {

                string filePath = buildPath + Path.AltDirectorySeparatorChar + Path.GetFileNameWithoutExtension(prefabPath[i]) + ".bin";

                if (File.Exists(filePath) && CompareMd5(prefabPath[i]))
                {
                    continue;
                }
                canBuild = true;
                assetBundleBuilds[i] = new AssetBundleBuild();
                assetBundleBuilds[i].assetBundleName = Path.GetFileNameWithoutExtension(prefabPath[i]) + ".bin";
                assetBundleBuilds[i].assetNames = new string[1] { prefabPath[i] };
            }
            if (canBuild)
            {
                BuildPipeline.BuildAssetBundles(buildPath, assetBundleBuilds, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
                ClearGarbageFiles(buildPath);
            }
            if (recursive)
            {
                string[] childs = Directory.GetDirectories(path);
                if (childs != null && childs.Length > 0)
                {
                    for (int i = 0; i < childs.Length; i++)
                    {
                        BuildInPath(childs[i], string.Format(@"{0}\{1}", folderName, new DirectoryInfo(childs[i]).Name));
                    }
                }
            }
        }
        if (canBuild)
            UpdateVersion();
    }
}
