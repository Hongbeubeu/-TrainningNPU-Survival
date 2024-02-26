#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;
using Npu.Helper;
using Object = UnityEngine.Object;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.EditorSupport
{
    public class SceneOrganizerWindow : EditorWindow
    {
        public static void Open()
        {
            var window = GetWindow<SceneOrganizerWindow>("Scene Organizer", true);
            window.Show();
        }

        #region CONFIGS

        Vector2 selectionScroll;
        Vector2 orgScroll;
        List<ASub> subs = new List<ASub>();

        #endregion

        private void OnEnable()
        {
            subs.Add(new Basics());
            subs.Add(new Sorts());
            subs.Add(new Distributes());
            subs.Add(new Matches());
            subs.Add(new Rnds());
            // subs.Add(new Spherize());

            RefreshSelected();
        }

        void OnFocus()
        {
            RefreshSelected();
        }

        void OnSelectionChange()
        {
            RefreshSelected();
            Repaint();
        }

        void RefreshSelected()
        {
            SelectedGameObjects.Clear();
            SelectedGameObjects.AddRange(InstantSelectedGameObjects);
        }

        private void OnGUI()
        {
            //RefreshSelected();

            EditorGUIUtils.SectionHeader(titleContent.text);

            using (new HorizontalLayout())
            {
                DrawSelectionsSection();
                using (new LabelWidth(EditorGUIUtility.labelWidth * 0.75f))
                {
                    DrawHelpsSection();
                }
                
            }
        }

        void DrawSelectionsSection()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(140), GUILayout.ExpandHeight(true));
            GUILayout.Label($"Selection ({SelectedGameObjects.Count()})", EditorStyles.boldLabel);
            selectionScroll = EditorGUILayout.BeginScrollView(selectionScroll);
            foreach (var go in SelectedGameObjects)
            {
                EditorGUILayout.ObjectField(go, typeof(GameObject), true);
                if (IsActive(go))
                {
                    var rect = GUILayoutUtility.GetLastRect();
                    EditorGUI.DrawRect(rect, Color.green.SetAlpha(0.5f));
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        void DrawHelpsSection()
        {
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            orgScroll = EditorGUILayout.BeginScrollView(orgScroll, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            foreach (var sub in subs) sub.OnGUI();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        #region Static helpers

        public static List<GameObject> SelectedGameObjects = new List<GameObject>();

        public static bool NoObjects => (SelectedGameObjects?.Count ?? 0) <= 0;
        public static bool TooFewObjects => (SelectedGameObjects?.Count ?? 0) < 2;

        public static GameObject ActiveGameObject => Selection.activeGameObject;
        public static IEnumerable<GameObject> InstantSelectedGameObjects => (Selection.gameObjects?.Where(go => go.scene.IsValid()) ?? Enumerable.Empty<GameObject>()).OrderBy(go => go.transform.GetSiblingIndex());

        public static bool IsActive(GameObject go)
        {
            return go == ActiveGameObject;
        }

        public static void Edit(Object t) => t.DirtyWithUndo();

        #endregion
    }
}
#endif