using UnityEngine;
using UnityEditor;

namespace SeikaGameKit.Helper
{
    [CustomEditor(typeof(DrawWire))]
    public class DrawWireEditor : BaseEditor
    {
        DrawWire _target;
        SerializedProperty _enable;
        SerializedProperty _color;
        SerializedProperty _type;
        SerializedProperty _size;
        SerializedProperty _radius;

        void OnEnable()
        {
            _target = (DrawWire)target;
            _enable = serializedObject.FindProperty("_enable");
            _color = serializedObject.FindProperty("_color");
            _type = serializedObject.FindProperty("_type");
            _size = serializedObject.FindProperty("_size");
            _radius = serializedObject.FindProperty("_radius");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.ScriptField();

            EditorGUILayout.PropertyField(_enable);
            EditorGUILayout.PropertyField(_color);

            var type = (DrawWire.DrawType)EditorGUILayout.Popup("Type", _type.enumValueIndex, System.Enum.GetNames(typeof(DrawWire.DrawType)));
            if (type == DrawWire.DrawType.Cube)
            {
                EditorGUILayout.PropertyField(_size);
            }
            if (type == DrawWire.DrawType.Sphere)
            {
                EditorGUILayout.PropertyField(_radius);
            }
            _type.enumValueIndex = (int)type;

            serializedObject.ApplyModifiedProperties();
        }
    }
}