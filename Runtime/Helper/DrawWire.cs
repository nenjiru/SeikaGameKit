using UnityEngine;

namespace SeikaGameKit.Helper
{
    /// <summary>
    /// 領域をワイヤー描画
    /// </summary>
    public class DrawWire : MonoBehaviour
    {
        #region DEFINITION
        public enum DrawType
        {
            Cube,
            Sphere,
            Capsule,
            Collider,
        }
        #endregion

        #region VARIABLE
        [SerializeField, Tooltip("描画の有効化")] bool _enable = true;
        [SerializeField, Tooltip("描画色")] Color _color = Color.green;
        [SerializeField, Tooltip("描画の形状")] DrawType _type = DrawType.Cube;
        [SerializeField, Tooltip("領域を指定")] Vector3 _size = Vector3.one;
        [SerializeField, Tooltip("半径を指定")] float _radius = 0.5f;
        [SerializeField, Tooltip("高さを指定")] float _height = 2f;
        Collider _collider;
        #endregion

#if UNITY_EDITOR
        #region UNITY_EVENT
        void OnDrawGizmos()
        {
            if (_enable == false)
            {
                return;
            }

            Color tmpColor = Gizmos.color;
            Matrix4x4 tmpMat = Gizmos.matrix;
            Gizmos.color = _color;
            Gizmos.matrix = transform.localToWorldMatrix;

            if (_type == DrawType.Cube)
            {
                Gizmos.DrawWireCube(Vector3.zero, _size);
            }
            else if (_type == DrawType.Sphere)
            {
                Gizmos.DrawSphere(Vector3.zero, _radius);
            }
            else if (_type == DrawType.Capsule)
            {
                DrawWire.DrawCapsuleGizmo(transform, _radius, _height, 1, _color);
            }
            else if (_type == DrawType.Collider)
            {
                if (_collider == null)
                {
                    _collider = GetComponent<Collider>();
                }

                switch (_collider)
                {
                    case BoxCollider box:
                        Gizmos.DrawWireCube(Vector3.zero + box.center, box.size);
                        break;
                    case CapsuleCollider capsule:
                        DrawWire.DrawCapsuleGizmo(transform, capsule.radius, capsule.height, capsule.direction, _color);
                        break;
                    case SphereCollider sphere:
                        Gizmos.DrawWireSphere(Vector3.zero + sphere.center, sphere.radius);
                        break;
                }
            }

            Gizmos.color = tmpColor;
            Gizmos.matrix = tmpMat;
        }
        #endregion

        #region PUBLIC_METHOD
        public static void DrawCapsuleGizmo(Transform transform, float radius, float height, int direction, Color color = default)
        {
            DrawCapsuleGizmo(transform, radius, height, direction, Vector3.zero, color);
        }

        public static void DrawCapsuleGizmo(Transform transform, float radius, float height, Vector3 offset, Color color = default)
        {
            DrawCapsuleGizmo(transform, radius, height, 1, offset, color);
        }
        #endregion

        #region PRIVATE_METHOD
        /// <summary>
        /// Drawing capsule wires
        /// </summary>
        private static void DrawCapsuleGizmo(Transform transform, float radius, float height, int direction, Vector3 offset, Color color = default)
        {
            Color tmpColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = color == default ? Color.white : color;
            Quaternion rot = transform.rotation;
            switch (direction)
            {
                case 0: rot *= Quaternion.Euler(0, 0, 90); break;
                case 2: rot *= Quaternion.Euler(90, 0, 0); break;
            }

            Matrix4x4 matrix = Matrix4x4.TRS(transform.position + offset, rot, transform.lossyScale);
            using (new UnityEditor.Handles.DrawingScope(matrix))
            {
                float halfHeight = (height - (radius * 2)) / 2;
                //draw side ways
                UnityEditor.Handles.DrawWireArc(Vector3.up * halfHeight, Vector3.left, Vector3.back, -180, radius);
                UnityEditor.Handles.DrawLine(new Vector3(0, halfHeight, -radius), new Vector3(0, -halfHeight, -radius));
                UnityEditor.Handles.DrawLine(new Vector3(0, halfHeight, radius), new Vector3(0, -halfHeight, radius));
                UnityEditor.Handles.DrawWireArc(Vector3.down * halfHeight, Vector3.left, Vector3.back, 180, radius);
                //draw front ways
                UnityEditor.Handles.DrawWireArc(Vector3.up * halfHeight, Vector3.back, Vector3.left, 180, radius);
                UnityEditor.Handles.DrawLine(new Vector3(-radius, halfHeight, 0), new Vector3(-radius, -halfHeight, 0));
                UnityEditor.Handles.DrawLine(new Vector3(radius, halfHeight, 0), new Vector3(radius, -halfHeight, 0));
                UnityEditor.Handles.DrawWireArc(Vector3.down * halfHeight, Vector3.back, Vector3.left, -180, radius);
                //draw center
                UnityEditor.Handles.DrawWireDisc(Vector3.up * halfHeight, Vector3.up, radius);
                UnityEditor.Handles.DrawWireDisc(Vector3.down * halfHeight, Vector3.up, radius);
            }
            UnityEditor.Handles.color = tmpColor;
        }
        #endregion
#endif
    }
}