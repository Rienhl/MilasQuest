using UnityEditor;
using UnityEngine;

namespace MilasQuest.GameData
{
    [CustomEditor(typeof(CellTypeData))]
    public class CellTypeDataEditor : BaseDataEditor
    {
        SerializedProperty sprite;

        protected override void DoEnable()
        {
            base.DoEnable();
            sprite = serializedObject.FindProperty("sprite");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.BeginVertical();
            sprite.objectReferenceValue = (Sprite)EditorGUILayout.ObjectField("Sprite: ", sprite.objectReferenceValue, typeof(Sprite), false);
            EditorGUILayout.EndVertical();

        }
    }
}
