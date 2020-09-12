using UnityEditor;

namespace MilasQuest.GameData
{
    [CustomEditor(typeof(BaseData))]
    public class BaseDataEditor : Editor
    {
        SerializedProperty id;
        SerializedProperty definedName;

        private void OnEnable()
        {
            DoEnable();
        }

        protected virtual void DoEnable()
        {
            id = serializedObject.FindProperty("id");
            definedName = serializedObject.FindProperty("definedName");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(serializedObject.targetObject.name + " (ID #" + id.intValue.ToString() + ")", EditorStyles.centeredGreyMiniLabel);
            definedName.stringValue = EditorGUILayout.TextField("Defined Name: ", definedName.stringValue);
            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
