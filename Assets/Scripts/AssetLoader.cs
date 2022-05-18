using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class AssetLoader : MonoBehaviour
{
    public string assetRemotePath = "";
    public string assetBundleName = "";
    public string assetName = "";
    IEnumerator Start()
    {
        if (string.IsNullOrEmpty(assetRemotePath) && string.IsNullOrEmpty(assetBundleName) && string.IsNullOrEmpty(assetName))
        {
            yield break;
        }

        string bundlePath = Path.Combine(assetRemotePath, assetBundleName);
        UnityWebRequest req = UnityWebRequestAssetBundle.GetAssetBundle(bundlePath, 0);
        
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
            if (bundle != null)
            {
                Debug.Log("Download Success");

                GameObject go = bundle.LoadAsset<GameObject>(assetName);
                Instantiate(go, Vector3.zero, Quaternion.identity);

                
            }
            else
            {
                Debug.Log("Download Fail");
            }
        }
        else
        {
            Debug.LogError("Download Error " + bundlePath);
            Debug.LogError(req.error);
        }

        yield break;
    }
}
