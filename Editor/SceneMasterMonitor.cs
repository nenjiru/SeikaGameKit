#if UNITY_EDITOR
using System.IO;
using UnityEditor;

namespace SeikaGameKit.SceneManagement
{
    /// <summary>
    /// Update SceneMaster when scene assets are changed
    /// </summary>
    public class SceneMasterMonitor : AssetPostprocessor
    {
        private static SceneMaster _sceneMaster;
        private static SerializedObject _serializedObject;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.quitting += () =>
            {
                _serializedObject = null;
                _sceneMaster = null;
            };
        }

        // Called when assets are imported, deleted, or moved
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            SerializedObject sceneMaster = GetSerializedSceneMaster();
            if (sceneMaster == null)
            {
                return;
            }

            SerializedProperty sceneRelations = sceneMaster.FindProperty("_sceneRelations");
            if (sceneRelations == null)
            {
                return;
            }

            foreach (string path in deletedAssets)
            {
                if (IsSceneAsset(path))
                {
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    bool hasChanges = RemoveParentByGuid(sceneRelations, guid);
                    bool childChanges = RemoveChildByGuid(sceneRelations, guid);

                    if (hasChanges || childChanges)
                    {
                        sceneMaster.ApplyModifiedProperties();
                    }
                }
            }
        }

        // Determines if the path is a scene asset
        private static bool IsSceneAsset(string path)
        {
            return Path.GetExtension(path).ToLower() == ".unity";
        }

        // Removes a SceneRelation with the GUID from the parent list
        private static bool RemoveParentByGuid(SerializedProperty sceneRelations, string guid)
        {
            return RemoveElementByGuid(sceneRelations, guid);
        }

        // Removes a SceneRelation with the GUID from the child list
        private static bool RemoveChildByGuid(SerializedProperty sceneRelations, string guid)
        {
            if (sceneRelations == null || string.IsNullOrEmpty(guid))
            {
                return false;
            }

            bool hasChanges = false;

            for (int i = 0; i < sceneRelations.arraySize; i++)
            {
                SerializedProperty parent = sceneRelations.GetArrayElementAtIndex(i);
                SerializedProperty childList = parent.FindPropertyRelative("child");

                if (childList != null)
                {
                    bool childChanged = RemoveElementByGuid(childList, guid);
                    if (childChanged)
                    {
                        hasChanges = true;
                    }
                }
            }

            return hasChanges;
        }

        // Removes elements with the GUID from a SerializedProperty array
        private static bool RemoveElementByGuid(SerializedProperty sceneList, string guid)
        {
            if (sceneList == null || string.IsNullOrEmpty(guid))
                return false;

            bool hasChanges = false;

            for (int i = sceneList.arraySize - 1; i >= 0; i--)
            {
                var scene = sceneList.GetArrayElementAtIndex(i).FindPropertyRelative("sceneGuid");
                if (scene != null && scene.stringValue == guid)
                {
                    sceneList.DeleteArrayElementAtIndex(i);
                    hasChanges = true;
                }
            }

            return hasChanges;
        }

        // Gets the SerializedObject of SceneMaster
        private static SerializedObject GetSerializedSceneMaster()
        {
            if (_sceneMaster == null)
            {
                try
                {
                    _sceneMaster = SceneMaster.Instance;
                    if (_sceneMaster == null)
                    {
                        return null;
                    }
                }
                catch (System.Exception)
                {
                    return null;
                }
            }

            try
            {
                if (_serializedObject == null || _serializedObject.targetObject == null)
                {
                    _serializedObject = new SerializedObject(_sceneMaster);
                }
                else
                {
                    _serializedObject.Update();
                }
                return _serializedObject;
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}
#endif