using System.IO;
using Backtory.Core.Internal;
using Backtory.Core.Public;
using UnityEditor;
using UnityEngine;

namespace Backtory.Unity
{
  [CustomEditor(typeof(BacktoryConfig))]
  public class BacktorySettingEditor : Editor
  {
	GUIContent AuthIdLabel = new GUIContent("X-Backtory-Authentication-Id [?]: ", "Needed for working with users. All Other services depend on it.");
	GUIContent AuthClientKeyLabel = new GUIContent("X-Backtory-Authentication-Key(Client) [?]: ", "Needed for working with users. All Other services depend on it.");
	GUIContent CloudcodeIdLabel = new GUIContent("Cloudcode id [?]: ", "Needed if you want to call your cloud-codes");
	GUIContent GameIdLabel = new GUIContent("X-Backtory-Game-Id [?]: ", "Needed if you want leader-board features");
	GUIContent ConnectivityIdLabel = new GUIContent("X-Backtory-Connectivity-Id [?]: ", "Needed if you want match making or chat features");
	GUIContent ObjectStorageIdLabel = new GUIContent("X-Backtory-Object-Storage-Id [?]: ", "Needed if you want to save/restore data to/from database.");
	GUIContent FileStorageIdLabel = new GUIContent("X-Backtory-Storage-Id [?]: ", "Needed if you want to upload files to server");
	GUIContent DebugModeLabel = new GUIContent("Debug Mode [?]: ", @"If checked, SDK prints extensive log messages which may slow your application. Useful for debugging.\n
																	 Be sure to uncheck it for publishing.");
	GUIContent VersionLabel = new GUIContent("SDK Version: ");
	GUIContent LogLevelLabel = new GUIContent("Log Level [?]: ", @"Controls if a log should be printed based on it's severity. You receive logs from selected level and it's higher levels.\n
																   Verbose: Low important or very frequent log. setting this level is not recommended!.\n
																   Debug: Logs useful for debugging specially network requests and responses details.\n
																   Warning: Shows warning messages (bad but not critical things) and error logs.\n
																   Error: Error logs indicating bad events which should have not happened.\n
																   Exception: Exception occurred during execution.");
	GUIContent ServerDestinationLabel = new GUIContent("Server address [?]: ", @"Switch between Backtory production and development servers. \n
																				 Be sure to select Production for publishing.");

	const string UnityAssetFolder = "Assets";
	public static BacktoryConfig GetOrCreateSettingsAsset()
	{
	  string fullPath = Path.Combine(Path.Combine(UnityAssetFolder, BacktoryConfig.BacktorySettingsPath),
									 BacktoryConfig.BacktorySettingsAssetName + BacktoryConfig.BacktorySettingsAssetExtension);

	  BacktoryConfig instance = AssetDatabase.LoadAssetAtPath(fullPath, typeof(BacktoryConfig)) as BacktoryConfig;

	  if (instance == null)
	  {
		// no asset found, we need to create it. 

		if (!Directory.Exists(Path.Combine(UnityAssetFolder, BacktoryConfig.BacktorySettingsPath)))
		{
		  AssetDatabase.CreateFolder(Path.Combine(UnityAssetFolder, "Backtory"), "Resources");
		}

		instance = CreateInstance<BacktoryConfig>();
		AssetDatabase.CreateAsset(instance, fullPath);
		AssetDatabase.SaveAssets();
	  }
	  return instance;
	}

	[MenuItem("Backtory/Edit Settings")]
	public static void Edit()
	{
	  Selection.activeObject = GetOrCreateSettingsAsset();
	}

	void OnDisable()
	{
	  // make sure the runtime code will load the Asset from Resources when it next tries to access this. 
	  BacktoryConfig.SetInstance(null);
	}




	public override void OnInspectorGUI()
	{
	  BacktoryConfig settings = (BacktoryConfig)target;
	  BacktoryConfig.SetInstance(settings);

	  EditorGUILayout.BeginHorizontal();
	  EditorGUILayout.LabelField(VersionLabel, GUILayout.Width(EditorGUIUtility.labelWidth));
	  EditorGUILayout.LabelField(BacktoryClient.SdkVersion);
	  EditorGUILayout.EndHorizontal();

	  EditorGUILayout.Space();


	  EditorGUILayout.HelpBox("Copy your application keys from panel and paste them here", MessageType.None);

	  EditorGUILayout.BeginHorizontal();
	  BacktoryConfig.BacktoryAuthId = EditorGUILayout.TextField(AuthIdLabel, BacktoryConfig.BacktoryAuthId);
	  EditorGUILayout.EndHorizontal();

	  EditorGUILayout.BeginHorizontal();
	  BacktoryConfig.BacktoryAuthClientKey = EditorGUILayout.TextField(AuthClientKeyLabel, BacktoryConfig.BacktoryAuthClientKey);
	  EditorGUILayout.EndHorizontal();

	  EditorGUILayout.BeginHorizontal();
	  BacktoryConfig.BacktoryObjectStorageId = EditorGUILayout.TextField(ObjectStorageIdLabel, BacktoryConfig.BacktoryObjectStorageId);
	  EditorGUILayout.EndHorizontal();

	  EditorGUILayout.BeginHorizontal();
	  BacktoryConfig.BacktoryGameId = EditorGUILayout.TextField(GameIdLabel, BacktoryConfig.BacktoryGameId);
	  EditorGUILayout.EndHorizontal();

	  EditorGUILayout.BeginHorizontal();
	  BacktoryConfig.BacktoryCloudcodeId = EditorGUILayout.TextField(CloudcodeIdLabel, BacktoryConfig.BacktoryCloudcodeId);
	  EditorGUILayout.EndHorizontal();

	  EditorGUILayout.BeginHorizontal();
	  BacktoryConfig.BacktoryFileStorageId = EditorGUILayout.TextField(FileStorageIdLabel, BacktoryConfig.BacktoryFileStorageId);
	  EditorGUILayout.EndHorizontal();

	  EditorGUILayout.BeginHorizontal();
	  BacktoryConfig.BacktoryConnectivityId = EditorGUILayout.TextField(ConnectivityIdLabel, BacktoryConfig.BacktoryConnectivityId);
	  EditorGUILayout.EndHorizontal();

	  EditorGUILayout.Space();


	  EditorGUILayout.HelpBox("Debug settings", MessageType.None);

	  BacktoryConfig.SdkDebugMode = EditorGUILayout.Toggle(DebugModeLabel, BacktoryConfig.SdkDebugMode);

	  EditorGUI.BeginDisabledGroup(BacktoryConfig.SdkDebugMode == false);
	  EditorGUILayout.BeginHorizontal();
	  BacktoryConfig.LogLevel = (LogLevel)EditorGUILayout.EnumPopup(LogLevelLabel, BacktoryConfig.LogLevel);
	  EditorGUILayout.EndHorizontal();

	  EditorGUI.EndDisabledGroup();

	  EditorGUILayout.BeginHorizontal();
	  BacktoryConfig.ServerDestination = (ServerDestination)EditorGUILayout.EnumPopup(
		ServerDestinationLabel,
		BacktoryConfig.ServerDestination);
	  EditorGUILayout.EndHorizontal();

	  if (GUI.changed)
	  {
		EditorUtility.SetDirty(settings);
		AssetDatabase.SaveAssets();
	  }
	}
  }
}