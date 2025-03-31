#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

namespace SeikaGameKit.SceneManagement
{
    [CustomEditor(typeof(SceneMaster))]
    public class SceneMasterEditor : BaseEditor
    {
        #region DEFINITION
        private static readonly string PARENT_SCENE_HEADER = "シーンの親子設定";
        private static readonly string CHILD_SCENES_HEADER = "子シーンリスト";
        #endregion

        #region VARIABLE
        private SceneMaster _sceneMaster;
        private SerializedProperty _sceneRelations;
        private ReorderableList _parentSceneList;
        private Dictionary<int, ReorderableList> _childListCache = new Dictionary<int, ReorderableList>();
        #endregion

        #region UNITY_EVENT
        private void OnEnable()
        {
            _sceneMaster = (SceneMaster)target;
            _sceneRelations = serializedObject.FindProperty("_sceneRelations");
            _childListCache.Clear();
            _parentSceneList = CreateParentList(_sceneRelations);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            base.ScriptField();

            EditorGUILayout.Space();
            _parentSceneList.DoLayoutList();
            EditorGUILayout.Space();

            if (GUILayout.Button("登録シーンをビルド設定に追加", GUILayout.Width(180)))
            {
                AddToBuildSettings();
                EditorApplication.ExecuteMenuItem("File/Build Settings...");
            }

            EditorGUILayout.Space(20);
            base.RawInspector();
            serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region PRIVATE_METHOD
        // Creates a new SceneMaster asset
        [MenuItem("Assets/Create/Seika Game Kit/Scene Master")]
        private static void CreateSceneMasterAsset()
        {
            SceneMaster.CreateAsset();
        }

        // Applies changes and saves the asset
        private void UpdateAndSave()
        {
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(_sceneMaster);
            AssetDatabase.SaveAssetIfDirty(_sceneMaster);
        }

        // Draws a scene asset field in the inspector
        private void DrawSceneAsset(SerializedProperty target, SerializedProperty sibling, Rect rect)
        {
            string sceneGuid = target.FindPropertyRelative("sceneGuid").stringValue;
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            EditorGUI.BeginChangeCheck();
            rect.height = EditorGUIUtility.singleLineHeight + 4;
            SceneAsset newSceneAsset = EditorGUI.ObjectField(rect, sceneAsset, typeof(SceneAsset), false) as SceneAsset;

            if (EditorGUI.EndChangeCheck() && sceneAsset != newSceneAsset)
            {
                UpdateSceneConfig(target, sibling, newSceneAsset);
            }
        }

        // Updates scene configuration when a scene asset is changed
        private void UpdateSceneConfig(SerializedProperty target, SerializedProperty sibling, SceneAsset newScene)
        {
            string newPath = AssetDatabase.GetAssetPath(newScene);
            string newGuid = AssetDatabase.AssetPathToGUID(newPath);
            for (int i = 0; i < sibling.arraySize; i++)
            {
                var siblingID = sibling.GetArrayElementAtIndex(i).FindPropertyRelative("sceneGuid").stringValue;
                if (siblingID == newGuid)
                {
                    EditorUtility.DisplayDialog("警告", $"シーン '{newScene.name}' は既に登録されています。", "OK");
                    return;
                }
            }
            target.FindPropertyRelative("sceneGuid").stringValue = newGuid;
            UpdateAndSave();
        }

        // Adds a new empty scene element to a list
        private void AddSceneElement(SerializedProperty target)
        {
            target.InsertArrayElementAtIndex(target.arraySize);
            SerializedProperty newElement = target.GetArrayElementAtIndex(target.arraySize - 1);
            newElement.FindPropertyRelative("sceneGuid").stringValue = string.Empty;
            if (newElement.FindPropertyRelative("child") != null)
            {
                newElement.FindPropertyRelative("child").ClearArray();
            }
            UpdateAndSave();
        }

        // Removes a scene element from a list
        private void RemoveSceneElement(SerializedProperty target, ReorderableList list)
        {
            if (list.index >= 0 && list.index < target.arraySize)
            {
                target.DeleteArrayElementAtIndex(list.index);
                UpdateAndSave();
            }
        }

        #region PARENT_LIST
        /// <summary>
        /// Creates parent list
        /// </summary>
        private ReorderableList CreateParentList(SerializedProperty sceneRelations)
        {
            return new ReorderableList(serializedObject, sceneRelations)
            {
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, PARENT_SCENE_HEADER),
                elementHeightCallback = CalcParentHeight,
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    SerializedProperty parent = sceneRelations.GetArrayElementAtIndex(index);
                    Rect parentRect = new Rect(rect.x, rect.y + 2, rect.width, EditorGUIUtility.singleLineHeight + 4);
                    DrawSceneAsset(parent, sceneRelations, parentRect);
                    CreateChildList(parent.FindPropertyRelative("child"), parentRect, index);
                },
                drawFooterCallback = (Rect rect) =>
                {
                    rect.x = -rect.width + 100;
                    ReorderableList.defaultBehaviours.DrawFooter(rect, _parentSceneList);
                },
                onAddCallback = list => AddSceneElement(sceneRelations),
                onRemoveCallback = list => RemoveSceneElement(sceneRelations, list),
                onReorderCallback = list => UpdateAndSave(),
            };
        }

