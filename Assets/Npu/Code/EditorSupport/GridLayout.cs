#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Npu.EditorSupport
{
    public class GridLayout<TData> : ListLayoutBase<TData>
    {
        private Vector2 _scroll;
        private readonly float _minItemWidth;
        private readonly float _maxItemWidth;
        private readonly float _minItemHeight;
        private readonly float _maxItemHeight;

        private float ItemSize
        {
            get => Settings.GetFloat("itemSize", 0.5f);
            set => Settings.SetFloat("itemSize", value);
        }
        
        public float ItemWidth => Mathf.Lerp(_minItemWidth, _maxItemWidth, ItemSize); 
        public float ItemHeight => Mathf.Lerp(_minItemHeight, _maxItemHeight, ItemSize); 
        
        public Action<Rect, int> EntryGUI { get; set; }
        
        public GridLayout(string key, 
            List<TData> data,
            float minItemWidth, float maxItemWidth,
            float minItemHeight, float maxItemHeight,
            Action<Rect, int> entryGUI,
            Predicate<(TData entry, string pattern)> predicate=null) : base(key, data, predicate)
        {
            EntryGUI = entryGUI;
            _scroll = Vector2.zero;
            _minItemWidth = minItemWidth;
            _maxItemWidth = maxItemWidth;
            _minItemHeight = minItemHeight;
            _maxItemHeight = maxItemHeight;
        }

        protected override void DoLayout(List<int> visibleIndices)
        {
            (_, ItemSize) = EditorGUIUtils.GridLayout(
                visibleIndices.Count, ref _scroll, ItemWidth, ItemHeight, 
                (rect, i) => EntryGUI?.Invoke(rect, Indices[i]), 
                ItemSize);
        }
    }
    
    public abstract class ListLayoutBase<TData>
    {
        protected Predicate<(TData entry, string pattern)> Predicate { get; set; }
        protected List<int> Indices { get; set; }
        protected bool Changed { get; set; }

        private List<TData> _data;
        public List<TData> Data
        {
            get => _data;
            set
            {
                _data = value;
                Filter();
            }
        }

        public EditorCommonSettings Settings { get; }

        protected ListLayoutBase(
            string key, 
            List<TData> data,
            Predicate<(TData entry, string pattern)> predicate=null)
        {
            Settings = new EditorCommonSettings(key);
            Predicate = predicate;
            Data = data;
        }

        public void DoLayout(List<TData> data, List<string> searchSuggestions)
        {
            Data = data;
            DoLayout(searchSuggestions);
        }
        
        public void DoLayout(List<string> searchSuggestions)
        {
            if (Predicate != null)
            {
                SearchPatternGui(searchSuggestions);
                if (Changed)
                {
                    Filter();
                }
            }

            DoLayout(Indices);
        }

        protected abstract void DoLayout(List<int> visibleIndices);
        
        
        protected void Filter()
        {
            Changed = false;
            if (Predicate == null || string.IsNullOrEmpty(Settings.SearchPattern))
            {
                Indices = Enumerable.Range(0, Data.Count).ToList();
            }
            else
            {
                Indices = Enumerable.Range(0, Data.Count)
                    .Zip(Data, (index, entry) => (index, entry))
                    .Where(i => Predicate.Invoke((i.entry, Settings.SearchPattern)))
                    .Select(i => i.index).ToList();
                
            }
        }
        
        protected void SearchPatternGui(IReadOnlyCollection<string> suggestions=null)
        {
            using (new HorizontalHelpBox())
            using (new LabelWidth(100))
            {
                EditorGUI.BeginChangeCheck();
                Settings.SearchPattern = EditorGUILayout.DelayedTextField("Search", Settings.SearchPattern);
                Changed |= EditorGUI.EndChangeCheck();
                
                using (new GuiColor(Color.cyan))
                {
                    if (GUILayout.Button("x", GUILayout.Width(30)))
                    {
                        Settings.SearchPattern = "";
                        Changed = true;
                    }
                    
                    if (suggestions != null && suggestions.Count > 0 && GUILayout.Button("...", GUILayout.Width(30)))
                    {
                        var menu = new GenericMenu();

                        foreach (var i in suggestions)
                        {
                            menu.AddItem(new GUIContent(i), false, data =>
                            {
                                Settings.SearchPattern = data as string;
                                Changed = true;
                            }, i);
                        }
                        
                        menu.ShowAsContext();
                    }
                }
            }
        }
    }
    
    
}

#endif