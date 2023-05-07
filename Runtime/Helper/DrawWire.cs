using UnityEngine;
using UnityEditor;

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
            Collider,
        }
        #endregion

        #region VARIABLE
        [SerializeField, Tooltip("描画の有効化")] bool _enable = true;
        [SerializeField, Tooltip("描画色")] Color _color = Color.green;
        [SerializeField, Tooltip("描画の形状")] DrawType _type = DrawType.Cube;
        [SerializeField, Tooltip("領域を指定")] Vector3 _size = Vector3.one;
        [SerializeField, Tooltip("半径を指定")] float _radius = 0.5f;
        Collider _collider;
        #endregion

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
            else if (_type == DrawType.Collider)
            {
                if (_collider == null)
                {
                    _collider = GetComponent<Collider>();
                }

                if (_collider is BoxCollider)
                {
                    Gizmos.DrawWireCube(Vector3.zero + (_collider as BoxCollider).center, (_collider as BoxCollider).size);
                }
                else if (_collider is CapsuleCollider)
                {
                    DrawCapsuleGizmo(_collider as CapsuleCollider);
                }
                else if (_collider is SphereCollider)
                {
                    Gizmos.DrawWireSphere(Vector3.zero + (_collider as SphereCollider).center, (_collider as SphereCollider).radius);
                }
            }

            Gizmos.color = tmpColor;
            Gizmos.matrix = tmpMat;
        }
        #endregion

        #region PUBLIC_METHODS
        #endregion

        #region PRIVATE_METHODS
        private void DrawCapsuleGizmo(CapsuleCollider capsule)
        {
            Color tmpColor = Handles.color;
            Handles.color = _color;
            Matrix4x4 matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
            using (new Handles.DrawingScope(matrix))
            {
                float radius = capsule.radius;
                float offset = (capsule.height - (radius * 2)) / 2;

                //draw side ways
                Handles.DrawWireArc(Vector3.up * offset, Vector3.left, Vector3.back, -180, radius);
                Handles.DrawLine(new Vector3(0, offset, -radius), new Vector3(0, -offset, -radius));
                Handles.DrawLine(new Vector3(0, offset, radius), new Vector3(0, -offset, radius));
                Handles.DrawWireArc(Vector3.down * offset, Vector3.left, Vector3.back, 180, radius);
                //draw front ways
                Handles.DrawWireArc(Vector3.up * offset, Vector3.back, Vector3.left, 180, radius);
                Handles.DrawLine(new Vector3(-radius, offset, 0), new Vector3(-radius, -offset, 0));
                Handles.DrawLine(new Vector3(radius, offset, 0), new Vector3(radius, -offset, 0));
                Handles.DrawWireArc(Vector3.down * offset, Vector3.back, Vector3.left, -180, radius);
                //draw center
                Handles.DrawWireDisc(Vector3.up * offset, Vector3.up, radius);
                Handles.DrawWireDisc(Vector3.down * offset, Vector3.up, radius);
            }
            Handles.color = tmpColor;
        }
        #endregion
    }
}