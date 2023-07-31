#if UNITY_EDITOR
using UnityEngine;

namespace SeikaGameKit.Helper
{
    /// <summary>
    /// 撮影範囲を表示する
    /// </summary>
    public class CameraGizmo : MonoBehaviour
    {
        public Color gizmosColor = Color.gray;
        Camera _camera = null;

        void OnDrawGizmos()
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            float fov = _camera.fieldOfView;
            float size = _camera.orthographicSize;
            float max = _camera.farClipPlane;
            float min = _camera.nearClipPlane;
            float aspect = _camera.aspect;

            Color tempColor = Gizmos.color;
            Gizmos.color = gizmosColor;

            Matrix4x4 tempMat = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, new Vector3(aspect, 1.0f, 1.0f));

            if (_camera.orthographic)
            {
                Gizmos.DrawWireCube(new Vector3(0.0f, 0.0f, ((max - min) / 2.0f) + min), new Vector3(size * 2.0f, size * 2.0f, max - min));
            }
            else
            {
                Gizmos.DrawFrustum(Vector3.zero, fov, max, min, 1.0f);
            }

            Gizmos.color = tempColor;
            Gizmos.matrix = tempMat;
        }
    }
}
#endif