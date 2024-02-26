using System.Collections.Generic;
using Npu.EditorSupport;
using Npu.EditorSupport.Inspector;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu.Tools
{
    public class ReferenceFinder : MonoBehaviour
    {
        public int maxFound = 1;
        
#if UNITY_EDITOR
        private Object _root;
        private Object _target;
        
        [InspectorGUI(gui = true)]
        private void GUI()
        {
            using (new VerticalLayout())
            {
                _root = EditorGUILayout.ObjectField("Root", _root, typeof(Object), true);
                _target = EditorGUILayout.ObjectField("Target", _target, typeof(Object), true);

                using (new DisabledGui(!_root || !_target))
                {
                    if (GUILayout.Button("Find"))
                    {
                        found = 0;
                        Find("", _root, _target, new List<Object>());

                    }
                    
                }
                
            }
        }

        private int found = 0;
        
        private void Find(string path, Object root, Object target, List<Object> traversed)
        {
            if (found >= maxFound)
            {
                return;
            }

            if (traversed.Contains(root))
            {
                return;
            }
            traversed.Add(root);
            
            foreach (var i in root.EnumerateSerializedProperties())
            {
                if (i.propertyType != SerializedPropertyType.ObjectReference) continue;

                var objectRef = i.objectReferenceValue;
                if (objectRef == target)
                {
                    Debug.Log($"[{root.name}] [{path}] {i.propertyPath}");
                    found++;
                    
                    if (found >= maxFound) return;
                }
                else if (objectRef)
                {
                    var newPath = $"{path}/{objectRef.name}|{i.propertyPath}";
                    Find(newPath, objectRef, target, traversed);
                }
            }
        }
#endif
    }
    
}
