using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

#endif

namespace Npu.Helper
{

    public static partial class Utils
    {
        public static string FacebookUri(string id) => $"fb://profile/{id}";
        public static string FacebookPageUri(string id) => $"fb://page?id={id}";
        public static string FacebookFallbackUri(string id) => $"https://www.facebook.com/{id}";
        public static string InstagramUri(string id) => $"instagram://user?username={id}";
        public static string InstagramFallbackUri(string id) => $"https://www.instagram.com/{id}";
        public static string TwitterUri(string id) => $"twitter://user?screen_name={id}";
        public static string TwitterFallbackUri(string id) => $"https://twitter.com/{id}";

#if UNITY_IPHONE && !UNITY_EDITOR
	    [DllImport ("__Internal")]
	    private static extern bool Utils_isAppInstalled(string identifier);
	    [DllImport ("__Internal")]
	    private static extern void Utils_open(string uri, string fallback);
	    [DllImport ("__Internal")]
	    private static extern void Utils_openApp(string identifier);
	    [DllImport ("__Internal")]
	    private static extern int Utils_getVersionCode();
	    [DllImport ("__Internal")]
	    private static extern string Utils_getVersionName();
	    [DllImport ("__Internal")]
	    private static extern void Utils_registerNotifications();
	    [DllImport ("__Internal")]
	    private static extern void Utils_clearAppBadge ();
	    [DllImport ("__Internal")]
	    private static extern void Utils_openDeveloperStore (string id);

        [DllImport ("__Internal")]
        private static extern void Utils_setAdsConsentStatus (int status);

        [DllImport ("__Internal")] private static extern string Utils_getIdfa ();
        [DllImport ("__Internal")] private static extern string Utils_getVendorIdentifier ();
        
        [DllImport ("__Internal")] private static extern void Utils_showAlert (string title, string message, string okButton, string cancelButton, string moreButton);
        [DllImport ("__Internal")] private static extern void Utils_showRate ();
        
        [DllImport ("__Internal")] private static extern string Utils_getSystemVersion ();
#endif

        public static string NewGuid(int length) =>
            System.Guid.NewGuid().ToString().Replace("-", "").Substring(0, length);

        public static bool IsAppInstalled(string identifier)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
		return Utils_isAppInstalled(identifier);
#elif UNITY_ANDROID
        return UtilsAndroid.IsAppInstalled(identifier);
#endif
            return false;
        }

        public static void OpenApp(string identifier)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
		Utils_openApp(identifier);
#elif UNITY_ANDROID
        UtilsAndroid.OpenApp(identifier);
#endif
        }

        public static void Open(string uri, string fallbackUri)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
        Utils_open(uri, fallbackUri);
#endif
        }

        public static void OpenFacebook(string id)
        {
            Open(FacebookUri(id), FacebookFallbackUri(id));
        }

        public static void OpenFacebookPage(string id)
        {
#if UNITY_IPHONE        
            Open(FacebookPageUri(id), FacebookFallbackUri(id));
#else
            UtilsAndroid.OpenFacebook(id);
#endif
        }

        public static void OpenTwitter(string username)
        {
#if UNITY_IPHONE
            Open(TwitterUri(username), TwitterFallbackUri(username));
#else
            UtilsAndroid.OpenTwitter(username);
#endif            
        }

        public static void OpenInstagram(string username)
        {
#if UNITY_IPHONE            
            Open(InstagramUri(username), InstagramFallbackUri(username));
#else
            UtilsAndroid.OpenInstagram(username);
#endif
        }

        public static void OpenDeveloperStore(string id)
        {
#if UNITY_IPHONE && !UNITY_EDITOR
		Utils_openDeveloperStore(id);
#elif UNITY_ANDROID
        UtilsAndroid.OpenDeveloperStore(id);
#endif
        }

        public static void FinishActivity()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
		UtilsAndroid.FinishActivity();
#endif
        }

        public static void ShowToast(string message)
        {
#if UNITY_ANDROID
        UtilsAndroid.ShowToast(message);
#endif
        }

        public static int GetAppVersionCode()
        {
#if UNITY_EDITOR
#if UNITY_IPHONE
		    return int.Parse(PlayerSettings.iOS.buildNumber);
#elif UNITY_ANDROID
        return PlayerSettings.Android.bundleVersionCode;
#else
            return 0;
#endif
#elif UNITY_IPHONE
		return Utils_getVersionCode();
#elif UNITY_ANDROID
		return UtilsAndroid.GetAppVersionCode();
#else
        return 0;
#endif
        }

        public static string GetAppVersionName()
        {
#if UNITY_EDITOR
            return PlayerSettings.bundleVersion;
#elif UNITY_IPHONE
		return Utils_getVersionName();
#elif UNITY_ANDROID
		return UtilsAndroid.GetAppVersionName();

#else
        return "";
#endif
        }

        public static string GetSystemVersion()
        {
#if UNITY_EDITOR
            return "0.0.0";
#elif UNITY_IPHONE
		return Utils_getSystemVersion();
#else
		return "0.0";
#endif
        }

        public static string GetPackageName()
        {
#if UNITY_EDITOR
            return PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
#elif UNITY_ANDROID
		return UtilsAndroid.GetPackageName();
#else
        return "";
#endif
        }

        public static void RegisterNotifications()
        {
#if UNITY_IPHONE && !UNITY_EDITOR
		Utils_registerNotifications ();
#endif
        }

        public static void ClearAppBadge()
        {
#if UNITY_IPHONE && !UNITY_EDITOR
		Utils_clearAppBadge ();
#endif
        }

        public static string GetParameterDeeplink()
        {
#if UNITY_ANDROID
        return UtilsAndroid.GetParameterDeeplink();
#else
            return "";
#endif
        }

        public static string Idfa
        {
            get
            {
#if UNITY_EDITOR
                return "";
#elif UNITY_IPHONE
            return Utils_getIdfa();
#elif UNITY_ANDROID
            return UtilsAndroid.GetAdvertisingId();
#else
            return "";
#endif
            }
        }

