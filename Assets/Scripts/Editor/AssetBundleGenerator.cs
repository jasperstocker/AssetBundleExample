using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleGenerator : MonoBehaviour
{
	/// <summary>
	/// This is the editor script to process all asset bundles in the project and export them to a folder
	/// It should be living in an Editor folder
	/// You can find the build option in the Assets menu > Build AssetBundles
	///
	/// The script will create the CSV manifest file for you which details the asset bundles that are exported
	/// You can then put these bundles on a webserver and load them from there
	/// Or place them in the streaming assets folder and load them from there
	///
	/// Bundles are placed into a subfolder for each platform
	///
	/// If you upload these to a webserver, ensure they are uploaded as binary files 
	///
	/// Known issue
	/// For some reason this will create an extra asset bundle named after the folder you are exporting to
	/// You can safely delete that asset bundle
	/// </summary>
	
	[MenuItem("Assets/Build AssetBundles")]
	static void ExportResource()
	{
		// Bring up save panel
		string path = EditorUtility.SaveFolderPanel("Save Resources", "", "");
		// here we have hardcoded the platforms we want to build for - you can change this
		BuildTarget[] targetPlatforms = { BuildTarget.Android, BuildTarget.StandaloneWindows };

		// if no path is selected, return
		if (path.Length == 0) return;
		
		foreach (BuildTarget targetPlatform in targetPlatforms)
		{
			string platformPath = Path.Combine(path, targetPlatform.ToString());
			bool exists = Directory.Exists(platformPath);
			if (!exists)
			{
				Directory.CreateDirectory(platformPath);
			}
				
			AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(platformPath, BuildAssetBundleOptions.None, targetPlatform);

			string[] names = manifest.GetAllAssetBundles();
			string fileName = Path.Combine(platformPath, "assetbundles.csv");
			using StreamWriter file = new (fileName);
			file.Write(string.Join(",", names));
		}
	}
}