using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleGenerator : MonoBehaviour
{
	[MenuItem("Assets/Build AssetBundles")]
	static void ExportResource()
	{
		// Bring up save panel
		string path = EditorUtility.SaveFolderPanel("Save Resources", "", "");
		BuildTarget[] targetPlatforms = { BuildTarget.Android, BuildTarget.StandaloneWindows };

		if (path.Length != 0)
		{
			foreach (BuildTarget targetPlatform in targetPlatforms)
			{
				string platformPath = Path.Combine(path, targetPlatform.ToString());
				bool exists = Directory.Exists(platformPath);
				if (!exists)
				{
					Directory.CreateDirectory(platformPath);
				}
				
				
				
				// BuildPipeline.BuildAssetBundle(Selection.activeObject,
				// 	selection, path + ".android.unity3d",
				// 	BuildAssetBundleOptions.CollectDependencies |
				// 	BuildAssetBundleOptions.CompleteAssets,
				// 	BuildTarget.Android);
				
				
				
				AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(platformPath, BuildAssetBundleOptions.None, targetPlatform);

				string[] names = manifest.GetAllAssetBundles();
				string fileName = Path.Combine(platformPath, "assetbundles.csv");
				using (StreamWriter file = new (fileName))
				{
					file.Write(string.Join(",", names));
				}
			}
		}
	}
}