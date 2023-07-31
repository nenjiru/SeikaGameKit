using UnityEngine;
using UnityEditor;
using System.IO;

namespace SeikaGameKit
{
    public class CustomLayout : MonoBehaviour
    {
        private const string LAYOUT_ASSET = "Packages/jp.digicre.seika-game-kit/Resources/Layouts/SGKitLayout.wlt";

        [MenuItem("Seika Game Kit/Load Custom Layout")]
        static void LoadCustomLayout()
        {
            string path = Path.GetFullPath(LAYOUT_ASSET);
            if (System.IO.File.Exists(path))
            {
                EditorUtility.LoadWindowLayout(path);
            }
            else
            {
                Debug.LogWarning("Layout not loaded. Layout file missing at: " + LAYOUT_ASSET);
            }
        }
    }
}