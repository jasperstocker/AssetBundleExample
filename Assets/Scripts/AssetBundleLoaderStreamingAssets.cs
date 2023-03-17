using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace AssetBundleExample
{
	
    /// <summary>
    /// This example loads asset bundles from the steaming assets folder of your build.
    /// The initial call finds the CSV manifest of the AssetBundles available
    /// The CSV is parsed and each bundle is loaded in turn
    /// The example then keeps a reference to each asset bundle
    /// And scrapes any scenes from the bundles and adds them to the list of available scenes to load
    /// Finally, an OnGUI button is created for each scene which you can load the new asset bundle scenes with
    /// This component will not be destroyed when loading a new scene
    /// The list of asset bundles could be used to unload them when you no longer need them
    ///
    /// This example assumes you have exported your asset bundles already
    /// using the Assets/Build AssetBundles menu option
    /// You have then placed these bundles into the Assets/StreamingAssets folder
    /// And then you have entered the correct location of the bundleLocation - which would likely be the platform subfolder
    /// </summary>
    public class AssetBundleLoaderStreamingAssets : MonoBehaviour
    {
        public string bundleLocation = "";
        public List<AssetBundle> bundles = new ();
        public List<string> sceneList = new ();

        private void LoadAssetBundles()
        {
            StartCoroutine(LoadBundleManifest());
        }
        
        private IEnumerator LoadBundleManifest()
        {
            string url = "file://" + Path.Combine(Application.streamingAssetsPath, bundleLocation, "assetbundles.csv");
            Debug.Log("Loading manifest from: " + url);
            UnityWebRequest www = UnityWebRequest.Get(url);
            yield return www.SendWebRequest();
            
            if (www.result != UnityWebRequest.Result.Success) 
            {
                Debug.Log(www.error);
            }
            else
            {
                string csvText = www.downloadHandler.text;
                string[] bundleManifest = csvText.Split(',');
                foreach (string bundleName in bundleManifest)
                {
                    LoadAssetBundle(bundleName);
                }
            }
        }

        private void LoadAssetBundle(string bundleName)
        {
            string url = Path.Combine(Application.streamingAssetsPath, bundleLocation, bundleName);
            Debug.Log("Loading assetbundle from: " + url);
            AssetBundle localAssetBundle = AssetBundle.LoadFromFile(url);
            bundles.Add(localAssetBundle);
            sceneList.AddRange(localAssetBundle.GetAllScenePaths());

            if (localAssetBundle == null) {
                Debug.LogError("Failed to load AssetBundle!");
            }
        } 

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            LoadAssetBundles();
        }

        private void OnGUI()
        {
            foreach (string sceneName in sceneList)
            {
                if (GUILayout.Button(sceneName))
                {
                    SceneManager.LoadSceneAsync(sceneName);
                }
            }
        }
    }
}