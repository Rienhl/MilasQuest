using MilasQuest.GameData;
using UnityEditor;
using UnityEngine;

namespace MilasQuest.EditorTools
{
    [CustomEditor(typeof(Initializer))]
    public class InitializerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Update Level Datas"))
            {
                Initializer initializer = target as Initializer;
                string[] guids = AssetDatabase.FindAssets("t:LevelData", null);
                if (guids.Length == 0)
                {
                    EditorUtility.DisplayDialog("Warning", "Could not find any LevelData files", "Ok");
                    return;
                }
                Debug.Log("Found " + guids.Length + " LevelData files in the project");
                LevelData[] _levelDatas = new LevelData[guids.Length];
                for (int i = 0; i < guids.Length; i++)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                    _levelDatas[i] = AssetDatabase.LoadAssetAtPath<LevelData>(path);
                }
                initializer.SetLevelDatas(_levelDatas);
                EditorUtility.SetDirty(initializer);
                serializedObject.ApplyModifiedProperties();
            }
            base.OnInspectorGUI();
        }
    }
}
