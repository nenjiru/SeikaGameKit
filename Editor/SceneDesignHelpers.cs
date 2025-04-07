#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace SeikaGameKit
{
    public class SceneDesignHelpers
    {
        private static readonly string MaterialPath = "Packages/jp.digicre.seika-game-kit/Resources/Materials/ScaleGrid.mat";

        [MenuItem("GameObject/Seika Game Kit/Scale Grid (10m)", false)]
        public static void CreateScaleGrid()
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "Scale Grid (10m)";

            Material gridMaterial = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (gridMaterial != null)
            {
                plane.GetComponent<Renderer>().sharedMaterial = gridMaterial;
            }
        }
    }
}
#endif