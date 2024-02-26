using System.Collections.Generic;
using System.Linq;
using Npu;
using UnityEditor;
using UnityEngine;

public interface IGalleryItemProvider
{
    IEnumerable<GalleryHelper.Item> Items { get; }
}

public class GalleryHelper : MonoBehaviour
{
    [TypeConstraint(typeof(IGalleryItemProvider))] public Object provider;
    
    private IGalleryItemProvider Provider => provider as IGalleryItemProvider;

    public void Instantiate(bool clear)
    {
        if (Provider == null) return;

        if (clear)
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);    
            }
        }

        foreach (var item in Provider.Items.GroupBy(i => i.Group)) Instantiate(item);
    }

    private void Instantiate(IGrouping<string, Item> group)
    {
        var parent = transform;
        if (!string.IsNullOrEmpty(group.Key))
        {
            parent = transform.Find(group.Key);
            if (!parent)
            {
                parent = new GameObject(group.Key).transform;
                parent.parent = transform;
                parent.localPosition = Vector3.zero;
            }
        }

#if UNITY_EDITOR        
        foreach (var i in group)
        {
            var o = PrefabUtility.InstantiatePrefab(i.Prefab, parent);
            o.name += $" ({i.Name})";    
        }
#endif        
    }

    [ContextMenu("Add")]
    private void Add() => Instantiate(false);
    
    [ContextMenu("Create")]
    private void Create() => Instantiate(true);
    
    public class Item
    {
        public string Group { get; set; }
        public string Name { get; set; }
        public GameObject Prefab { get; set; }
    }
}
