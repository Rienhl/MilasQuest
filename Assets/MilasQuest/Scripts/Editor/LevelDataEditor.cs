
using MilasQuest.Stats;
using System;
using UnityEditor;
using UnityEngine;

namespace MilasQuest.GameData
{
    [CustomEditor(typeof(LevelData))]
    public class LevelDataEditor : BaseDataEditor
    {
        EndLevelData endLevelData;
        Vector2 successConditionsScrollPos;
        Vector2 failureConditionsScrollPos;

        protected override void DoEnable()
        {
            base.DoEnable();
            endLevelData = (target as LevelData).endLevelData;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField("Success Conditions", EditorStyles.boldLabel);
            if (GUILayout.Button("Add General Condition", EditorStyles.miniButton))
            {
                endLevelData.successConditions.generalConditions.Add(new GeneralStatConditionData());
            }
            if (GUILayout.Button("Add Gathered Cells Condition", EditorStyles.miniButton))
            {
                endLevelData.successConditions.gatheredCellsConditions.Add(new GatheredCellsConditionData());
            }
            EditorGUILayout.EndHorizontal();
            successConditionsScrollPos = EditorGUILayout.BeginScrollView(successConditionsScrollPos, GUILayout.ExpandWidth(false));
            for (int i = 0; i < endLevelData.successConditions.generalConditions.Count; i++)
            {
                DrawGeneralStatEndCondition(endLevelData.successConditions.generalConditions[i], () => endLevelData.successConditions.generalConditions.Remove(endLevelData.successConditions.generalConditions[i]));
            }
            for (int i = 0; i < endLevelData.successConditions.gatheredCellsConditions.Count; i++)
            {
                DrawGatheredCellsEndCondition(endLevelData.successConditions.gatheredCellsConditions[i], () => endLevelData.successConditions.gatheredCellsConditions.Remove(endLevelData.successConditions.gatheredCellsConditions[i]));
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal("Box");
            EditorGUILayout.LabelField("Failure Conditions", EditorStyles.boldLabel);
            if (GUILayout.Button("Add General Condition", EditorStyles.miniButton))
            {
                endLevelData.failureConditions.Add(new GeneralStatConditionData());
            }
            EditorGUILayout.EndHorizontal();
            failureConditionsScrollPos = EditorGUILayout.BeginScrollView(failureConditionsScrollPos);
            for (int i = 0; i < endLevelData.failureConditions.Count; i++)
            {
                DrawGeneralStatEndCondition(endLevelData.failureConditions[i], () => endLevelData.failureConditions.Remove(endLevelData.failureConditions[i]));
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            EditorGUILayout.HelpBox("Remember to always SAVE your changes", MessageType.Warning);
            if (GUILayout.Button("SAVE CHANGES", GUILayout.Height(100)))
            {
                EditorUtility.SetDirty(target);
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            
        }
        }

        private void DrawGeneralStatEndCondition(GeneralStatConditionData condition, Action onDelete)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Tracked Stat: ", GUILayout.MaxWidth(100));
            condition.statType = (STAT_TYPE)EditorGUILayout.EnumPopup(condition.statType, GUILayout.MaxWidth(250));
            EditorGUILayout.LabelField(" is ", GUILayout.MaxWidth(30));
            condition.relationalOperator = (RELATIONAL_OPERATOR)EditorGUILayout.EnumPopup(condition.relationalOperator, GUILayout.MaxWidth(250));
            condition.targetValue = EditorGUILayout.FloatField(condition.targetValue, GUILayout.MaxWidth(60));
            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("Warning!", "Do you want to delete this condition data?", "Yes", "No"))
                {
                    onDelete?.Invoke();
                    EditorUtility.SetDirty(target);
                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawGatheredCellsEndCondition(GatheredCellsConditionData condition, Action onDelete)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Gather ", GUILayout.MaxWidth(80));
            condition.relationalOperator = (RELATIONAL_OPERATOR)EditorGUILayout.EnumPopup(condition.relationalOperator, GUILayout.MaxWidth(250));
            condition.targetValue = EditorGUILayout.FloatField(condition.targetValue, GUILayout.MaxWidth(60));
            condition.cellType = (CELL_TYPES)EditorGUILayout.EnumPopup(condition.cellType, GUILayout.MaxWidth(250));
            if (GUILayout.Button("X", GUILayout.Width(30)))
            {
                if (EditorUtility.DisplayDialog("Warning!", "Do you want to delete this condition data?", "Yes", "No"))
                {
                    onDelete?.Invoke();
                    EditorUtility.SetDirty(target);
                    serializedObject.ApplyModifiedProperties();
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}