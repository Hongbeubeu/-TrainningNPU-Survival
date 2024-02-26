using UnityEngine;
using System;
using System.Reflection;

#if UNITY_EDITOR

#endif

namespace Npu.EditorSupport
{
    public static class DeviceUtils
    {

        public static float Aspect
        {
            get
            {
#if UNITY_EDITOR
                if (IsRunningInDeviceSimulator) return (float)Mathf.Min(Screen.currentResolution.width, Screen.currentResolution.height) / Mathf.Max(Screen.currentResolution.width, Screen.currentResolution.height);
                var r = GetGameView();
                return r.x / r.y;
#else
                return (float)Mathf.Min(Screen.width, Screen.height) / Mathf.Max(Screen.width, Screen.height);
#endif
            }
        }

        static bool? _iPhoneX;
        public static bool iPhoneX
        {
            get
            {
#if UNITY_EDITOR || UNITY_IPHONE
#if !UNITY_EDITOR
                if (_iPhoneX == null)
#endif
                {
                    var thisAspect = Aspect;
                    _iPhoneX = thisAspect < 0.5f;
                }

                return _iPhoneX.Value;
#endif

                return false;
            }
        }

        static bool? isTablet;

        public static bool IsTablet
        {
            get
            {
#if !UNITY_EDITOR
            if (isTablet == null)
#endif
                {
#if UNITY_IPHONE || UNITY_EDITOR
                    var thisAspect = Aspect;
                    isTablet = thisAspect > 0.7f;
#else
                    isTablet = false;//UtilsAndroid.IsTablet ();
#endif
                    Debug.LogFormat("Device Detected: IsTablet ? {0}", isTablet);
                }
                return isTablet.Value;
            }
        }

#if UNITY_ANDROID
        public static bool IsAndroid => true;
#else
    public static bool IsAndroid => false;
#endif

#if UNITY_IPHONE
    public static bool IsIPhone => true;
#else
        public static bool IsIPhone => false;
#endif
        
#if UNITY_EDITOR
        private static bool? _isRunningInDeviceSimulator;

        public static bool IsRunningInDeviceSimulator
        {
            get
            {
                if (_isRunningInDeviceSimulator == null) _isRunningInDeviceSimulator = CheckRunningInDeviceSimulator();
                return _isRunningInDeviceSimulator.Value;
            }
        }
        
        [ContextMenu("Check Running in Simulator")]
        public static bool CheckRunningInDeviceSimulator()
        {
            var simWinType = Type.GetType("Unity.DeviceSimulator.SimulatorWindow, Unity.DeviceSimulator.Editor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            if (simWinType == null)
            {
                return false;
            }

            return Resources.FindObjectsOfTypeAll(simWinType).Length > 0;
        }
    
        public static Vector2 GetGameView()
        {
            var T = Type.GetType("UnityEditor.GameView,UnityEditor");
            var getSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView", BindingFlags.NonPublic | BindingFlags.Static);
            var resolution = getSizeOfMainGameView.Invoke(null, null);
            return (Vector2)resolution;
        }
    
#endif        
        
    }

#if UNITY_ANDROID
    public static class AndroidDeviceUtils
    {

        public const int ORIENTATION_UNDEFINED = 0x00000000;
        public const int ORIENTATION_PORTRAIT = 0x00000001;
        public const int ORIENTATION_LANDSCAPE = 0x00000002;

        public const int ROTATION_0 = 0x00000000;
        public const int ROTATION_90 = 0x00000001;
        public const int ROTATION_180 = 0x00000002;
        public const int ROTATION_270 = 0x00000003;

        public const int PORTRAIT = 0;
        public const int PORTRAIT_UPSIDEDOWN = 1;
        public const int LANDSCAPE = 2;
        public const int LANDSCAPE_LEFT = 3;


        static AndroidJavaObject mConfig;
        static AndroidJavaObject mWindowManager;

        //adapted from http://stackoverflow.com/questions/4553650/how-to-check-device-natural-default-orientation-on-android-i-e-get-landscape/4555528#4555528
        public static int GetDeviceDefaultOrientation()
        {
            if ((mWindowManager == null) || (mConfig == null))
            {
                using (AndroidJavaObject activity =
                    new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>(
                        "currentActivity"))
                {
                    mWindowManager = activity.Call<AndroidJavaObject>("getSystemService", "window");
                    mConfig = activity.Call<AndroidJavaObject>("getResources")
                        .Call<AndroidJavaObject>("getConfiguration");
                }
            }

            int lRotation = mWindowManager.Call<AndroidJavaObject>("getDefaultDisplay").Call<int>("getRotation");
            int dOrientation = mConfig.Get<int>("orientation");

            if ((((lRotation == ROTATION_0) || (lRotation == ROTATION_180)) &&
                 (dOrientation == ORIENTATION_LANDSCAPE)) ||
                (((lRotation == ROTATION_90) || (lRotation == ROTATION_270)) && (dOrientation == ORIENTATION_PORTRAIT)))
            {
                return (LANDSCAPE); //TABLET
            }

            return (PORTRAIT); //PHONE
        }

        public static bool IsTablet
        {
            get { return GetDeviceDefaultOrientation() == LANDSCAPE; }
        }

    }
#endif

}