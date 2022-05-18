namespace GameFramework.AssetBundle.Runtime
{
    public struct SceneAsyncLoadingCallback
    {
        public System.Action<UnityEngine.AsyncOperation> OnComplete;
        public System.Action<UnityEngine.AsyncOperation> OnAsyncOpResult;
    }

    public interface IAssetBundleSceneLoader
    {
        void LoadScene(string sceneName);
        void LoadSceneAsync(string sceneName, SceneAsyncLoadingCallback callback);
    }
}