#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SeikaGameKit
{
    /// <summary>
    /// プロジェクト共通で使用する定型フォルダを作成
    /// </summary>
    public class ProjectFolderCreator
    {
        private static readonly string[] projectFolders = new string[]
        {
            "Assets/_Project",
            "Assets/_Project/Audios",
            "Assets/_Project/Effects",
            "Assets/_Project/Fonts",
            "Assets/_Project/Misc",
            "Assets/_Project/Models/Characters",
            "Assets/_Project/Models/Environments",
            "Assets/_Project/Models/Props",
            "Assets/_Project/Prefabs",
            "Assets/_Project/Scenes",
            "Assets/_Project/Scripts",
            "Assets/_Project/Shaders",
            "Assets/_Project/Sprites",
            "Assets/_Project/Timelines",
            "Assets/Resources",
        };

        [MenuItem("Seika Game Kit/Create Project Folders")]
        public static void CreateProjectFolders()
        {
            foreach (string folderPath in projectFolders)
            {
                if (!AssetDatabase.IsValidFolder(folderPath))
                {
                    CreateFolder(folderPath);
                }

                string gitkeepPath = Path.Combine(folderPath, ".gitkeep");
                string fullGitkeepPath = Path.Combine(Application.dataPath, "..", gitkeepPath);

                if (!File.Exists(fullGitkeepPath))
                {
                    File.WriteAllText(fullGitkeepPath, string.Empty);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("プロジェクトフォルダの作成が完了しました");
        }

        private static void CreateFolder(string folderPath)
        {
            string[] pathParts = folderPath.Split('/');
            string currentPath = pathParts[0];

            for (int i = 1; i < pathParts.Length; i++)
            {
                string parentPath = currentPath;
                currentPath = Path.Combine(currentPath, pathParts[i]);

                if (!AssetDatabase.IsValidFolder(currentPath))
                {
                    string guid = AssetDatabase.CreateFolder(parentPath, pathParts[i]);
                    if (string.IsNullOrEmpty(guid))
                    {
                        Debug.LogError($"フォルダの作成に失敗: {currentPath}");
                    }
                }
            }
        }
    }
}
#endif