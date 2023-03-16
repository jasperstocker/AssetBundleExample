using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace DefaultNamespace
{
	public class AssetBundleManager : MonoBehaviour
	{
		public string bundleLocation = "";
		public List<AssetBundle> bundles = new List<AssetBundle>();
		public List<string> sceneList = new List<string>();

		public void LoadAssetBundles()
		{
			StartCoroutine(LoadBundleManifest());
		}

		private IEnumerator LoadBundleManifest()
		{
			string url = Path.Combine(bundleLocation + "/assetbundles.csv");
			Debug.Log("Loading manifest from: " + url);
			UnityWebRequest www = UnityWebRequest.Get(url);
			yield return www.SendWebRequest();
 
			if (www.result != UnityWebRequest.Result.Success) {
				Debug.Log(www.error);
			}
			else {
				// Show results as text
				string csvText = www.downloadHandler.text;
				string[] bundleManifest = csvText.Split(',');
				foreach (string bundleName in bundleManifest)
				{
					StartCoroutine(LoadAssetBundle(bundleName));
				}
			}
		}
		
		public IEnumerator LoadAssetBundle(string bundleName)
		{
			string url = Path.Combine(bundleLocation + bundleName);
			Debug.Log("Loading bundle from: " + url);
			
			using (UnityWebRequest uwr = UnityWebRequestAssetBundle.GetAssetBundle(url))
			{
				yield return uwr.SendWebRequest();

				if (uwr.result != UnityWebRequest.Result.Success)
				{
					Debug.Log(uwr.error);
				}
				else
				{
					// Get downloaded asset bundle
					AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(uwr);
					bundles.Add(bundle);
					sceneList.AddRange(bundle.GetAllScenePaths());
				}
			}
			
			
			
			/*UnityWebRequest bundleLoadRequest = UnityWebRequestAssetBundle.GetAssetBundle(url);
			yield return bundleLoadRequest.SendWebRequest();
			
			if (bundleLoadRequest.result != UnityWebRequest.Result.Success) {
				Debug.Log(bundleLoadRequest.error);
			}
			else {
				// Show results as text
				AssetBundle bundle = bundleLoadRequest.downloadHandler.;
				string[] bundleManifest = csvText.Split(',');
				foreach (string bundleName in bundleManifest)
				{
					StartCoroutine(LoadAssetBundle(bundleName));
				}
			}
			
			// AssetBundleCreateRequest bundleLoadRequest = AssetBundle.LoadFromFileAsync(url);
			// yield return bundleLoadRequest;
			//
			// AssetBundle bundle = bundleLoadRequest.assetBundle;
			if (bundle == null)
			{
				Debug.Log("Failed to load AssetBundle!");
				yield break;
			}
			
			bundles.Add(bundle);
			sceneList.AddRange(bundle.GetAllScenePaths());*/
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