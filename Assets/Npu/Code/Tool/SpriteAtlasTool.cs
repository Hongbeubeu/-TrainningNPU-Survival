using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using System.Linq;
#endif

namespace Npu.Helper
{

    [CreateAssetMenu]
    public class SpriteAtlasTool : ScriptableObject
    {
        public SpriteAtlas[] atlas;
    }


#if UNITY_EDITOR
    [CustomEditor(typeof(SpriteAtlasTool))]
    public class SpriteAtlasToolEditor : Editor
    {
        SpriteAtlasTool parent;
        ReorderableList list;
        SpriteAtlas selected;
        Sprite sprite;
        List<Sprite> duplicates;
        List<SpriteAtlas> foundAtlases;
        bool showAtlases = true;
        int atlasView;

        void OnEnable()
        {
            parent = target as SpriteAtlasTool;
            LoadAll();
        }

        void UpdateList()
        {
            list = null;
            if (parent.atlas != null)
            {
                list = new ReorderableList(parent.atlas, typeof(SpriteAtlas), false, false, false, false)
                {
                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        rect.x += 20;
                        EditorGUI.ObjectField(rect, list.list[index] as Object, typeof(SpriteAtlas), true);
                    },

                    onSelectCallback = (list) => { selected = list.list[list.index] as SpriteAtlas; },
                };
            }
        }

        void LoadAll()
        {
            var guids = AssetDatabase.FindAssets("t:SpriteAtlas");
            var atlases = new List<SpriteAtlas>();
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var q0 = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
                if (q0 != null) atlases.Add(q0);
            }

            parent.atlas = atlases.ToArray();
            UpdateList();
        }

        public override void OnInspectorGUI()
        {
            if ((showAtlases = EditorGUILayout.Foldout(showAtlases, "Atlases")) && list != null)
            {
                list.DoLayoutList();
            }

            EditorGUILayout.Space();
            if (GUILayout.Button("Find Duplicated Sprites"))
            {
                if (parent.atlas != null && parent.atlas.Length > 0)
                {
                    duplicates = parent.atlas.SelectMany(i => i.Sprites()).GroupBy(i => i).Where(i => i.Count() > 1)
                        .Select(i => i.Key).ToList();
                }
            }

            Action find = () =>
            {
                if (parent.atlas != null && parent.atlas.Length > 0 && sprite != null)
                {
                    selected = null;
                    foundAtlases = new List<SpriteAtlas>();
                    foreach (var i in parent.atlas)
                    {
                        if (i.Contains(sprite))
                        {
                            selected = i;
                            foundAtlases.Add(i);
                        }
                    }
                }
            };

            if (duplicates != null)
            {
                var l = new ReorderableList(duplicates, typeof(Sprite), false, true, false, false)
                {
                    drawHeaderCallback = (rect) =>
                    {
                        EditorGUI.LabelField(rect, string.Format("Duplicated Sprites ({0})", duplicates.Count));
                    },
                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        rect.width -= 60;
                        EditorGUI.ObjectField(rect, duplicates[index], typeof(Object), true);

                        rect.x += rect.width + 5;
                        rect.width = 55;
                        if (GUI.Button(rect, "Find"))
                        {
                            sprite = duplicates[index];
                            find();
                        }
                    },
                };

                l.DoLayoutList();

            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            {
                sprite = EditorGUILayout.ObjectField(sprite, typeof(Sprite), false) as Sprite;
                if (GUILayout.Button("Find"))
                {
                    find();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (foundAtlases != null)
            {
                var l = new ReorderableList(foundAtlases, typeof(SpriteAtlas), false, true, false, false)
                {
                    drawHeaderCallback = (rect) =>
                    {
                        EditorGUI.LabelField(rect,
                            string.Format("Found '{0}' in {1} atlas(es)", sprite.name, foundAtlases.Count));
                    },
                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        rect.x += 20;
                        EditorGUI.ObjectField(rect, foundAtlases[index], typeof(SpriteAtlas), true);
                    },
                    onSelectCallback = (list) => { selected = foundAtlases[list.index]; }
                };

                l.DoLayoutList();
            }

            if (selected != null)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(string.Format("{0}", selected.name), EditorStyles.boldLabel);

                atlasView = GUILayout.Toolbar(atlasView, new string[] {"Normal View", "Debug View"});
                if (atlasView == 0)
                {
                    var e = CreateEditor(selected);
                    e.OnInspectorGUI();
                }
                else
                {
                    var so = new SerializedObject(selected);

                    var p = so.FindProperty("m_EditorData").FindPropertyRelative("packables");
                    EditorGUILayout.PropertyField(p, true);

                    p = so.FindProperty("m_PackedSprites");
                    EditorGUILayout.PropertyField(p, true);
                }

            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (selected != null)
            {
                var e = CreateEditor(selected);
                e.OnPreviewGUI(r, background);
            }

        }

        public override bool HasPreviewGUI()
        {
            return selected != null;
        }
    }

    public static class AtlasExtensions
    {
        public static List<Sprite> Sprites(this SpriteAtlas atlas)
        {
            var so = new SerializedObject(atlas);
            var p = so.FindProperty("m_PackedSprites");
            var sprites = new List<Sprite>();
            for (var i = 0; i < p.arraySize; i++)
            {
                sprites.Add(p.GetArrayElementAtIndex(i).objectReferenceValue as Sprite);
            }

            return sprites;
        }

        public static bool Contains(this SpriteAtlas atlas, Sprite sprite)
        {
            return atlas.Sprites().Contains(sprite);
        }
    }
#endif

}