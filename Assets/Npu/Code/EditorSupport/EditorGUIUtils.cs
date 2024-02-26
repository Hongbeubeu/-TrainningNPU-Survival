using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Npu.Helper;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace Npu.EditorSupport
{
    public static class EditorGUIUtils
    {
        #region Styles
        
        private static GUIStyle _imagePreviewStyle;
        public static GUIStyle ImagePreviewStyle => _imagePreviewStyle ??= new GUIStyle(EditorStyles.miniButton)
        {
            imagePosition = ImagePosition.ImageAbove,
            margin = new RectOffset(), fixedHeight = 0
        };

        private static GUIStyle _righLabelStyle;
        public static GUIStyle RighLabelStyle => _righLabelStyle ??= new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleRight
        };
        
        private static GUIStyle _centerLabelStyle;
        public static GUIStyle CenterLabelStyle => _centerLabelStyle ??= new GUIStyle(EditorStyles.label)
        {
            alignment = TextAnchor.MiddleCenter
        };
        
        private static GUIStyle _boldLabelStyle;
        public static GUIStyle BoldLabelStyle => _boldLabelStyle ??= new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Bold,
        };

        private static GUIStyle _italicLabelStyle;
        public static GUIStyle ItalicLabelStyle => _italicLabelStyle ??= new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.Italic,
        };
        
        private static GUIStyle _boldItalicLabelStyle;
        public static GUIStyle BoldItalicLabelStyle => _boldItalicLabelStyle ??= new GUIStyle(EditorStyles.label)
        {
            fontStyle = FontStyle.BoldAndItalic,
        };
        
        private static GUIStyle _richTextLabelStyle;
        public static GUIStyle RichTextLabelStyle => _richTextLabelStyle ??= new GUIStyle(EditorStyles.label)
        {
            richText = true,
        };

        #endregion  
        public static void Header(string name)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(name, EditorStyles.boldLabel);
        }

        public static void HorizontalSeparator()
        {
            var styleHr = new GUIStyle(GUI.skin.box) {stretchWidth = true, fixedHeight = 2};
            GUILayout.Box("", styleHr);
        }

        public static string DirectorySelector(string label, string path)
        {
            using (new HorizontalLayout())
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(100));
                using (new GuiColor(Directory.Exists(EditorUtils.AbsolutePath(path)) ? Color.white : Color.yellow))
                {
                    path = EditorGUILayout.TextField(GUIContent.none, path);
                }

                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    var chosen = EditorUtility.OpenFolderPanel("Select Folder", path, "");
                    if (string.IsNullOrEmpty(chosen)) return path;

                    path = EditorUtils.RelativePath(chosen);
                }
            }

            return path;
        }

        public static string FileSelector(string label, string path)
        {
            using (new HorizontalLayout())
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(100));
                using (new GuiColor(File.Exists(EditorUtils.AbsolutePath(path)) ? Color.white : Color.yellow))
                {
                    path = EditorGUILayout.TextField(GUIContent.none, path);
                }

                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    var chosen = EditorUtility.OpenFilePanel("Select File", path, "");
                    if (string.IsNullOrEmpty(chosen)) return path;

                    path = EditorUtils.RelativePath(chosen);
                }
            }

            return path;
        }

        public static int DrawBitMaskField(Rect aPosition, int aMask, Type aType, GUIContent aLabel)
        {
            var itemNames = Enum.GetNames(aType);
            var itemValues = Enum.GetValues(aType) as int[];

            var val = aMask;
            var maskVal = 0;
            for (var i = 0; i < itemValues.Length; i++)
            {
                if (itemValues[i] != 0)
                {
                    if ((val & itemValues[i]) == itemValues[i])
                        maskVal |= 1 << i;
                }
                else if (val == 0)
                    maskVal |= 1 << i;
            }

            var newMaskVal = EditorGUI.MaskField(aPosition, aLabel, maskVal, itemNames);
            var changes = maskVal ^ newMaskVal;

            for (var i = 0; i < itemValues.Length; i++)
            {
                if ((changes & (1 << i)) != 0) // has this list item changed?
                {
                    if ((newMaskVal & (1 << i)) != 0) // has it been set?
                    {
                        if (itemValues[i] == 0) // special case: if "0" is set, just set the val to 0
                        {
                            val = 0;
                            break;
                        }
                        else
                            val |= itemValues[i];
                    }
                    else // it has been reset
                    {
                        val &= ~itemValues[i];
                    }
                }
            }

            return val;
        }

        public static bool SmallControl(string label, float width = 20, float height = 20)
        {
            return GUILayout.Button(label, GUILayout.Width(width), GUILayout.Height(height));
        }

        public static bool SmallControl(GUIContent label, float width = 20, float height = 20)
        {
            return GUILayout.Button(label, GUILayout.Width(width), GUILayout.Height(height));
        }

        public static bool CenterButton(string text, params GUILayoutOption[] options)
        {
            var btnTxt = new GUIContent(text);
            var rt = GUILayoutUtility.GetRect(btnTxt, GUI.skin.button, options);
            rt.center = new Vector2(EditorGUIUtility.currentViewWidth / 2, rt.center.y);
            return GUI.Button(rt, btnTxt, GUI.skin.button);
        }

        public static void RichLabel(string text, 
            int sizeDelta = 0, 
            FontStyle fontStyle = FontStyle.Normal, 
            Color? color = null,
            TextAnchor alignment = TextAnchor.MiddleCenter,
            params GUILayoutOption[] options)
        {
            var style = new GUIStyle(GUI.skin.label)
            {
                alignment = alignment,
                fontSize = GUI.skin.label.fontSize + sizeDelta,
                fontStyle = fontStyle,
                normal = {textColor = color ?? GUI.skin.label.normal.textColor}
            };
            EditorGUILayout.LabelField(text, style, options);
        }

        public static void Box(Rect rect)
        {
            var style = new GUIStyle(EditorStyles.helpBox);
            GUI.Box(rect, GUIContent.none, style);
        }

        public static bool SectionHeader(string text, TextAnchor alignment = TextAnchor.MiddleLeft, int sizeDelta = 3,
            params GUILayoutOption[] options)
        {
            var content = new GUIContent(text);
            
            var color = ColorUtils.niceBlue;
            var style = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold, alignment = alignment,
                fontSize = GUI.skin.label.fontSize + sizeDelta,
                hover = {textColor = color},
                onActive = {textColor = color},
                active = {textColor = color},
                padding = new RectOffset(3, 3, 3, 3)
            };
            
            var rect = GUILayoutUtility.GetRect(content, style, options);
            
            return GUI.Button(rect, content, style);
        }
        
        public static bool SectionFoldout(string text, bool foldout, TextAnchor alignment = TextAnchor.MiddleLeft, int sizeDelta = 3,
            params GUILayoutOption[] options)
        {
            var content = new GUIContent(text);
            
            var color = ColorUtils.niceBlue;
            var margin = EditorStyles.foldout.margin;
            var style = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = foldout ? FontStyle.Bold : FontStyle.BoldAndItalic,
                alignment = alignment,
                fontSize = EditorStyles.foldout.fontSize + sizeDelta,
                normal = {textColor = color},
                onNormal = {textColor = color},
                margin = new RectOffset(margin.left + 8, margin.right, margin.top, margin.bottom)
            };
            
            var rect = GUILayoutUtility.GetRect(content, style, options);
            return EditorGUI.Foldout(rect, foldout, content, true, style);
        }

        public static void Section(string label, Action onGUI, TextAnchor alignment = TextAnchor.MiddleCenter,
            string keyPrefix = "", bool helpBox = true)
        {
            var key = $"{keyPrefix}_{label}";
            var expanded = EditorPrefs.GetBool(key, true);

            using (new VerticalHelpBox())
            {
                var foldout = SectionFoldout(label, expanded, alignment);
                if (foldout != expanded)
                {
                    expanded = foldout;
                    EditorPrefs.SetBool(key, foldout);
                }

                if (!expanded) return;

                if (helpBox)
                {
                    using (new VerticalHelpBox())
                    {
                        onGUI?.Invoke();
                    }
                }
                else onGUI?.Invoke();
            }
        }

        public static Texture2D CompactTextureField(string name, Texture2D texture)
        {
            GUILayout.BeginVertical();
            var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.UpperCenter, fixedWidth = 70};
            GUILayout.Label(name, style);
            var result = (Texture2D) EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70),
                GUILayout.Height(70));
            GUILayout.EndVertical();
            return result;
        }
        
        public static bool DrawTexturePreview(Rect position, Texture tex, string text="", bool selected=false, GUIStyle style=null)
        {
            var controlID = GUIUtility.GetControlID (FocusType.Passive);
            style ??= ImagePreviewStyle;

            switch (Event.current.GetTypeForControl(controlID))
            {
                case UnityEngine.EventType.Repaint:
                    if (tex)
                    {
                        var content = new GUIContent(text, tex);
                        style.Draw(position, content, controlID, selected);
                    }
                    else
                    {
                        style.Draw(position, GUIContent.none, controlID);
                    }
                    break;
                    
                case UnityEngine.EventType.MouseDown:
                    if (position.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
                        return true;
                    }
                    break;
                case UnityEngine.EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        //GUIUtility.hotControl = 0;
                        //if (position.Contains(Event.current.mousePosition)) selected = true;
                    }
                    break;
            }
                
            return selected;
        }
        
        public static void EnumFlags<T>(Func<T> currentFunc, Action<T> setter, System.Func<string, bool> btnFunc = null) where T : Enum
        {
            var flags = (T[])Enum.GetValues(typeof(T));
            var currentInt = Convert.ToInt32(currentFunc());
            var changed = false;
            foreach (var flag in flags)
            {
                var flagInt = Convert.ToInt32(flag);
                if (flagInt == 0 || flagInt == ~0) continue;

                var selected = (currentInt & flagInt) != 0;
                var name = ObjectNames.NicifyVariableName(flag.ToString());
                var color = selected ? (Color?)ColorUtils.lightBlue : null;
                using (new GuiColor(color ?? GUI.color))
                {
                    if (btnFunc?.Invoke(name) ?? GUILayout.Button(name))
                    {
                        if (selected) currentInt ^= flagInt;
                        else currentInt |= flagInt;
                        changed = true;
                    }
                }
            }
            if (changed)
            {
                setter?.Invoke((T)Enum.ToObject(typeof(T), currentInt));
            }
        }

        #region GridLayout

        public static (int control, float slider) GridLayout(
            int count, 
            ref Vector2 scroll, float itemWidth, float itemHeight, 
            Action<Rect, int> entryGui,
            float slider = 0,
            params GUIContent[] controls)
        {

            var control = -1;
            using (new HorizontalHelpBox())
            using (new GuiColor(Color.cyan))
            {
                EditorGUILayout.LabelField($"Display {count}");
                GUILayout.FlexibleSpace();
                controls.ForEach((i, k) => {
                    if (GUILayout.Button(i)) control = k;
                });

                slider = GUILayout.HorizontalSlider(slider, 0, 1, GUILayout.Width(120));
            }
            
            var rect = GUILayoutUtility.GetRect(
                GUIContent.none, 
                EditorStyles.miniButton, 
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true));

            if (rect.width < itemWidth || count == 0) return (control, slider);

            var itemsPerRow = Mathf.FloorToInt(rect.width / itemWidth);
            var totalRows = Mathf.CeilToInt((float)count / itemsPerRow);
            var viewRect = new Rect(0, 0, rect.width, totalRows * itemHeight);
                
            scroll = GUI.BeginScrollView(rect, scroll, viewRect);
            {
                var firstRow = Mathf.FloorToInt(scroll.y / itemHeight);
                var lastRow = Mathf.CeilToInt((scroll.y + rect.height) / itemHeight);
                    
                var xOffset = (rect.width - itemsPerRow * itemWidth) / 2;
                for (var index = 0; index < count; index++)
                {
                    var row = index / itemsPerRow;
                    if (row < firstRow || row > lastRow) continue;
                    var col = index % itemsPerRow;
                    var position = new Rect(xOffset + col * itemWidth, row * itemHeight, itemWidth, itemHeight);
                    entryGui?.Invoke(position, index);
                }
            }
            GUI.EndScrollView();

            return (control, slider);
        }

        #endregion

        #region PagingLayout

        public static (int page, int items) PagingLayout<TData>(
            List<TData> entries,
            int currentPage, int itemsPerPage,
            Predicate<TData> predicate,
            Action<TData, int> entryGUI,
            Action onBeginLayout = null,
            Action onEndLayout = null)
        {

            var filtered = Enumerable.Range(0, entries.Count)
                .Zip(entries, (i, data) => (entry: data, index: i))
                .Where(i => predicate(i.entry))
                .ToList();
            
            return PagingLayout(
                filtered,
                currentPage, itemsPerPage,
                index => entryGUI(index.entry, index.index),
                onBeginLayout, onEndLayout);
        }
        
        public static (int page, int items) PagingLayout<TData>(
            List<TData> entries,
            int currentPage, int itemsPerPage,
            Action<TData> entryGUI,
            Action onBeginLayout = null,
            Action onEndLayout = null)
        {
            return PagingLayout(
                entries.Count,
                currentPage, itemsPerPage,
                index => entryGUI(entries[index]),
                onBeginLayout, onEndLayout);
        }
        
        public static (int page, int items) PagingLayout(
            int count, 
            int currentPage, int itemsPerPage,
            Action<int> entryGUI, 
            Action onBeginLayout=null, 
            Action onEndLayout=null)
        {
            var indexedData = Enumerable.Range(0, count)
                .Skip(itemsPerPage * (currentPage - 1))
                .Take(itemsPerPage);

            using (new VerticalHelpBox())
            {
                (currentPage, itemsPerPage) = PageNav(count, currentPage, itemsPerPage);
                
                onBeginLayout?.Invoke();
                using (new VerticalLayout())
                {
                    foreach (var index in indexedData)
                    {
                        entryGUI(index);
                    }
                }
                onEndLayout?.Invoke();
                
                return (currentPage, itemsPerPage);
            }
        }

        private static (int page, int items) PageNav(int total, int currentPage, int itemPerPage)
        {
            using (new HorizontalHelpBox())
            {
                var maxPage = Mathf.CeilToInt((float) total / itemPerPage);
                currentPage = Math.Max(1, Math.Min(currentPage, maxPage));

                using (new GuiColor(Color.cyan))
                using (new HorizontalLayout())
                {
                    EditorGUILayout.LabelField($"Display {(currentPage - 1) * itemPerPage + 1} to {Math.Min(currentPage * itemPerPage, total)} of {total}");

                    GUILayout.FlexibleSpace();

                    itemPerPage = EditorGUILayout.DelayedIntField(GUIContent.none, itemPerPage, GUILayout.Width(30));


                    using (new DisabledGui(currentPage == 1))
                    {
                        if (GUILayout.Button("<<", GUILayout.Width(40))) currentPage = 0;
                        if (GUILayout.Button("<", GUILayout.Width(40))) currentPage--;
                    }

                    GUILayout.Button($"{currentPage}/{maxPage}", GUILayout.Width(80));

                    using (new DisabledGui(currentPage == maxPage))
                    {
                        if (GUILayout.Button(">", GUILayout.Width(40))) currentPage++;
                        if (GUILayout.Button(">>", GUILayout.Width(40))) currentPage = maxPage;
                    }

                }

                return (Math.Max(1, Math.Min(currentPage, maxPage)), Math.Max(1, itemPerPage));
            }
        }

        #endregion


        private static Material guiMaterial;

        private static Material GuiMaterial
        {
            get
            {
                if (guiMaterial) return guiMaterial;
                var shader = Shader.Find("Hidden/Internal-Colored");
                guiMaterial = new Material(shader);
                return guiMaterial;
            }
        }

        public static void Line(Rect rect, Vector2 from, Vector2 to, Color color)
        {
            if (Event.current.type == UnityEngine.EventType.Repaint)
            {
                GUI.BeginClip(rect);
                GL.PushMatrix();
                GL.Clear(true, false, Color.black);
                GuiMaterial.SetPass(0);
         
                GL.Begin(GL.LINES);
                GL.Color(color);
                GL.Vertex3(from.x, from.y, 0);
                GL.Vertex3(to.x, to.y, 0);
                GL.End();
                GUI.EndClip();
            }
        }
    }
}

#endif