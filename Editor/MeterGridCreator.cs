#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace SeikaGameKit
{
    public class MeterGridCreator
    {
        private static readonly string MaterialPath = "Packages/jp.digicre.seika-game-kit/Resources/Materials/MeterGrid.mat";

        [MenuItem("GameObject/3D Object/Meter Grid (Plane)")]
        public static void CreateMeterGridPlane()
        {
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.name = "Meter Grid";

            Material gridMaterial = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (gridMaterial != null)
            {
                plane.GetComponent<Renderer>().sharedMaterial = gridMaterial;
            }
        }
    }
}
#endif