using UnityEngine;

namespace Npu
{
    public class DataExchanger : MonoBehaviour
    {
        [SerializeField, MemberSelector(returnType = typeof(object), labelWidth = 100)]
        private MemberSelector src;
        [SerializeField, MemberSelector(parameterType = typeof(object), labelWidth = 100)]
        private MemberSelector dst;

        [ContextMenu("Exchange")]
        public void Exchange()
        {
            src.Setup();
            dst.Setup();
            
            if (!src.IsValid || !dst.IsValid) return;
            
            dst.SetValue(src.GetValue());
        }
    }
}