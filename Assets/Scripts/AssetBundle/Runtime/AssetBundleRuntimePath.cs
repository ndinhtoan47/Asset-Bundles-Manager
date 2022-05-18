namespace GameFramework.AssetBundle.Runtime
{
    public static class AssetBundleRuntimePath
    {
        public const string ASSET_BUNDLE_SETTINGS_DIRECTORY = "AssetsBundleSettings";
        public const string ASSET_BUNDLE_SETTINGS_ASSET_NAME = "AssetBundleSettings.asset";
        public const string ASSET_BUNDLE_BUILD_EXPORT_ASSET_NAME = "AssetBundleBuildExport.asset";

        public static string GetAssetBundleSettingsRelativeDirectory()
        {
            return System.IO.Path.Combine("Assets", "Resources", ASSET_BUNDLE_SETTINGS_DIRECTORY);
        }
    }
}
