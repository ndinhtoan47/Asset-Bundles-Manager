namespace GameFramework.AssetBundle.CustomEditor
{    
#if UNITY_EDITOR
    using UnityEditor;
    using GameFramework.AssetBundle.Runtime;
#endif

    public static class AssetBundleEditorPath
    {
#if UNITY_EDITOR
        public const string EDITOR_MENU = "Tools/AssetBundle/";
        public static string GetProjectDirectory()
        {
            return System.IO.Path.Combine(UnityEngine.Application.dataPath, "../");
        }

        public static string GetContentBuildDirectory()
        {
            return System.IO.Path.Combine(GetProjectDirectory(), "AssetBundles");
        }

        public static BuildTarget GetCurrentPlatform()
        {
            return EditorUserBuildSettings.activeBuildTarget;
        }

        public static string GetPlatformContentBuildDirectory(BuildTarget target)
        {
            return System.IO.Path.Combine(GetContentBuildDirectory(), target.ToString());
        }

        public static string GetCurrentPlatformContentBuildDirectory()
        {
            return System.IO.Path.Combine(GetContentBuildDirectory(), AssetBundleHelper.GetPlatform());
        }
#endif
    }
}