using MilasQuest.GameData;
using MilasQuest.Grids.GameData;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MilasQuest.EditorTools
{
    public class EditorWindow_DataManager : EditorWindow
    {
        CellTypeData[] _cellDatas;
        List<int> _usedIds;
        private Vector2 scrollPos;

        [MenuItem("MilasQuest/Editor Tools/Data Manager")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(EditorWindow_DataManager), false, "Data Manager");
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("Remember to always run the update process on this Data Manager after creating new CellTypeData files.\nThe Asset PostProcessor is not used since we do not want to run this EVERY time a new data is create, just when we are done creating new ones.", MessageType.Info);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Button("New Data", EditorStyles.toolbarButton))
            {
                if (EditorUtility.DisplayDialog("Creating New Data", "Do you want to create a new CellTypeData?", "Yes", "No"))
                {
                    var path = EditorUtility.SaveFilePanel("Creating New CellTypeData", "/MilasQuest/Data/CellTypes/", "New Cell Type Data", "asset");
                    Debug.Log(path);
                    if (path.Length != 0)
                    {
                        path = path.Replace(Application.dataPath, "Assets");
                        CellTypeData data = CreateInstance<CellTypeData>();
                        data.id = GetUnusedInt();
                        EditorUtility.SetDirty(data);
                        AssetDatabase.CreateAsset(data, path);
                        AssetDatabase.SaveAssets();
                        AssetDatabase.Refresh();
                        GetCellDatas();
                    }
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (_cellDatas == null)
            {
                GUI.enabled = false;
                EditorGUILayout.HelpBox("No CellTypeData files found in project", MessageType.Warning);
                GetCellDatas();
            }

            if (GUILayout.Button("SAVE AND UPDATE"))
            {
                UpdateProcess();
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (int i = 0; i < _cellDatas.Length; i++)
            {
                GUILayout.BeginVertical(EditorStyles.helpBox);
                Editor.CreateEditor(_cellDatas[i]).OnInspectorGUI();
                GUILayout.EndVertical();
                GUILayout.Space(20);
            }
            EditorGUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();


        }

        private void UpdateProcess()
        {
            //assign ids
            for (int i = 0; i < _cellDatas.Length; i++)
            {
                if (_cellDatas[i].id == -1 || _usedIds.Contains(_cellDatas[i].id))
                {
                    _cellDatas[i].id = GetUnusedInt();
                }
            EditorUtility.SetDirty(_cellDatas[i]);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorTools.GenerateEnumFromDatas("CELL_TYPES", _cellDatas, "/MilasQuest/Scripts/Generated/", "MilasQuest");
        }

        private int GetUnusedInt()
        {
            int rnd = EditorTools.GetRandomInt();
            if (!_usedIds.Contains(rnd))
            {
                _usedIds.Add(rnd);
                return rnd;
            }
            else
                return GetUnusedInt();

        }

        private void GetCellDatas()
        {
            string[] guids = AssetDatabase.FindAssets("t:CellTypeData", null);
            if (guids.Length == 0)
            {
                EditorUtility.DisplayDialog("Warning", "Could not find any CellTypeData files", "Ok");
                return;
            }
            Debug.Log("Found " + guids.Length + " CellTypeData files in the project");
            _cellDatas = new CellTypeData[guids.Length];
            if (_usedIds == null)
                _usedIds = new List<int>();
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                _cellDatas[i] = AssetDatabase.LoadAssetAtPath<CellTypeData>(path);
                if (_cellDatas[i].id != -1)
                    _usedIds.Add(_cellDatas[i].id);
            }
            GUI.enabled = true;
        }
    }
}
