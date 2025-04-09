#if UNITY_EDITOR
using UnityEditor;

namespace SeikaGameKit.Helper
{
    [CustomEditor(typeof(DrawWire))]
    public class DrawWireEditor : BaseEditor
    {
        SerializedProperty _enable;
        SerializedProperty _type;
        SerializedProperty _color;
        SerializedProperty _size;
        SerializedProperty _radius;
        SerializedProperty _height;
        string[] _drawTypes;

        void OnEnable()
        {
            _enable = serializedObject.FindProperty("_enable");
            _type = serializedObject.FindProperty("_type");
            _color = serializedObject.FindProperty("_color");
            _size = serializedObject.FindProperty("_size");
            _radius = serializedObject.FindProperty("_radius");
            _height = serializedObject.FindProperty("_height");
            _drawTypes = System.Enum.GetNames(typeof(DrawWire.DrawType));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.ScriptField();

            EditorGUILayout.PropertyField(_enable);

            if (_enable.boolValue)
            {
                var type = (DrawWire.DrawType)EditorGUILayout.Popup("Type", _type.enumValueIndex, _drawTypes);
                if (type == DrawWire.DrawType.Cube)
                {
                    EditorGUILayout.PropertyField(_size);
                }
                if (type == DrawWire.DrawType.Sphere)
                {
                    EditorGUILayout.PropertyField(_radius);
                }
                if (type == DrawWire.DrawType.Capsule)
                {
                    EditorGUILayout.PropertyField(_height);
                    EditorGUILayout.PropertyField(_radius);
                }
                _type.enumValueIndex = (int)type;

                EditorGUILayout.PropertyField(_color);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif