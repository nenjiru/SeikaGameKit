#if UNITY_EDITOR
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
            if (target is MonoBehaviour)
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), typeof(Object), false);
            }
            else if (target is ScriptableObject)
            {
                EditorGUILayout.ObjectField("Script", MonoScript.FromScriptableObject((ScriptableObject)target), typeof(Object), false);
            }
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
#endif