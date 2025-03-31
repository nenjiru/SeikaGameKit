#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace SeikaGameKit.SceneManagement
{
    /// <summary>
    /// Loads child-scenes when a scene is opened in the editor
    /// </summary>
    [InitializeOnLoad]
    public class EditorSceneLoader
    {
        #region VARIABLE
        private static HashSet<Scene> _lockedScenes = new HashSet<Scene>();
        #endregion

        static EditorSceneLoader()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
            {
                // Only editor mode
                LoadAndConfigureScenes(SceneManager.GetActiveScene());
            }

            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.sceneClosing += OnSceneClosing;
        }

        #region PRIVATE_METHOD
        /// <summary>
        /// Update scene name just before play mode
        /// </summary>
        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
            {
                SceneMaster.Instance?.ConvertSceneGuidsToNames();
            }
        }

        /// <summary>
        /// Scene is opened
        /// </summary>
        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            if (IsParentScene(scene))
            {
                LoadAndConfigureScenes(scene);
            }
        }

        /// <summary>
        /// Scene is closing
        /// </summary>
        private static void OnSceneClosing(Scene scene, bool removingScene)
        {
            if (_lockedScenes.Contains(scene))
            {
                UnlockScene(scene);
            }
        }

        /// <summary>
        /// Scenes loading and locking
        /// </summary>
        private static void LoadAndConfigureScenes(Scene parentScene)
        {
            List<string> childScenePaths = GetChildScenes(parentScene);
            if (childScenePaths.Count == 0)
            {
                return;
            }

            _lockedScenes.Clear();

            foreach (string path in childScenePaths)
            {
                if (!IsSceneLoaded(path))
                {
                    try
                    {
                        Scene child = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
                        LockScene(child);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"シーンのロードに失敗しました: {path}\n{e.Message}");
                        continue;
                    }
                }
                else
                {
                    Scene child = EditorSceneManager.GetSceneByPath(path);
                    LockScene(child);
                }
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        /// <summary>
        /// Scene is already loaded
        /// </summary>
        private static bool IsSceneLoaded(string scenePath)
        {
            Scene scene = EditorSceneManager.GetSceneByPath(scenePath);
            return scene.IsValid() && scene.isLoaded;
        }

        /// <summary>
        /// Determine if the scene is parental
        /// </summary>
        private static bool IsParentScene(Scene scene)
        {
            string sceneGuid = AssetDatabase.AssetPathToGUID(scene.path);
            return SceneMaster.Instance?.IsParentScene(sceneGuid) ?? false;
        }

        /// <summary>
        /// Get child-scenes by parent scene
        /// </summary>
        private static List<string> GetChildScenes(Scene parentScene)
        {
            List<string> childScenePaths = new List<string>();
            string parentGuid = AssetDatabase.AssetPathToGUID(parentScene.path);
            var childScenes = SceneMaster.Instance?.GetChildScenesByGuid(parentGuid) ?? new List<SceneMaster.SceneConfig>();

            foreach (var childScene in childScenes)
            {
                string path = AssetDatabase.GUIDToAssetPath(childScene.sceneGuid);
                if (!string.IsNullOrEmpty(path))
                {
                    childScenePaths.Add(path);
                }
            }

            return childScenePaths;
        }

        /// <summary>
        /// Unlock scene
        /// </summary>
        private static void UnlockScene(Scene scene)
        {
            SceneVisibilityManager.instance.EnablePicking(scene.GetRootGameObjects(), true);
            _lockedScenes.Remove(scene);
        }

        /// <summary>
        /// Lock scene
        /// </summary>
        private static void LockScene(Scene scene)
        {
            SceneVisibilityManager.instance.DisablePicking(scene.GetRootGameObjects(), true);
            _lockedScenes.Add(scene);
        }
        #endregion
    }
}
#endif