#if UNITY_EDITOR
using UnityEditor;

namespace SeikaGameKit.Helper
{
    [CustomEditor(typeof(DrawWire), true)]
    [CanEditMultipleObjects]
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

            // Handle mixed values for multi-object editing
            EditorGUI.showMixedValue = _enable.hasMultipleDifferentValues;
            EditorGUILayout.PropertyField(_enable);
            EditorGUI.showMixedValue = false;

            if (_enable.boolValue)
            {
                EditorGUI.showMixedValue = _type.hasMultipleDifferentValues;
                var type = (DrawWire.DrawType)EditorGUILayout.Popup("Type", _type.enumValueIndex, _drawTypes);
                _type.enumValueIndex = (int)type;
                EditorGUI.showMixedValue = false;

                if (type == DrawWire.DrawType.Cube)
                {
                    EditorGUI.showMixedValue = _size.hasMultipleDifferentValues;
                    EditorGUILayout.PropertyField(_size);
                    EditorGUI.showMixedValue = false;
                }
                if (type == DrawWire.DrawType.Sphere)
                {
                    EditorGUI.showMixedValue = _radius.hasMultipleDifferentValues;
                    EditorGUILayout.PropertyField(_radius);
                    EditorGUI.showMixedValue = false;
                }
                if (type == DrawWire.DrawType.Capsule)
                {
                    EditorGUI.showMixedValue = _height.hasMultipleDifferentValues;
                    EditorGUILayout.PropertyField(_height);
                    EditorGUI.showMixedValue = false;

                    EditorGUI.showMixedValue = _radius.hasMultipleDifferentValues;
                    EditorGUILayout.PropertyField(_radius);
                    EditorGUI.showMixedValue = false;
                }

                EditorGUI.showMixedValue = _color.hasMultipleDifferentValues;
                EditorGUILayout.PropertyField(_color);
                EditorGUI.showMixedValue = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif