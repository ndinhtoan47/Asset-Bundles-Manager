namespace GameFramework.AssetBundle.Runtime
{
    using UnityEngine;

    public static class AssetBundleHelper
    {
        private static AssetBundleSettings _assetBundleSettings;

        public static AssetBundleSettings GetAssetBundleSettings()
        {
            string relativePath = System.IO.Path.Combine(
                AssetBundleRuntimePath.ASSET_BUNDLE_SETTINGS_DIRECTORY,
                AssetBundleRuntimePath.ASSET_BUNDLE_SETTINGS_ASSET_NAME).Replace(".asset", string.Empty);

            AssetBundleSettings settings = Resources.Load(relativePath) as AssetBundleSettings;

            if (settings == null)
            {
                throw new System.Exception("Settings not found, please choose 'Tools/AssetBundle/Task/Select Settings' to initalize the one");
            }
            return settings;
        }

        public static AssetBundleBuildExport GetAssetBundleBuildExport()
        {
            string relativePath = System.IO.Path.Combine(
                AssetBundleRuntimePath.ASSET_BUNDLE_SETTINGS_DIRECTORY,
                AssetBundleRuntimePath.ASSET_BUNDLE_BUILD_EXPORT_ASSET_NAME).Replace(".asset", string.Empty);

            AssetBundleBuildExport buildExport = Resources.Load(relativePath) as AssetBundleBuildExport;

            if (buildExport == null)
            {
                throw new System.Exception("Asset bundle has not been built yet.");
            }
            return buildExport;
        }

        public static string GetPlatform()
        {
#if UNITY_STANDALONE_WIN
            return "StandaloneWindows64";
#elif UNITY_WEBGL
            return "WebGL";
#elif UNITY_STANDALONE_OSX
            return "StandaloneOSX";
#else
            return "Unknow";
#endif
        }
    }

}
