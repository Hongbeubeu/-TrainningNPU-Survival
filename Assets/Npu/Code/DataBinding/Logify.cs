using System;
using UnityEngine;

namespace Npu
{
    public class Logify : MonoBehaviour
    {
        public LogType logType;
        public string clearFormat = "{0}";
        public string separator = "\n";
        string msg = "";

        public void Clear(object data)
        {
            msg = string.Format(clearFormat, data);
        }

        public void Append(object data)
        {
            msg = $"{msg}{separator}{data}";
        }

        public void Out(object data)
        {
            Append(data);
            Log(msg);
        }

        public void Log(object data)
        {
            switch (logType)
            {
                case LogType.Log:
                    Debug.Log($"{data}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"{data}");
                    break;
                case LogType.Error:
                    Debug.LogError($"{data}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        public enum LogType
        {
            Log, Warning, Error
        }
    }

}