#if UNITY_EDITOR
        static string _FakedVendorIdentifier
        {
            get
            {
                var id = PlayerPrefs.GetString("_faked_vendor_id_");
                if (string.IsNullOrEmpty(id))
                {
                    id = System.Guid.NewGuid().ToString();
                    PlayerPrefs.SetString("_faked_vendor_id_", id);
                }

                return id;
            }
        }
#endif

        public static string VendorIdentifier
        {
            get
            {
#if UNITY_EDITOR
                return _FakedVendorIdentifier;
#elif UNITY_IPHONE
            return Utils_getVendorIdentifier();
#elif UNITY_ANDROID
            return UtilsAndroid.GetAdvertisingId();
#else
            return "";
#endif
            }
        }

        public static string ToJsonString(Dictionary<string, string> data)
        {
            return "{" + string.Join(", ",
                data.Select(i => string.Format("\"{0}\" : \"{1}\"", i.Key, i.Value)).ToArray()) + "}";
        }

        static System.Action<int> alertCallback;

        public static void ShowSystemAlert(string title, string message, string okButton, string cancelButton = null,
            string moreButton = null, System.Action<int> callback = null)
        {
            if (alertCallback != null)
            {
                Debug.LogErrorFormat("Alert callback is not null");
            }

            alertCallback = callback;

#if UNITY_EDITOR
#elif UNITY_IPHONE
            Utils_showAlert (title, message, okButton, cancelButton, moreButton);
#elif UNITY_ANDROID
            UtilsAndroid.ShowSystemAlert(title, message, okButton, cancelButton, moreButton);
#endif
        }

        public static void ShowOfflineAlert()
        {
        }

        public static void OnNativeDialogClicked(int button)
        {
            if (alertCallback != null)
            {
                alertCallback.Invoke(button);
                alertCallback = null;
            }
        }

        public static void ShowRatePopup(string title = null, string message = null, string rateButton = null,
            string cancelButton = null)
        {
#if UNITY_EDITOR
#elif UNITY_IPHONE
        Utils_showRate ();
#elif UNITY_ANDROID
        ShowSystemAlert(title, message, rateButton, cancelButton, null, OnRateDialogClicked);
#endif
        }

        static void OnRateDialogClicked(int i)
        {
            if (i == 0)
            {
                StoreUtils.OpenStore(GetPackageName());
            }
        }

    }

#if UNITY_ANDROID
public class UtilsAndroid
{
    const string JavaClassName = "com.nopowerup.screw.utils.Utils";
    const string JavaStoreUtilsClassName = "com.nopowerup.screw.utils.StoreUtils";

    public static bool IsAppInstalled(string identifier)
    {
        // Java public static boolean isAppInstalled()
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            return jc.CallStatic<bool>("isAppInstalled", identifier);
        }
    }

    public static void OpenApp(string identifier)
    {
        // Java public static void OpenApp()
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            jc.CallStatic("openApp", identifier);
        }
    }

    public static void OpenFacebook(string pageId)
    {
        // Java public static void OpenFacebook(final String pageId, final String pageName)
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            jc.CallStatic("openFacebook", pageId);
        }
    }

    public static void OpenTwitter(string username)
    {
        // Java public static void OpenFacebook(final String pageId, final String pageName)
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            jc.CallStatic("openTwitter", username);
        }
    }

    public static void OpenInstagram(string username)
    {
        // Java public static void OpenFacebook(final String pageId, final String pageName)
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            jc.CallStatic("openInstagram", username);
        }
    }

    public static void FinishActivity()
    {
        // Java public static void finishActivity()
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            jc.CallStatic("finishActivity");
        }
    }

    public static void ShowToast(string message)
    {
        // Java public static void showToast(final String message)
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            jc.CallStatic("showToast", message);
        }
    }

    public static int GetAppVersionCode()
    {
        // Java public int getAppVersionCode ()
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            return jc.CallStatic<int>("getAppVersionCode");
        }
    }

    public static string GetAppVersionName()
    {
        // Java public String getAppVersionName ()
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            return jc.CallStatic<string>("getAppVersionName");
        }
    }

    public static void OpenDeveloperStore(string id)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaStoreUtilsClassName))
        {
            jc.CallStatic("openDeveloperStore", id);
        }
    }

    public static string GetParameterDeeplink()
    {
        // Java public String getAppVersionName ()
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            return jc.CallStatic<string>("getParameterDeeplink");
        }
    }

    public static string ScreenType()
    {
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            return jc.CallStatic<string>("screenType");
        }
    }

    public static bool IsTablet()
    {
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            return jc.CallStatic<bool>("isTablet");
        }
    }

    public static string GetAdvertisingId()
    {
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            return jc.CallStatic<string>("getAdvertisingId");
        }
    }

    public static void ShowSystemAlert(string title, string message, string okButton, string cancelButton, string moreButton)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            jc.CallStatic("showAlertDialog", title, message, okButton, cancelButton, moreButton);
        }
    }

    public static void ShowRatePopup(string title, string message, string rateButton, string cancelButton)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            jc.CallStatic("showRatePopup", title, message, rateButton, cancelButton);
        }
    }

    public static string GetPackageName()
    {

        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            return jc.CallStatic<string>("getPackageName");
        }
    }

}
#endif

}