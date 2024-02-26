using System;
using UnityEngine;

namespace Npu.Helper
{
    public class PreserveActivation : IDisposable
    {
        private GameObject gameObject;
        private bool active;
        
        public PreserveActivation(GameObject go)
        {
            active = go.activeSelf;
            gameObject = go;
        }
        
        public void Dispose()
        {
            gameObject.SetActive(active);        
        }
    }
}