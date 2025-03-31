using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeikaGameKit.SceneManagement
{
    /// <summary>
    /// Handle initialization of scenes in build mode
    /// </summary>
    public static class SceneInitializer
    {
        private static bool _isInitialized = false;

        /// <summary>
        /// Automatically loads child scenes for the start scene after the scene is loaded
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeStartScene()
        {
            // Preventing editors and duplicate executions
            if (Application.isEditor || _isInitialized)
            {
                return;
            }

            string startSceneName = SceneManager.GetActiveScene().name;
            var childScenes = SceneMaster.Instance?.GetChildScenesByName(startSceneName) ?? new List<SceneMaster.SceneConfig>();
            if (childScenes.Count == 0)
            {
                _isInitialized = true;
                return;
            }

            HashSet<string> loadedScenes = new HashSet<string>();
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                loadedScenes.Add(scene.name);
            }

            foreach (var childScene in childScenes)
            {
                if (!loadedScenes.Contains(childScene.sceneName))
                {
                    try
                    {
                        SceneManager.LoadScene(childScene.sceneName, LoadSceneMode.Additive);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError($"[SceneInitializer] '{childScene.sceneName}' のロードに失敗: {e.Message}");
                    }
                }
            }

            _isInitialized = true;
        }
    }
}