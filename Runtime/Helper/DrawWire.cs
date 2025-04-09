using UnityEngine;

namespace SeikaGameKit.Helper
{
    /// <summary>
    /// Drawing lines on game objects
    /// </summary>
    public class DrawWire : MonoBehaviour
    {
        #region DEFINITION
        public enum DrawType
        {
            Camera,
            Collider,
            Cube,
            Sphere,
            Capsule,
        }
        #endregion

        #region VARIABLE
        [SerializeField, Tooltip("描画の有効化")] bool _enable = true;
        [SerializeField, Tooltip("描画の形状")] DrawType _type = DrawType.Cube;
        [SerializeField, Tooltip("描画色")] Color _color = Color.green;
        [SerializeField, Tooltip("領域を指定")] Vector3 _size = Vector3.one;
        [SerializeField, Tooltip("半径を指定")] float _radius = 0.5f;
        [SerializeField, Tooltip("高さを指定")] float _height = 2f;
        Camera _camera = null;
        Collider _collider = null;
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

            if (_type == DrawType.Camera)
            {
                if (_camera == null)
                {
                    _camera = GetComponent<Camera>();
                }
                DrawCameraFrustum(_camera, _color);
            }
            else if (_type == DrawType.Cube)
            {
                Gizmos.DrawWireCube(Vector3.zero, _size);
            }
            else if (_type == DrawType.Sphere)
            {
                Gizmos.DrawWireSphere(Vector3.zero, _radius);
            }
            else if (_type == DrawType.Capsule)
            {
                DrawCapsuleGizmo(transform, _radius, _height, 1, _color);
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
                        DrawCapsuleGizmo(transform, capsule.radius, capsule.height, capsule.direction, _color);
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
        /// <summary>
        /// Drawing camera frustum
        /// </summary>
        public static void DrawCameraFrustum(Camera camera, Color color = default)
        {
            if (camera == null)
            {
                return;
            }

            float fov = camera.fieldOfView;
            float size = camera.orthographicSize;
            float max = camera.farClipPlane;
            float min = camera.nearClipPlane;
            float aspect = camera.aspect;

            Color tempColor = Gizmos.color;
            Gizmos.color = color == default ? Color.white : color;

            Matrix4x4 tempMat = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(camera.transform.position, camera.transform.rotation, new Vector3(aspect, 1.0f, 1.0f));

            if (camera.orthographic)
            {
                Gizmos.DrawWireCube(Vector3.forward * ((max - min) / 2.0f) + Vector3.forward * min, new Vector3(size * 2.0f * aspect, size * 2.0f, max - min));
            }
            else
            {
                Gizmos.DrawFrustum(Vector3.zero, fov, max, min, 1.0f);
            }

            Gizmos.color = tempColor;
            Gizmos.matrix = tempMat;
        }

        /// <summary>
        /// Draw capsule gizmo
        /// </summary>
        private static void DrawCapsuleGizmo(Transform transform, float radius, float height, int direction, Color color = default)
        {
            Color tmpColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = color == default ? Color.white : color;
            Quaternion rot = transform.rotation;
            switch (direction)
            {
                case 0: rot *= Quaternion.Euler(0, 0, 90); break;
                case 2: rot *= Quaternion.Euler(90, 0, 0); break;
            }

            Matrix4x4 matrix = Matrix4x4.TRS(transform.position, rot, transform.lossyScale);
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