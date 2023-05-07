using UnityEngine;
using UnityEditor;

namespace SeikaGameKit
{
    public class CustomLayout : MonoBehaviour
    {
        private const string LAYOUT_PATH = "Packages/SeikaGameKit/Resources/Layouts/SGKitLayout.wlt";

        [MenuItem("Seika Game Kit/Load Custom Layout")]
        static void LoadCustomLayout()
        {
            if (System.IO.File.Exists(LAYOUT_PATH))
            {
                EditorUtility.LoadWindowLayout(LAYOUT_PATH);
            }
            else
            {
                Debug.LogWarning("Layout not loaded. Layout file missing at: " + LAYOUT_PATH);
            }
        }
    }
}