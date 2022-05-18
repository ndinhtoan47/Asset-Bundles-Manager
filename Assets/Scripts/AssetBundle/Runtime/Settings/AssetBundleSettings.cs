namespace GameFramework.AssetBundle.Runtime
{
    using UnityEngine;

    public enum Environment
    {
        Development = 0,
        Staging = 1,
        Production = 2,
    }

    [System.Serializable]
    public struct EnvironmentSettings
    {
        public uint version; // Change version to force re-download bundles
        public string contentHost;
        public Environment environment;
    }

    public class AssetBundleSettings : ScriptableObject
    {
        public Environment CurrentEnvironment = Environment.Development;
        public EnvironmentSettings[] EnvSettings = new EnvironmentSettings[1];
        public EnvironmentSettings GetCurrentEnvironment()
        {
            if (EnvSettings != null && EnvSettings.Length > 0)
            {
                for (int i = 0; i < EnvSettings.Length; i++)
                {
                    if (EnvSettings[i].environment == CurrentEnvironment)
                    {
                        return EnvSettings[i];
                    }
                }
            }
            return default;
        }
    }
}

