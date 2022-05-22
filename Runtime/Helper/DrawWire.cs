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
            Collider,
        }
        #endregion

        #region VARIABLE
        [SerializeField, Tooltip("描画の有効化")] bool _enable = true;
        [SerializeField, Tooltip("描画色")] Color _color = Color.green;
        [SerializeField, Tooltip("描画の形状")] DrawType _type = DrawType.Cube;
        [SerializeField, Tooltip("領域を指定")] Vector3 _size = Vector3.one;
        [SerializeField, Tooltip("半径を指定")] float _radius = 0.5f;
        BoxCollider _box;
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

            if (_type == DrawType.Collider && _box == null)
            {
                _box = GetComponent<BoxCollider>();
            }

            switch (_type)
            {
                case DrawType.Cube: Gizmos.DrawWireCube(Vector3.zero, _size); break;
                case DrawType.Sphere: Gizmos.DrawSphere(Vector3.zero, _radius); break;
                case DrawType.Collider: Gizmos.DrawWireCube(Vector3.zero + _box.center, _box.size); break;
            }

            Gizmos.color = tmpColor;
            Gizmos.matrix = tmpMat;
        }
        #endregion

        #region PUBLIC_METHODS
        #endregion

        #region PRIVATE_METHODS
        #endregion
    }
}