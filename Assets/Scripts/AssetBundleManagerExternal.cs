using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace AssetBundleExample
{
	
	/// <summary>
	/// This example loads asset bundles from a remote server.
	/// The initial call finds the CSV manifest of the AssetBundles available on the server
	/// The CSV is parsed and each bundle is loaded in turn
	/// The example then keeps a reference to each asset bundle
	/// And scrapes any scenes from the bundles and adds them to the list of available scenes to load
	/// Finally, an OnGUI button is created for each scene which you can load the new asset bundle scenes with
	/// This component will not be destroyed when loading a new scene
	/// The list of asset bundles could be used to unload them when you no longer need them
	///
	/// This example assumes you have exported your asset bundles already
	/// using the Assets/Build AssetBundles menu option
	/// You have then uploaded these bundles onto a webserver ensuring they were transfered as binaries
	/// And then you have entered the correct location of the bundleLocation
	/// </summary>
	
	public class AssetBundleManagerExternal : MonoBehaviour
	{
		public string bundleLocation = "";
		public List<AssetBundle> bundles = new ();
		public List<string> sceneList = new ();

		private void LoadAssetBundles()
		{
			StartCoroutine(LoadBundleManifest());
		}

		/// <summary>
		/// Load the manifest CSV of asset bundles from the server
		/// </summary>
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

		/// <summary>
		/// Load a specific asset bundle from the server
		/// </summary>
		private IEnumerator LoadAssetBundle(string bundleName)
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
		}

		private void Start()
		{
			// for this example we'll keep this component alive when loading new scenes
			DontDestroyOnLoad(gameObject);
			// start loading the asset bundles from the server
			LoadAssetBundles();
		}

		/// <summary>
		/// Simple GUI to display the available asset bundle scenes
		/// </summary>
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