using System;
using Npu.Helper;
using UnityEngine;

namespace Npu
{
    public class NativeMessageReceiver : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        
        public void OnNativeMessage(string msg)
        {
            Debug.LogFormat("OnNativeMessage({0})", msg);
            if (string.IsNullOrEmpty(msg)) return;

            var opts = msg.Split(new char[] {':'});

            // System Dialog
            if (opts[0].Equals("dialog", StringComparison.Ordinal))
            {
                if (opts.Length < 1)
                {
                    Debug.LogErrorFormat("OnNativeMessage: invalid msg: {0}", msg);
                    return;
                }

                try
                {
                    Utils.OnNativeDialogClicked(int.Parse(opts[1]));
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("OnNativeMessage: {0}", ex.Message);
                }
            }
        }
    }
}