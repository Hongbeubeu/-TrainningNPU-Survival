#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Npu.EditorSupport
{
    public static class EditorGameViewSize
    {

        public static GameViewSizeGroupType CurrentViewGroupType
        {
            get
            {
#if UNITY_IPHONE
                return GameViewSizeGroupType.iOS;
#endif

#if UNITY_ANDROID
            return GameViewSizeGroupType.Android;
#endif
                return GameViewSizeGroupType.Standalone;
            }
        }

        static object CurrentViewGroup
        {
            get { return GetGroupMethodInfo.Invoke(GameViewSizesInstance, new object[] { (int)CurrentViewGroupType }); }
        }

        static object gameViewSizesInstance;
        static object GameViewSizesInstance
        {
            get
            {
                if (gameViewSizesInstance == null)
                {
                    var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
                    var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
                    var instanceProp = singleType.GetProperty("instance");
                    gameViewSizesInstance = instanceProp.GetValue(null, null);
                }
                return gameViewSizesInstance;
            }
        }

        static MethodInfo getGroupMethodInfo;
        static MethodInfo GetGroupMethodInfo
        {
            get
            {
                if (getGroupMethodInfo == null)
                {
                    var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
                    getGroupMethodInfo = sizesType.GetMethod("GetGroup");
                }

                return getGroupMethodInfo;
            }
        }

        public static int CurrentGameViewIndex
        {
            get 
            {
                var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
                var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var gvWnd = EditorWindow.GetWindow(gvWndType);
                return (int)selectedSizeIndexProp.GetValue(gvWnd, null);    
            }
            set
            {
                var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
                var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var gvWnd = EditorWindow.GetWindow(gvWndType);
                selectedSizeIndexProp.SetValue(gvWnd, value, null);
                SetMinimumScale();
            }
        }

        public static GameView CurrentGameView
        {
            get
            {
                var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
                var currentGameViewSizeProp = gvWndType.GetProperty("currentGameViewSize",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var gvWnd = EditorWindow.GetWindow(gvWndType);
                var obj = currentGameViewSizeProp.GetValue(gvWnd, null);

                return new GameView 
                {
                    width = (int)  GetPropertyValue(obj, "width"),
                    height = (int)GetPropertyValue(obj, "height"),
                    name = (string)GetPropertyValue(obj, "baseText")
                };
            }

            set
            {
                var index = FindViewSizeIndex(value.width, value.height, value.name);
                if (index >= 0) CurrentGameViewIndex = index;
                else Debug.LogErrorFormat("Cannot find Game View {0} ({1}x{2})", value.name, value.width, value.height);
            }
        }

        public static int FindViewSizeIndex(int width, int height, string name)
        {
            var group = CurrentViewGroup;
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            var sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gvsType = getGameViewSize.ReturnType;
            var widthProp = gvsType.GetProperty("width");
            var heightProp = gvsType.GetProperty("height");
            var textProp = gvsType.GetProperty("baseText");

            var indexValue = new object[1];
            for (var i = 0; i < sizesCount; i++)
            {
                indexValue[0] = i;
                var size = getGameViewSize.Invoke(group, indexValue);
                var sizeWidth = (int)widthProp.GetValue(size, null);
                var sizeHeight = (int)heightProp.GetValue(size, null);
                var text = (string)textProp.GetValue(size, null);

                if (sizeWidth == width && sizeHeight == height && text.Equals(name))
                    return i;
            }

            return -1;
        }
    
        public static int GetSizeCount()
        {
            var group = CurrentViewGroup;
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            return (int) getBuiltinCount.Invoke(group, null) + (int) getCustomCount.Invoke(group, null);
        }

        public static void SetMinimumScale()
        {
            var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var gvWnd = EditorWindow.GetWindow(gvWndType);
        
            var minScale = gvWndType.GetProperty("minScale",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var minValue = (float) minScale.GetValue(gvWnd);
        
            var areaField = gvWndType.GetField("m_ZoomArea",
                BindingFlags.Instance | BindingFlags.NonPublic);
            var areaObj = areaField.GetValue(gvWnd);
            var scaleField = areaObj.GetType().GetField("m_Scale",
                BindingFlags.Instance | BindingFlags.NonPublic);
        
            Debug.Log("minValue: " + minValue);
        
            Debug.Log("pre: " + ((Vector2)scaleField.GetValue(areaObj)).x);
            scaleField.SetValue(areaObj, new Vector2(minValue, minValue));
            Debug.Log("post: " + ((Vector2)scaleField.GetValue(areaObj)).x);
        }

        public static void AddCustomViewSize(int width, int height, string name, bool fixedResolution=true)
        {
            var group = CurrentViewGroup;
            var addCustomSize = getGroupMethodInfo.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
            var gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");
            var gvstType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizeType");
            var ctor = gvsType.GetConstructor(new Type[] { gvstType, typeof(int), typeof(int), typeof(string) });
            var newSize = ctor.Invoke(new object[] { fixedResolution ? 1 : 0, width, height, name });
            addCustomSize.Invoke(group, new object[] { newSize });
        }

        static object GetPropertyValue(object target, string name, BindingFlags flags=BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        {
            var prop = target.GetType().GetProperty(name, flags);
            return prop.GetValue(target, null);
        }

        public struct GameView
        {
            public int width;
            public int height;
            public string name;

            public float Aspect => (float)width / height;

            public override string ToString()
            {
                return string.Format("{0} ({1}x{2})", name, width, height);
            }
        }
    }
}

#endif
