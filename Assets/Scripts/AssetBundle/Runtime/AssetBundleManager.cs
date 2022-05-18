namespace GameFramework.AssetBundle.Runtime
{
    using UnityEngine;
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine.SceneManagement;
    
    /// <summary>
    /// Class is using for asset bundle manager, it can be Singleton, MonoBehaviour event doesn't inherite by any class
    /// </summary> 
    public class AssetBundleManager : MonoBehaviour
    {   
        public delegate void LoadAllAssetBundlesCallback(int successCount, int failCount);

        private Dictionary<string, AssetBundle> _loadedBundles = new Dictionary<string, AssetBundle>();

        private void Internal_OnAssetBundleDownloaded(string bundleName, AssetBundle bundle, string error)
        {
            if (string.IsNullOrEmpty(error) && bundle != null)
            {
                if (_loadedBundles.ContainsKey(bundleName))
                {
                    _loadedBundles[bundleName] = bundle;
                }
                else
                {
                    _loadedBundles.Add(bundleName, bundle);
                }
            }            
        }

        private void Internal_LoadScene(AssetBundle bundle, string sceneName, LoadSceneMode loadMode, IAssetBundleSceneLoader sceneLoader)
        {
            if (bundle == null || string.IsNullOrEmpty(sceneName)) { return; }

            string[] scenes = bundle.GetAllScenePaths();
            bool isSceneExisted = scenes != null && scenes.Any((s) => s == sceneName);
            if (isSceneExisted)
            {
                if (sceneLoader == null)
                {
                    SceneManager.LoadScene(sceneName, loadMode);
                }
                else
                {
                    sceneLoader.LoadScene(sceneName);
                }
            }
        }

        private void Internal_LoadSceneAsync(
            AssetBundle bundle, 
            string sceneName, 
            LoadSceneMode loadMode, 
            IAssetBundleSceneLoader sceneLoader,
            SceneAsyncLoadingCallback sceneLoadingCallback)
        {
            if (bundle == null || string.IsNullOrEmpty(sceneName)) { return; }

            string[] scenes = bundle.GetAllScenePaths();
            bool isSceneExisted = scenes != null && scenes.Any((s) => s == sceneName);
            if (isSceneExisted)
            {
                if (sceneLoader == null)
                {
                    AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, loadMode);
                    op.allowSceneActivation = true;

                    sceneLoadingCallback.OnAsyncOpResult?.Invoke(op);
                    op.completed += sceneLoadingCallback.OnComplete;
                }
                else
                {
                    sceneLoader.LoadSceneAsync(sceneName, sceneLoadingCallback);
                }
            }
        }

        /// <summary>
        /// Load a specified bundle with its name
        /// </summary>         
        public AssetBundleDownloadRequest LoadAssetBundle(string bundleName, uint crc, AssetBundleDownloadCallback callback)
        {
            if (_loadedBundles.ContainsKey(bundleName))
            {
                callback?.Invoke(null, _loadedBundles[bundleName], null);
                return default;
            }
            else
            {
                callback += (req, assetBundle, err) =>
                {
                    Internal_OnAssetBundleDownloaded(bundleName, assetBundle, err);
                };
                AssetBundleDownloadRequest downloader = AssetBundleDownloader.Instance.Download(bundleName, crc, callback);
                return downloader;
            }
        }

        public void LoadAllAssetBundles(LoadAllAssetBundlesCallback callback)
        {
            AssetBundleBuildExport buildExport = AssetBundleHelper.GetAssetBundleBuildExport();
            if (buildExport != null && buildExport.AssetBundles != null && buildExport.AssetBundles.Length > 0)
            {
                int count = buildExport.AssetBundles.Length;
                int failCount = 0;
                int successCount = 0;
                for (int i = 0; i < buildExport.AssetBundles.Length; i++)
                {
                    string bundleName = buildExport.AssetBundles[i];
                    
                    LoadAssetBundle(
                        bundleName: bundleName,
                        crc: 0,
                        callback: (req, assetBundle, err) =>
                        {
                            count--;

                            if (string.IsNullOrEmpty(err))
                            {
                                successCount++;
                            }
                            else 
                            {
                                failCount++;
                            }
                            if (count == 0)
                            {
                                callback?.Invoke(successCount, failCount);
                            }
                        });
                }
            }
        }

        /// <summary>
        /// Create an instance of <c>T</c>, auto load bundle or download if undleName doesn't existed yet
        /// <param name = "assetName"> Name of asset is placed inside the bundle
        /// <param name = "bundleName"> Name of bundle contains the asset
        /// <param name = "callback"> Load result callback if any error occurs, the param <c>T</c> will be null
        /// <param name = "crc"> crc use to compare with downloaded bundle
        /// </summary>
        public void InstantiateAsset<T>(string bundleName, string assetName, System.Action<T> callback, uint crc = 0) where T : Object
        {
            if (_loadedBundles.ContainsKey(bundleName))
            {
                AssetBundle bundle = _loadedBundles[bundleName];                
                T asset = bundle.LoadAsset<T>(assetName);

                if (asset != null)
                {
                    T objectIns = GameObject.Instantiate<T>(asset, Vector3.zero, Quaternion.identity);
                    callback?.Invoke(objectIns);
                }
                else
                {
                    callback?.Invoke(null);
                }
            }
            else
            {
                LoadAssetBundle(
                    bundleName: bundleName,
                    crc: crc,
                    callback: (req, bundle, err) =>
                    {
                        if (bundle != null)
                        {
                            T asset = bundle.LoadAsset<T>(assetName);
                            T objectIns = GameObject.Instantiate<T>(asset, Vector3.zero, Quaternion.identity);
                            callback?.Invoke(objectIns);
                        }
                        else
                        {
                            callback?.Invoke(null);
                        }
                    });
            }
        }

        public void LoadScene(
            string bundleName, 
            string sceneName, 
            uint crc = 0,  
            LoadSceneMode loadMode = LoadSceneMode.Single,
            IAssetBundleSceneLoader sceneLoader = null)
        {
            if (_loadedBundles.ContainsKey(bundleName))
            {
                AssetBundle bundle = _loadedBundles[bundleName];                
                Internal_LoadScene(bundle, sceneName, loadMode, sceneLoader);
            }
            else
            {
                LoadAssetBundle(
                    bundleName: bundleName,
                    crc: crc,
                    callback: (req, bundle, err) =>
                    {
                        Internal_LoadScene(bundle, sceneName, loadMode, sceneLoader);
                    });
            }
        }

        public void LoadSceneAsync(
            string bundleName, 
            string sceneName, 
            uint crc = 0,  
            LoadSceneMode loadMode = LoadSceneMode.Single,
            IAssetBundleSceneLoader sceneLoader = null,
            SceneAsyncLoadingCallback sceneLoadingCallback = default)
        {
            if (_loadedBundles.ContainsKey(bundleName))
            {
                AssetBundle bundle = _loadedBundles[bundleName];                
                Internal_LoadSceneAsync(bundle, sceneName, loadMode, sceneLoader, sceneLoadingCallback);
            }
            else
            {
                LoadAssetBundle(
                    bundleName: bundleName,
                    crc: crc,
                    callback: (req, bundle, err) =>
                    {
                        Internal_LoadSceneAsync(bundle, sceneName, loadMode, sceneLoader, sceneLoadingCallback);
                    });
            }
        }

        /// <summary>
        /// Unload bundle with a specified name, if the name is null, the function will unload all ones
        /// </summary>
        public bool ReleaseAssetBundles(string specifiedBundle = null)
        {
            if (!string.IsNullOrEmpty(specifiedBundle))
            {
                if (_loadedBundles.ContainsKey(specifiedBundle))
                {
                    _loadedBundles[specifiedBundle].Unload(true);
                    return _loadedBundles.Remove(specifiedBundle);
                }
                return false;
            }
            else
            {
                foreach (KeyValuePair<string, AssetBundle> kvp in _loadedBundles)
                {
                    kvp.Value.Unload(true);
                } 
                _loadedBundles.Clear();
                return true;
            }
        }
    }

}