using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SeikaGameKit.SceneManagement
{
    /// <summary>
    /// Load related scenes
    /// </summary>
    public static class SceneLoader
    {
        #region PUBLIC_METHOD
        /// <summary>
        /// Load a parent-scene (Single mode) with child-scenes (Additive mode)
        /// </summary>
        /// <example>
        /// SceneLoader.LoadWithChildScenes(sceneName);
        /// </example>
        public static void LoadWithChildScenes(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            var childScenes = GetChildScenesByName(sceneName);

            foreach (var child in childScenes)
            {
                try
                {
                    SceneManager.LoadScene(child.sceneName, LoadSceneMode.Additive);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"子シーンのロードに失敗しました: {e.Message}");
                    continue;
                }
            }
        }

        /// <summary>
        /// Load a parent-scene (Single mode) with child-scenes (Additive mode)
        /// </summary>
        /// <example>
        /// using System.Threading.Tasks;
        /// await SceneLoader.LoadWithChildScenesAsync(sceneName);
        /// </example>
        public static async Task LoadWithChildScenesAsync(string sceneName)
        {
            var parentOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            await ConvertToTask(parentOp);
            var childScenes = GetChildScenesByName(sceneName);

            foreach (var child in childScenes)
            {
                try
                {
                    var childOp = SceneManager.LoadSceneAsync(child.sceneName, LoadSceneMode.Additive);
                    await ConvertToTask(childOp);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"子シーンのロードに失敗しました: {e.Message}");
                    continue;
                }
            }
        }

        /// <summary>
        /// Unload all child-scenes
        /// </summary>
        /// <example>
        /// SceneLoader.UnloadAllChildScenes();
        /// </example>
        public static void UnloadAllChildScenes()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            var childScenes = GetChildScenesByName(currentScene);

            foreach (var child in childScenes)
            {
                var scene = SceneManager.GetSceneByName(child.sceneName);
                if (scene.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        /// <summary>
        /// Unload all child-scenes asynchronously
        /// </summary>
        /// <example>
        /// using System.Threading.Tasks;
        /// await SceneLoader.UnloadAllChildScenesAsync();
        /// </example>
        public static async Task UnloadAllChildScenesAsync()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            var childScenes = GetChildScenesByName(currentScene);
            List<Task> tasks = new List<Task>();

            foreach (var child in childScenes)
            {
                var scene = SceneManager.GetSceneByName(child.sceneName);
                if (scene.isLoaded)
                {
                    var childOp = SceneManager.UnloadSceneAsync(scene);
                    tasks.Add(ConvertToTask(childOp));
                }
            }

            await Task.WhenAll(tasks);
        }
        #endregion

        #region PRIVATE_METHOD
        /// <summary>
        /// Get child-scenes by parent-scene name
        /// </summary>
        private static List<SceneMaster.SceneConfig> GetChildScenesByName(string sceneName)
        {
            return SceneMaster.Instance?.GetChildScenesByName(sceneName) ?? new List<SceneMaster.SceneConfig>();
        }

        /// <summary>
        /// Convert asynchronous operation to Task
        /// </summary>
        private static Task ConvertToTask(AsyncOperation operation)
        {
            var tcs = new TaskCompletionSource<bool>();
            operation.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
        #endregion
    }
}