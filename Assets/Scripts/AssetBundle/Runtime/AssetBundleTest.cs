namespace GameFramework.AssetBundle.Runtime
{
    
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(AssetBundleManager))]
    public class AssetBundleTest : MonoBehaviour
    {
        public string bundleName;
        public string assetName;

        private AssetBundleManager assetBundleManager;
        private void Awake() 
        {
            assetBundleManager = GetComponent<AssetBundleManager>();
        }  


        [ContextMenu("Instantiate Sample Object")]
        public void InstantiateSampleObject()
        {
            assetBundleManager.InstantiateAsset<GameObject>(bundleName, assetName, null, 0);
        }

        [ContextMenu("Load scene")]
        public void LoadScene()
        {
            assetBundleManager.LoadScene("scene", @"Assets/Data/Bi-a/Scenes/Demo_full.unity");
        }

        [ContextMenu("Load scene async")]
        public void LoadSceneAsync()
        {
            SceneAsyncLoadingCallback callback = new SceneAsyncLoadingCallback();
            callback.OnComplete = (op) => Debug.Log("OnComplete callback is called");
            callback.OnAsyncOpResult = (op) => Debug.Log("OnAsyncOpResult callback is called");

            assetBundleManager.LoadSceneAsync("scene", @"Assets/Data/Bi-a/Scenes/Demo_full.unity", 0, UnityEngine.SceneManagement.LoadSceneMode.Single, null, callback);
        }
        

        [ContextMenu("Load All Asset Bunbles")]
        public void LoadAllAssetBunbles()
        {
            assetBundleManager.LoadAllAssetBundles((successCount, failCount) =>
            {
                Debug.Log($"Load all asset bundles complete {successCount}/{successCount + failCount}");
            });
        }
    }
}