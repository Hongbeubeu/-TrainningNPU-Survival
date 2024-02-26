using System;
using UnityEngine;


namespace Npu
{
    
    public class PropertyBlockSetter : MonoBehaviour
    {

        [SerializeField] public Renderer renderer;
        [SerializeField] private bool setOnEnable = true;
        [SerializeField] private bool clearOnDisable;
        [SerializeField, Box] public BlockValue[] values;

        private MaterialPropertyBlock block;

        private void OnEnable()
        {
            if (setOnEnable) Set();
        }

        private void OnDisable()
        {
            if (clearOnDisable) Clear();
        }

        [ContextMenu("Set Block")]
        public void Set()
        {
            if (block == null) block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);

            foreach (var v in values)
            {
                if (string.IsNullOrEmpty(v.name)) continue;
                
                switch (v.value.kind)
                {
                    case BindingData.Kind.Int:
                        block.SetInt(v.name, v.value.IntValue);
                        break;
                    case BindingData.Kind.Float:
                        block.SetFloat(v.name, v.value.FloatValue);
                        break;
                    case BindingData.Kind.Color:
                        block.SetColor(v.name, v.value.ColorValue);
                        break;
                    case BindingData.Kind.Vector4:
                        block.SetVector(v.name, v.value.Vector4Value);
                        break;
                    default:
                        Logger.Error<PropertyBlockSetter>($"{v.value.kind} not supported");
                        break;
                }
            }
            
            renderer.SetPropertyBlock(block);
        }

        [ContextMenu("Clear Block")]
        public void Clear()
        {
            renderer.SetPropertyBlock(null);
        }
        
        [Serializable]
        public class BlockValue
        {
            public string name;
            public BindingData value;
        }

    }
}
