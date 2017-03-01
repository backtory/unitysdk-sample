using System.IO;
using Backtory.Core.Internal;
using UnityEditor;
using UnityEngine;

namespace Backtory.Unity.Editor
{
  [CustomEditor(typeof(BacktoryConfig))]
  public class BacktoryConfigEditor : UnityEditor.Editor
  {

    GUIContent AuthIdLable = new GUIContent("X-Backtory-Authentication-Id");
    GUIContent AuthClientKeyLable = new GUIContent("X-Backtory-Authentication-Key(Client)");
    GUIContent CloudcodeIdLable = new GUIContent("Cloudcode id");
    GUIContent GameIdLable = new GUIContent("X-Backtory-Game-Id");
    GUIContent ConnectivityIdLable = new GUIContent("X-Backtory-Connectivity-Id");
    GUIContent ObjectStorageIdLable = new GUIContent("X-Backtory-Object-Storage-Id");
	GUIContent FileStorageIdLable = new GUIContent("X-Backtory-Storage-Id");

    const string UnityAssetFolder = "Assets";
    public static BacktoryConfig GetOrCreateSettingsAsset()
    {
      string fullPath = Path.Combine(Path.Combine(UnityAssetFolder, BacktoryConfig.BacktorySettingsPath),
                                     BacktoryConfig.BacktorySettingsAssetName + BacktoryConfig.BacktorySettingsAssetExtension
                                     );

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

      EditorGUILayout.HelpBox("Add the Backtory Api Keys", MessageType.None);

      EditorGUILayout.BeginHorizontal();
      BacktoryConfig.BacktoryAuthId = EditorGUILayout.TextField(AuthIdLable, BacktoryConfig.BacktoryAuthId);
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.BeginHorizontal();
      BacktoryConfig.BacktoryAuthClientKey = EditorGUILayout.TextField(AuthClientKeyLable, BacktoryConfig.BacktoryAuthClientKey);
      EditorGUILayout.EndHorizontal();

	   EditorGUILayout.BeginHorizontal();
      BacktoryConfig.BacktoryObjectStorageId = EditorGUILayout.TextField(ObjectStorageIdLable, BacktoryConfig.BacktoryObjectStorageId);
      EditorGUILayout.EndHorizontal();
	  
	  EditorGUILayout.BeginHorizontal();
      BacktoryConfig.BacktoryGameId = EditorGUILayout.TextField(GameIdLable, BacktoryConfig.BacktoryGameId);
      EditorGUILayout.EndHorizontal();
	  
      EditorGUILayout.BeginHorizontal();
      BacktoryConfig.BacktoryCloudcodeId = EditorGUILayout.TextField(CloudcodeIdLable, BacktoryConfig.BacktoryCloudcodeId);
      EditorGUILayout.EndHorizontal();

	  EditorGUILayout.BeginHorizontal();
      BacktoryConfig.BacktoryFileStorageId = EditorGUILayout.TextField(FileStorageIdLable, BacktoryConfig.BacktoryFileStorageId);
      EditorGUILayout.EndHorizontal();	

      EditorGUILayout.BeginHorizontal();
      BacktoryConfig.BacktoryConnectivityId = EditorGUILayout.TextField(ConnectivityIdLable, BacktoryConfig.BacktoryConnectivityId);
      EditorGUILayout.EndHorizontal();

     

      EditorGUILayout.Space();




      if (GUI.changed)
      {
        EditorUtility.SetDirty(settings);
        AssetDatabase.SaveAssets();
      }
    }
  }
}
