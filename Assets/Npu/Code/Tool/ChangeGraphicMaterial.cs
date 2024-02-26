using Npu.EditorSupport;
using Npu.EditorSupport.Inspector;
using UnityEngine;
using UnityEngine.UI;

public class ChangeGraphicMaterial : MonoBehaviour
{
    public Material material;
    
#if UNITY_EDITOR

    private Graphic[] _graphics;

    [InspectorGUI]
    private void Set()
    {
        _graphics = transform.GetComponentsInChildren<Graphic>(true);

        foreach (var g in _graphics)
        {
            g.DirtyWithUndo();
            g.material = material;
        }
    }

#endif

}
