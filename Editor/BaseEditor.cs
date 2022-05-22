using UnityEngine;
using UnityEditor;

namespace SeikaGameKit
{
    public class BaseEditor : Editor
    {
        bool _inspectorFoldout = false;

        protected virtual void ScriptField()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(MonoScript), false);
            EditorGUILayout.ObjectField("Editor", MonoScript.FromScriptableObject(this), typeof(BaseEditor), false);
            EditorGUI.EndDisabledGroup();
        }

        protected virtual void RawInspector()
        {
            _inspectorFoldout = EditorGUILayout.Foldout(_inspectorFoldout, "Raw Inspector", true);
            if (_inspectorFoldout)
            {
                base.OnInspectorGUI();
            }
        }
    }
}