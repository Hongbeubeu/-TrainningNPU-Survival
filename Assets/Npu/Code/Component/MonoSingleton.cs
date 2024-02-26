using UnityEngine;

namespace Npu.Utilities
{
    /// <summary>
    /// Inherit this class for singleton pattern. If the child class does not have an active instance in the scene, no new instance will be automatically created.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static object m_Lock = new object();
        private static T _instance;
        private static bool instanced;

        public static T Instance
        {
            get
            {
                //Lock one thread use only
                lock (m_Lock)
                {
                    if (!_instance)
                    {
                        _instance = FindExistingSingleton();
                        (_instance as MonoSingleton<T>)?.OnNewSingletonInstance();
                    }
                    
                    return _instance;
                }
            }
        }
        
        protected virtual void OnNewSingletonInstance()
        {
            instanced = true;
        }

        static T FindExistingSingleton()
        {
            return FindObjectOfType<T>();// ?? ObjectUtils.GetSceneObjectOfType<T>();
        }
        
        protected virtual void Awake()
        {
            if (_instance && _instance != this)
            {
                Debug.LogError("Multiple MonoSingleton instances of " + this + " detected.");
            }
            else
            {
                _instance = this as T;
                OnNewSingletonInstance();
            }
        }
    }
}