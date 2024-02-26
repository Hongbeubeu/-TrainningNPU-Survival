using UnityEngine;

namespace Npu
{

    public class DataBinderLateBindGroup : MonoBehaviour
    {
        [SerializeField] bool lateBind = true;

        public bool LateBind => lateBind;
    }
}