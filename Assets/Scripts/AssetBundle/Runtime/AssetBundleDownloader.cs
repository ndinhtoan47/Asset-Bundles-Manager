namespace GameFramework.AssetBundle.Runtime
{
    using UnityEngine;
    using System.Collections.Generic;
    using UnityEngine.Networking;

    public delegate void AssetBundleDownloadCallback(UnityEngine.Networking.UnityWebRequest req, AssetBundle bundle, string error);

    public struct AssetBundleDownloadRequest
    {
        public readonly uint id;
        public event AssetBundleDownloadCallback callback;
        public readonly UnityEngine.Networking.UnityWebRequest wwwRequest;
        public UnityWebRequestAsyncOperation operation;

        public AssetBundleDownloadRequest(uint initId, AssetBundleDownloadCallback initCb, UnityEngine.Networking.UnityWebRequest initReq)
        {
            id = initId;
            callback = initCb;
            wwwRequest = initReq;
            operation = null;
        }

        public void InvokeComplete(UnityEngine.Networking.UnityWebRequest req, AssetBundle bundle, string error)
        {
            callback?.Invoke(req, bundle, error);
        }
    }

    public class AssetBundleDownloader : MonoBehaviour
    {
        public const int MAX_REQUEST_AT_THE_SAME_TIME = 10;
        private static AssetBundleDownloader _instance = null;

        public static AssetBundleDownloader Instance 
        {
            get
            {
                if (_instance == null)
                {
                    System.Type insType = typeof(AssetBundleDownloader);
                    _instance = UnityEngine.GameObject.FindObjectOfType(insType) as AssetBundleDownloader;
                    if (_instance == null)
                    {
                        string insName = $"Singleton [{insType}]";

                        UnityEngine.GameObject obj = new UnityEngine.GameObject(insName, insType);                    
                        
                        obj.hideFlags = UnityEngine.HideFlags.DontSave;
                        
                        _instance = obj.GetComponent<AssetBundleDownloader>();
                    }
                }
                return _instance;
            }
        }

        // Do not set this, it will use config in Resources/AssetBundleSettings/AssetBundleSettings.asset
        private uint _assetVersion = 0;

        // Do not set this, it will use config in Resources/AssetBundleSettings/AssetBundleSettings.asset
        private string _contentHost = null;
        private string _platform = null;
        
        private uint _requestId = uint.MinValue;
        private List<AssetBundleDownloadRequest> _requests = new List<AssetBundleDownloadRequest>();
        private Queue<AssetBundleDownloadRequest> _pendingRequests = new Queue<AssetBundleDownloadRequest>();

        public AssetBundleDownloadRequest Download(string bundleName, uint crc, AssetBundleDownloadCallback callback)
        {
            // Initalize
            if (string.IsNullOrEmpty(_contentHost))
            {
                AssetBundleSettings settings = AssetBundleHelper.GetAssetBundleSettings();
                EnvironmentSettings current = settings.GetCurrentEnvironment();
                _contentHost = current.contentHost;
                _assetVersion = current.version;
                _platform = AssetBundleHelper.GetPlatform();
            }
            // -- Initalize

            string url = System.IO.Path.Combine(_contentHost, _platform, bundleName);
            UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle(url, _assetVersion ,crc);            
            return Request(req, callback);
        }

        private AssetBundleDownloadRequest Request(UnityWebRequest req, AssetBundleDownloadCallback callback)
        {
            if (req == null)
            {
                Debug.LogWarning("[AssetBundleDownloader] req is null");
                return default;
            }

            AssetBundleDownloadRequest apiRequest = new AssetBundleDownloadRequest(++_requestId, callback, req);

            if (_requests.Count < MAX_REQUEST_AT_THE_SAME_TIME)
            {
                try
                {
                    apiRequest.operation = req.SendWebRequest();
                    _requests.Add(apiRequest);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(string.Format("[AssetBundleDownloader] Internal Error: {0}", e.ToString()));
                }
            }
            else
            {
                _pendingRequests.Enqueue(apiRequest);
            }

            return apiRequest;
        }
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }
        
        private void Update()
        {
            int reqCount = _requests.Count;
            int pendingCount = _pendingRequests.Count;

            if (reqCount == 0 && pendingCount == 0) { return; }

            UpdateDownloading();
        }

        private void UpdateDownloading()
        {
            for (int i = _requests.Count - 1; i >= 0; i--)
            {
                AssetBundleDownloadRequest apiReq = _requests[i];
                try
                {
                    if (apiReq.operation.isDone)
                    {
                        if (apiReq.wwwRequest.result != UnityWebRequest.Result.Success)
                        {
                            Debug.LogError(string.Format("[AssetBundleDownloader] Error {0}{1}\nURL:{2}", apiReq.wwwRequest.responseCode, apiReq.wwwRequest.error, apiReq.wwwRequest.url));
                            apiReq.InvokeComplete(apiReq.wwwRequest, null, apiReq.wwwRequest.error);
                        }
                        else
                        {
                            try
                            {
                                AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(apiReq.wwwRequest);
                                apiReq.InvokeComplete(apiReq.wwwRequest, bundle, null);
                            }
                            catch (System.Exception e)
                            {
                                Debug.LogError(string.Format("[AssetBundleDownloader] Internal Error: {0}", e.ToString()));
                                apiReq.InvokeComplete(apiReq.wwwRequest, null, e.ToString());
                            }
                        }
                        apiReq.wwwRequest.Dispose();
                        _requests.RemoveAt(i);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError(string.Format("[AssetBundleDownloader] Internal Error: {0}", e.ToString()));
                    apiReq.wwwRequest.Dispose();
                    _requests.RemoveAt(i);
                }
            }
        }

        private void UpdatePendingDownload()
        {
            if (_requests.Count < MAX_REQUEST_AT_THE_SAME_TIME && _pendingRequests.Count > 0)
            {
                while (_requests.Count < MAX_REQUEST_AT_THE_SAME_TIME && _pendingRequests.Count > 0)
                {
                    AssetBundleDownloadRequest apiReq = _pendingRequests.Dequeue();
                    try
                    {
                        apiReq.operation = apiReq.wwwRequest.SendWebRequest();
                        _requests.Add(apiReq);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError(string.Format("[AssetBundleDownloader] Internal Error: {0}", e.ToString()));
                    }
                }
            }
        }
    }
}