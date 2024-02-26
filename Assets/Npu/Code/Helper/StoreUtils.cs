using UnityEngine;

namespace Npu.Helper
{
    public class StoreUtils
    {
#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	private static extern void StoreUtils_openStore(string appId);
	[DllImport ("__Internal")]
	private static extern void StoreUtils_openInGameStore(string appId);
	[DllImport ("__Internal")]
	private static extern void StoreUtils_openAppOrInGameStore(string identifier, string appId);
#endif

        public static void OpenStore(string appId)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
		StoreUtils_openStore(appId);
#elif UNITY_ANDROID && !UNITY_EDITOR
		StoreUtilsAndroid.openStore(appId);
#endif
        }

        public static void OpenInGameStore(string appId)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
		StoreUtils_openInGameStore(appId);
#elif UNITY_ANDROID
            StoreUtilsAndroid.openStore(appId);
#endif
        }

        public static void OpenAppOrInGameStore(string identifier, string appId)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
		StoreUtils_openAppOrInGameStore(identifier, appId);
#elif UNITY_ANDROID
            StoreUtilsAndroid.openAppOrInGameStore(appId);
#endif
        }
    }

#if UNITY_ANDROID
    class StoreUtilsAndroid
    {
        const string JavaClassName = "com.nopowerup.screw.utils.StoreUtils";

        public static void openStore(string appId)
        {
            // Java public static void openStore(final String packageName)
            using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
            {
                jc.CallStatic("openStore", appId);
            }
        }


        public static void openAppOrInGameStore(string appId)
        {
            // public static void openAppOrInGameStore(final String packageName)
            using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
            {
                jc.CallStatic("openAppOrInGameStore", appId);
            }
        }
    }
#endif
}