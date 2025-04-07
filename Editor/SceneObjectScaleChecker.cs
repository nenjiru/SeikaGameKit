#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace SeikaGameKit
{
    /// <summary>
    /// Highlight GameObjects that are not scaled uniformly in the hierarchy window
    /// </summary>
    public class SceneObjectScaleChecker
    {
        private static HashSet<int> _invalidScaleObjects = new HashSet<int>();
        private static bool _isCheckingScales = false;

        [MenuItem("GameObject/Seika Game Kit/Scale Checker", false)]
        public static void ScaleChecker()
        {
            GameObject selectedObject = Selection.activeGameObject;
            if (selectedObject == null)
            {
                return;
            }

            ExpandHierarchyTree(selectedObject);
            _invalidScaleObjects.Clear();
            bool result = CheckScaleRecursively(selectedObject);

            if (result)
            {
                Debug.Log($"{selectedObject.name} のスケールは正常です");
                DisableScaleChecker();
            }
            else
            {
                List<string> invalidObjectNames = new List<string>();
                foreach (int i in _invalidScaleObjects)
                {
                    invalidObjectNames.Add(EditorUtility.InstanceIDToObject(i).name);
                }
                Debug.LogWarning($"{string.Join(", ", invalidObjectNames)} のスケールが 1 ではありません");
                EnableScaleChecker();
            }
        }

        [MenuItem("GameObject/Seika Game Kit/Scale Checker (Clear Highlights)", false)]
        private static void ClearScaleChecker()
        {
            DisableScaleChecker();
        }

        private static void EnableScaleChecker()
        {
            if (!_isCheckingScales)
            {
                EditorApplication.hierarchyWindowItemOnGUI += HighlightInvalidScaleObjects;
                EditorApplication.hierarchyChanged += UpdateInvalidScaleObjectsList;
                _isCheckingScales = true;
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        private static void DisableScaleChecker()
        {
            if (_isCheckingScales)
            {
                EditorApplication.hierarchyWindowItemOnGUI -= HighlightInvalidScaleObjects;
                EditorApplication.hierarchyChanged -= UpdateInvalidScaleObjectsList;
                _isCheckingScales = false;
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        // Recursively checks for uniform object scale
        private static bool CheckScaleRecursively(GameObject gameObject)
        {
            bool isScaleOne = gameObject.transform.localScale == Vector3.one;
            if (!isScaleOne)
            {
                _invalidScaleObjects.Add(gameObject.GetInstanceID());
            }

            // Check child objects recursively
            bool childrenScalesAreOne = true;
            foreach (Transform child in gameObject.transform)
            {
                bool childScaleIsOne = CheckScaleRecursively(child.gameObject);
                if (!childScaleIsOne)
                {
                    childrenScalesAreOne = false;
                }
            }

            return isScaleOne && childrenScalesAreOne;
        }

        // Removes objects with corrected scale from the invalid objects list.
        private static void UpdateInvalidScaleObjectsList()
        {
            if (_invalidScaleObjects.Count == 0)
            {
                DisableScaleChecker();
                return;
            }

            var objectsToCheck = new List<int>(_invalidScaleObjects);
            bool anyRemoved = false;

            foreach (int instanceID in objectsToCheck)
            {
                GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                if (go == null)
                {
                    _invalidScaleObjects.Remove(instanceID);
                    anyRemoved = true;
                    continue;
                }

                if (go.transform.localScale == Vector3.one)
                {
                    _invalidScaleObjects.Remove(instanceID);
                    anyRemoved = true;
                }
            }

            if (anyRemoved)
            {
                EditorApplication.RepaintHierarchyWindow();
                if (_invalidScaleObjects.Count == 0)
                {
                    DisableScaleChecker();
                }
            }
        }

        // Highlighting in the Hierarchy Window
        private static void HighlightInvalidScaleObjects(int instanceID, Rect selectionRect)
        {
            if (_invalidScaleObjects.Contains(instanceID))
            {
                GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
                if (obj == null)
                {
                    _invalidScaleObjects.Remove(instanceID);
                    return;
                }

                Rect iconRect = new Rect(selectionRect);
                iconRect.x = selectionRect.xMax - 20;
                iconRect.width = 20;
                GUI.Label(iconRect, EditorGUIUtility.IconContent("console.warnicon.sml"));
                // EditorGUI.DrawRect(selectionRect, new Color(.5f, 0f, 0f, 0.3f));
            }
        }

        private static void ExpandHierarchyTree(GameObject gameObject)
        {
            try
            {
                var type = typeof(EditorWindow).Assembly.GetType("UnityEditor.SceneHierarchyWindow");
                if (type == null)
                {
                    return;
                }
                foreach (var window in Resources.FindObjectsOfTypeAll(type))
                {
                    var method = type.GetMethod("SetExpandedRecursive");
                    method?.Invoke(window, new object[] { gameObject.GetInstanceID(), true });
                }
            }
            catch (System.Exception)
            {
            }
        }

        // Disable highlighting when exiting editor or compiling
        [InitializeOnLoad]
        private class ScaleCheckerCleanup
        {
            static ScaleCheckerCleanup()
            {
                EditorApplication.quitting += CleanupScaleChecker;
                AssemblyReloadEvents.beforeAssemblyReload += CleanupScaleChecker;
            }

            private static void CleanupScaleChecker()
            {
                if (_isCheckingScales)
                {
                    EditorApplication.hierarchyWindowItemOnGUI -= HighlightInvalidScaleObjects;
                    EditorApplication.hierarchyChanged -= UpdateInvalidScaleObjectsList;
                    _isCheckingScales = false;
                }
            }
        }
    }
}
#endif