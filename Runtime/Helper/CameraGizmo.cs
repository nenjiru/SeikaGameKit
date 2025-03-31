using UnityEngine;

namespace SeikaGameKit.Helper
{
    /// <summary>
    /// カメラの視錐台を描画する
    /// </summary>
    public class CameraGizmo : MonoBehaviour
    {
        #region VARIABLE
        [SerializeField]
        Color _drawColor = Color.cyan;
        Camera _camera = null;
        #endregion

        #region UNITY_EVENT
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
            }

            CameraGizmo.DrawCameraFrustum(_camera, _drawColor);
        }
#endif
        #endregion

        #region PUBLIC_METHOD
        /// <summary>
        /// カメラ視錐台を描画
        /// </summary>
#if UNITY_EDITOR
        public static void DrawCameraFrustum(Camera camera, Color color = default)
        {
            if (camera == null) return;

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
#endif
        #endregion

        #region PRIVATE_METHOD
        #endregion
    }
}