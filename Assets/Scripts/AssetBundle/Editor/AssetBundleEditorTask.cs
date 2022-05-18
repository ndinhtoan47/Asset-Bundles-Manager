namespace GameFramework.AssetBundle.CustomEditor
{
#if UNITY_EDITOR    

    using System.IO;
    using GameFramework.AssetBundle.Runtime;
    using UnityEditor;
    using UnityEngine;

    public class AssetBundleEditorTask
    {

        [MenuItem(AssetBundleEditorPath.EDITOR_MENU + "Task/Build Asset Bundles/Current")]
        public static void BuildAllAssetBundlesCurrentPlatform()
        {
            BuildAllAssetBundles(EditorUserBuildSettings.activeBuildTarget);
        }

        [MenuItem(AssetBundleEditorPath.EDITOR_MENU + "Task/Build Asset Bundles/Standalone Windows")]
        public static void BuildAllAssetBundleWindows64()
        {
            BuildAllAssetBundles(BuildTarget.StandaloneWindows64);
        }

        [MenuItem(AssetBundleEditorPath.EDITOR_MENU + "Task/Build Asset Bundles/WebGL")]
        public static void BuildAllAssetBundleWebGL()
        {
            BuildAllAssetBundles(BuildTarget.WebGL);
        }

        [MenuItem(AssetBundleEditorPath.EDITOR_MENU + "Task/Build Asset Bundles/Standalone OSX")]
        public static void BuildAllAssetBundleOSX()
        {
            BuildAllAssetBundles(BuildTarget.StandaloneOSX);
        }

        public static void BuildAllAssetBundles(BuildTarget curTarget)
        {
            if (EditorApplication.isUpdating)
            {
                Debug.LogError("[BuildAllAssetBundles] EditorApplication is updating, please try again after a second");
                return;
            }
            string contentBuildDir = AssetBundleEditorPath.GetPlatformContentBuildDirectory(curTarget);

            if (!Directory.Exists(contentBuildDir))
            {
                Directory.CreateDirectory(contentBuildDir);
            }

            AssetBundleManifest manifest =  BuildPipeline.BuildAssetBundles(contentBuildDir,
                                            BuildAssetBundleOptions.None,
                                            curTarget);

            if (manifest != null)
            {
                string[] allAssetBundles = manifest.GetAllAssetBundles();
                string bundleNames = string.Join("\n", allAssetBundles);

                AssetBundleBuildExport buildExport = SelectAssetBundleBuildExport();
                if (buildExport != null)
                {
                    buildExport.AssetBundles = allAssetBundles;
                    EditorUtility.SetDirty(buildExport);
                }
                Debug.Log("[BuildAllAssetBundles]" + curTarget.ToString());
                Debug.Log("[BuildAllAssetBundles]\n" + bundleNames);            
            }
            else
            {
                Debug.LogError("[BuildAllAssetBundles] Build failed");
            }
        }

        [MenuItem(AssetBundleEditorPath.EDITOR_MENU + "Task/Clean Previous Build")]
        public static void CleanPreviousBuild()
        {
            BuildTarget curTarget = AssetBundleEditorPath.GetCurrentPlatform();
            string contentBuildDir = AssetBundleEditorPath.GetCurrentPlatformContentBuildDirectory();
            if (Directory.Exists(contentBuildDir))
            {
                Directory.Delete(contentBuildDir, true);
            }
            Debug.Log("Clean Previous Build");

            Caching.ClearCache();
        }

        [MenuItem(AssetBundleEditorPath.EDITOR_MENU + "Task/Clear Asset Bundles Cached")]
        public static void ClearAssetBundleCached()
        {
            bool isSuccess = Caching.ClearCache();
            string msg = "Clear Asset Bundles Cached: " + isSuccess;
            if (isSuccess)
            {
                Debug.Log(msg);
            }
            else
            {
                Debug.LogError(msg);
            }
        }

        [MenuItem(AssetBundleEditorPath.EDITOR_MENU + "Task/Select Settings")]
        public static void SelectSettings()
        {
#if UNITY_EDITOR
            string settingsDir = AssetBundleRuntimePath.GetAssetBundleSettingsRelativeDirectory();
            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }

            string settingsFilePath = System.IO.Path.Combine(settingsDir, AssetBundleRuntimePath.ASSET_BUNDLE_SETTINGS_ASSET_NAME);
            if (!File.Exists(settingsFilePath))
            {
                AssetBundleSettings settings = ScriptableObject.CreateInstance<AssetBundleSettings>();
                AssetDatabase.CreateAsset(settings, settingsFilePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = settings;
            }
            else
            {
                AssetBundleSettings settings = AssetDatabase.LoadAssetAtPath<AssetBundleSettings>(settingsFilePath);
                Selection.activeObject = settings;
            }
#endif
        }

        [MenuItem(AssetBundleEditorPath.EDITOR_MENU + "Task/Select Asset Bundle Build Export")]
        public static AssetBundleBuildExport SelectAssetBundleBuildExport()
        {
#if UNITY_EDITOR
            string settingsDir = AssetBundleRuntimePath.GetAssetBundleSettingsRelativeDirectory();
            if (!Directory.Exists(settingsDir))
            {
                Directory.CreateDirectory(settingsDir);
            }

            string buildExportfilePath = System.IO.Path.Combine(settingsDir, AssetBundleRuntimePath.ASSET_BUNDLE_BUILD_EXPORT_ASSET_NAME);
            if (!File.Exists(buildExportfilePath))
            {
                AssetBundleBuildExport buildExport = ScriptableObject.CreateInstance<AssetBundleBuildExport>();
                AssetDatabase.CreateAsset(buildExport, buildExportfilePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = buildExport;
                return buildExport;
            }
            else
            {
                AssetBundleBuildExport buildExport = AssetDatabase.LoadAssetAtPath<AssetBundleBuildExport>(buildExportfilePath);
                return buildExport;
            }
#endif
        }

        [MenuItem(AssetBundleEditorPath.EDITOR_MENU + "Task/Log Paths")]
        public static void LogPath()
        {
            Debug.Log("------ [LogPaths] ------");
            BuildTarget curTarget = AssetBundleEditorPath.GetCurrentPlatform();

            Debug.Log("[GetProjectDirectory] "              + AssetBundleEditorPath.GetProjectDirectory());
            Debug.Log("[GetContentBuildDirectory] "         + AssetBundleEditorPath.GetContentBuildDirectory());
            Debug.Log("[GetCurrentPlatformContentBuildDirectory] " + AssetBundleEditorPath.GetCurrentPlatformContentBuildDirectory());

            Debug.Log("------ [End] ------");
        }
    }
#endif
}