        /// <summary>
        /// Calculates the height of a parent scene element in the list
        /// </summary>
        private float CalcParentHeight(int index)
        {
            float baseHeight = EditorGUIUtility.singleLineHeight + 10;
            var parent = _sceneRelations.GetArrayElementAtIndex(index);
            var child = parent.FindPropertyRelative("child");

            if (_childListCache.TryGetValue(index, out ReorderableList childList))
            {
                return baseHeight + childList.GetHeight() + 10;
            }
            else
            {
                float headerHeight = EditorGUIUtility.singleLineHeight + 2;
                float elementsHeight = child.arraySize * (EditorGUIUtility.singleLineHeight + 6);
                float footerHeight = 16; // +,- Footer button height
                float additionalMargin = 15; // Margin
                return baseHeight + headerHeight + elementsHeight + footerHeight + additionalMargin;
            }
        }
        #endregion

        #region CHILD_LIST
        /// <summary>
        /// Creates or updates the list of child scenes for a parent
        /// </summary>
        private void CreateChildList(SerializedProperty child, Rect parentRect, int parentIndex)
        {
            if (!_childListCache.TryGetValue(parentIndex, out ReorderableList childList))
            {
                childList = new ReorderableList(serializedObject, child)
                {
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, CHILD_SCENES_HEADER),
                    elementHeightCallback = index => EditorGUIUtility.singleLineHeight + 6,
                    drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
                        DrawSceneAsset(child.GetArrayElementAtIndex(index), child, rect);
                    },
                    onAddCallback = list => AddSceneElement(child),
                    onRemoveCallback = list => RemoveSceneElement(child, list),
                    onReorderCallback = list => UpdateAndSave()
                };
                _childListCache[parentIndex] = childList;
            }

            Rect childRect = new Rect(parentRect);
            childRect.x += 20;
            childRect.width -= 20;
            childRect.y += parentRect.height + 8;
            childList.DoList(childRect);
        }
        #endregion

        // Add the scene to the build settings
        private void AddToBuildSettings()
        {
            var buildScenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            var buildScenePaths = new HashSet<string>(buildScenes.Select(s => s.path));
            bool hasChanges = false;

            var sceneGuids = _sceneMaster.GetAllScenes();
            foreach (var sceneGuid in sceneGuids)
            {
                var scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                if (!string.IsNullOrEmpty(scenePath))
                {
                    var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
                    if (sceneAsset != null && !buildScenePaths.Contains(scenePath))
                    {
                        buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                        hasChanges = true;
                    }
                }
            }

            if (hasChanges)
            {
                EditorBuildSettings.scenes = buildScenes.ToArray();
            }
        }

        #endregion
    }
}
#endif