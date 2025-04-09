using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SeikaGameKit.SceneManagement
{
    /// <summary>
    /// Manages parent-child relationships between scenes for hierarchical scene loading
    /// </summary>
    public class SceneMaster : ScriptableObject
    {
        #region DEFINITIONS
        [Serializable]
        public class SceneConfig
        {
            public string sceneGuid;
            public string sceneName;
        }

        [Serializable]
        public class SceneRelation : SceneConfig
        {
            public List<SceneConfig> child = new List<SceneConfig>();
        }
        #endregion

        #region VARIABLE
        [SerializeField] private List<SceneRelation> _sceneRelations = new List<SceneRelation>();
        private static SceneMaster _instance;
        #endregion

        #region PUBLIC_METHOD
        /// <summary>
        /// Singleton instance of SceneMaster
        /// </summary>
        public static SceneMaster Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<SceneMaster>("SceneMaster");
                }
                return _instance;
            }
        }

        /// <summary>
        /// Check if the scene is registered as a parent scene
        /// </summary>
        public bool IsParentScene(string sceneGuid)
        {
            foreach (var relation in _sceneRelations)
            {
                if (relation.sceneGuid == sceneGuid)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get child-scenes by parent-scene GUID
        /// </summary>
        public List<SceneConfig> GetChildScenesByGuid(string parentSceneGuid)
        {
            foreach (var relation in _sceneRelations)
            {
                if (relation.sceneGuid == parentSceneGuid)
                {
                    return new List<SceneConfig>(relation.child);
                }
            }
            return new List<SceneConfig>();
        }

        /// <summary>
        /// Get child-scenes by parent-scene name
        /// </summary>
        public List<SceneConfig> GetChildScenesByName(string parentSceneName)
        {
            foreach (var relation in _sceneRelations)
            {
                if (relation.sceneName == parentSceneName)
                {
                    return new List<SceneConfig>(relation.child);
                }
            }
            return new List<SceneConfig>();
        }

        /// <summary>
        /// Get all registered scenes
        /// </summary>
        public List<string> GetAllScenes()
        {
            HashSet<string> list = new HashSet<string>();

            foreach (var relation in _sceneRelations)
            {
                if (!string.IsNullOrEmpty(relation.sceneGuid))
                {
                    list.Add(relation.sceneGuid);
                }
                foreach (var child in relation.child)
                {
                    if (!string.IsNullOrEmpty(child.sceneGuid))
                    {
                        list.Add(child.sceneGuid);
                    }
                }
            }

            return new List<string>(list);
        }
        #endregion

#if UNITY_EDITOR
        /// <summary>
        /// Creates a new SceneMaster asset in the Resources folder
        /// </summary>
        public static void CreateAsset()
        {
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:SceneMaster");
            if (guids.Length > 0)
            {
                UnityEditor.EditorUtility.DisplayDialog("警告", $"SceneMaster は既に存在しています。", "OK");
                return;
            }

            if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");
            }

            SceneMaster asset = ScriptableObject.CreateInstance<SceneMaster>();
            UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/Resources/SceneMaster.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = asset;
        }

        /// <summary>
        /// Convert GUIDs to scene names for all relations and child scenes
        /// </summary>
        public void ConvertSceneGuidsToNames()
        {
            foreach (var relation in _sceneRelations)
            {
                relation.sceneName = ConvertGuidToName(relation.sceneGuid);
                foreach (var childScene in relation.child)
                {
                    childScene.sceneName = ConvertGuidToName(childScene.sceneGuid);
                }
            }

            string ConvertGuidToName(string guid)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetExtension(path).ToLower() != ".unity")
                {
                    return string.Empty;
                }
                return Path.GetFileNameWithoutExtension(path);
            }
        }
#endif
    }
}