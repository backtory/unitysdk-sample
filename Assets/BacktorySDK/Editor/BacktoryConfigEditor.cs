using UnityEngine;
using UnityEditor;
using System.IO;
using Assets.BacktorySDK.core;

namespace Assets.BacktorySDK.Editor
{
    [CustomEditor(typeof(BacktoryConfig))]
    public class BacktoryConfigEditor : UnityEditor.Editor
    {

        GUIContent AuthInstanceIdLable = new GUIContent("Auth instance id");
        GUIContent AuthClientKeyLable = new GUIContent("Auth client key");
        GUIContent CloudcodeInstanceIdLable = new GUIContent("Cloud-code instance id");
        GUIContent GameInstanceIdLable = new GUIContent("Game instance id");
        GUIContent RealtimeInstanceIdLable = new GUIContent("Connectivity instance id");

        const string UnityAssetFolder = "Assets";
        public static BacktoryConfig GetOrCreateSettingsAsset()
        {
            string fullPath = Path.Combine(Path.Combine(UnityAssetFolder, BacktoryConfig.gamesparksSettingsPath),
                                           BacktoryConfig.gamesparksSettingsAssetName + BacktoryConfig.gamesparksSettingsAssetExtension
                                           );

            BacktoryConfig instance = AssetDatabase.LoadAssetAtPath(fullPath, typeof(BacktoryConfig)) as BacktoryConfig;

            if (instance == null)
            {
                // no asset found, we need to create it. 

                if (!Directory.Exists(Path.Combine(UnityAssetFolder, BacktoryConfig.gamesparksSettingsPath)))
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
            BacktoryConfig.BacktoryAuthInstanceId = EditorGUILayout.TextField(AuthInstanceIdLable, BacktoryConfig.BacktoryAuthInstanceId);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            BacktoryConfig.BacktoryAuthClientKey = EditorGUILayout.TextField(AuthClientKeyLable, BacktoryConfig.BacktoryAuthClientKey);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            BacktoryConfig.BacktoryCloudcodeInstanceId = EditorGUILayout.TextField(CloudcodeInstanceIdLable, BacktoryConfig.BacktoryCloudcodeInstanceId);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            BacktoryConfig.BacktoryGameInstanceId = EditorGUILayout.TextField(GameInstanceIdLable, BacktoryConfig.BacktoryGameInstanceId);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            BacktoryConfig.BacktoryConnectivityInstanceId = EditorGUILayout.TextField(RealtimeInstanceIdLable, BacktoryConfig.BacktoryConnectivityInstanceId);
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
