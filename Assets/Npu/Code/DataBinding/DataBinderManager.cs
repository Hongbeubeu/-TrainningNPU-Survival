using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Npu.EditorSupport;
using Npu.EditorSupport.Inspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu
{

    public class DataBinderManager : MonoBehaviour
    {

        public static bool Register(DataBinder b)
        {
            return Instance != null && Instance.DoRegister(b);
        }

        public static void Unregister(DataBinder b)
        {
            if (Instance != null)
            {
                Instance.DoUnregister(b);
            }
        }

        public static void OnBindDelayed(DataBinder b)
        {
#if UNITY_EDITOR
            _frameDelayCalls++;
            _totalDelayedCalls++;
#endif
        }
        

#if UNITY_EDITOR
        private static int _totalDelayedCalls = 0;
        private static int _frameDelayCalls;
        private static float _averageDelayCalls;
#endif

        private static bool _everCreated;
        private static DataBinderManager _instance;

        private static DataBinderManager Instance
        {
            get
            {
                if (_everCreated || _instance) return _instance;
                
                _everCreated = true;
                var go = new GameObject("DataBinderManager");
                _instance = go.AddComponent<DataBinderManager>();
                DontDestroyOnLoad(go);
                return _instance;
            }
        }

        private readonly HashSet<DataBinder> _binders = new HashSet<DataBinder>();
        private readonly Dictionary<DataBinder, int> _queuedBinders = new Dictionary<DataBinder, int>();
        private bool _isUpdating;
        private int _startFrame;
        
        private void OnEnable()
        {
            _startFrame = Time.frameCount;
        }

        private void OnDestroy()
        {
            if (_everCreated && this == _instance)
            {
                _instance = null;
            }
        }

        private bool DoRegister(DataBinder binder)
        {
            if (_isUpdating)
            {
                _queuedBinders[binder] = 1;
                return true;
            }

            _binders.Add(binder);
            return true;
        }

        private void DoUnregister(DataBinder binder)
        {
            if (_isUpdating)
            {
                _queuedBinders[binder] = -1;
                return;
            }

            _binders.Remove(binder);
        }

        private void LateUpdate()
        {
#if UNITY_EDITOR            
            _averageDelayCalls = _frameDelayCalls;
            _frameDelayCalls = 0;
#endif            
            _isUpdating = true;

            foreach (var b in _binders)
            {
                b.DoLateBind();
            }

            _isUpdating = false;
            

            UpdateBinders();
        }

        private void UpdateBinders()
        {
            if (_queuedBinders.Count <= 0) return;
            
            foreach (var kvp in _queuedBinders)
            {
                var add = kvp.Value;
                var b = kvp.Key;
                if (add > 0)
                {
                    _binders.Add(b);
                }
                else
                {
                    _binders.Remove(b);
                }
            }
            _queuedBinders.Clear();
        }


#if UNITY_EDITOR
        [InspectorGUI(gui = true, group = "Stats")]
        protected void Editor_GUI()
        {
            using (new VerticalLayout())
            {
                EditorGUILayout.LabelField("DataBinders", _binders.Count.ToString());
                EditorGUILayout.LabelField("LateBinds", _binders.Count(b => b.IsLateBind).ToString());
                EditorGUILayout.LabelField("Average Optimized Calls", $"{(float)_totalDelayedCalls/(Time.frameCount - _startFrame):0.#}");
                EditorGUILayout.LabelField("Optimized Calls this Frame", $"{_averageDelayCalls:0.#}");
            }
            
        }
#endif

    }

}