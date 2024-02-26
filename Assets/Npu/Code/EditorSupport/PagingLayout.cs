using System;
using System.Collections.Generic;

#if UNITY_EDITOR

namespace Npu.EditorSupport
{

    
    public class PagingLayout<TData> : ListLayoutBase<TData>
    {
        public delegate void CallbackDelegate();
        
        public CallbackDelegate OnBeginLayout;
        public CallbackDelegate OnEndLayout;

        public Action<int> EntryGUI { get; set; }

        public PagingLayout(
            string key, 
            List<TData> data, 
            Action<int> entryGUI, 
            Predicate<(TData entry, string pattern)> predicate = null) : base(key, data, predicate)
        {
            EntryGUI = entryGUI;
        }

        protected override void DoLayout(List<int> visibleIndices)
        {
            (Settings.CurrentPage, Settings.ItemsPerPage) = EditorGUIUtils.PagingLayout(
                visibleIndices.Count, Settings.CurrentPage, Settings.ItemsPerPage,
                index => EntryGUI(visibleIndices[index]),
                () => OnBeginLayout?.Invoke(), 
                () => OnEndLayout?.Invoke());
        }
    }
}

#endif