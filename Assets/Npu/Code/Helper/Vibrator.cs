using UnityEngine;

#if UNITY_IPHONE

#endif

namespace Npu.Helper
{

    public partial class Vibrator
    {
#if UNITY_IPHONE && !UNITY_EDITOR
        [DllImport ("__Internal")]
        private static extern int Vibrator_supportLevel();

        [DllImport ("__Internal")]
        private static extern void Vibrator_vibrate();

        [DllImport ("__Internal")]
        private static extern void Vibrator_alert();

        [DllImport ("__Internal")]
        private static extern bool Vibrator_tapticPeek();

        [DllImport ("__Internal")]
        private static extern bool Vibrator_tapticPop();

        [DllImport ("__Internal")]
        private static extern bool Vibrator_tapticCancelled();

        [DllImport ("__Internal")]
        private static extern bool Vibrator_tapticTryAgain();

        [DllImport ("__Internal")]
        private static extern bool Vibrator_hapticNotification(int type); // 0 - Success, 1 - Warning, 2 - Error

        [DllImport ("__Internal")]
        private static extern bool Vibrator_hapticImpactLight();

        [DllImport ("__Internal")]
        private static extern bool Vibrator_hapticImpactMedium();

        [DllImport ("__Internal")]
        private static extern bool Vibrator_hapticImpactHeavy();

        [DllImport ("__Internal")]
        private static extern bool Vibrator_hapticSelection();
#endif

        static int? _SupportLevel;

        public static int SupportLevel
        {
            get
            {
#if UNITY_IPHONE && !UNITY_EDITOR
            if (_SupportLevel == null)
            {
                _SupportLevel = Vibrator_supportLevel();
            }
            return _SupportLevel.Value;
#endif
                return 0;
            }
        }

        public static void Select()
        {
#if UNITY_EDITOR
            Debug.Log("Vibrator.Select");
#elif UNITY_IPHONE
        if (SupportLevel >= 2)
        {
            Vibrator_hapticSelection();
        }
        else
        {
            Vibrator_tapticPeek();
        }
#elif UNITY_ANDROID
            VibratorAndroid.Vibrate(50);
#endif
        }

        public static void Peek()
        {
#if UNITY_EDITOR
            Debug.Log("Vibrator.Peek");
#elif UNITY_IOS
        if (SupportLevel >= 2)
        {
            Vibrator_hapticImpactMedium();
        }
        else
        {
            Vibrator_tapticPeek();
        }
#elif UNITY_ANDROID
        VibratorAndroid.Vibrate(45);
#endif
        }

        public static void Pop()
        {
#if UNITY_EDITOR
            Debug.Log("Vibrator.Pop");
#elif UNITY_IPHONE
        if (SupportLevel >= 2)
        {
            Vibrator_hapticImpactLight();
        }
        else
        {
            Vibrator_tapticPop();
        }
#elif UNITY_ANDROID
        VibratorAndroid.Vibrate(75);
#endif
        }

        public static void Success()
        {
#if UNITY_EDITOR
            Debug.Log("Vibrator.Success");
#elif UNITY_IOS
        if (SupportLevel >= 2)
        {
            Vibrator_tapticCancelled();
        }
        else
        {
            Vibrator_tapticPeek();
        }
#elif UNITY_ANDROID
        VibratorAndroid.Vibrate(45);
#endif
        }

        public static void Cancel()
        {
#if UNITY_EDITOR
            Debug.Log("Vibrator.Cancel");
#elif UNITY_IOS
        if (SupportLevel >= 2)
        {
            Vibrator_hapticNotification(1);
        }
        else
        {
            Vibrator_tapticTryAgain();
        }
#elif UNITY_ANDROID
         VibratorAndroid.Vibrate(60);
#endif
        }

        public static void Fail()
        {
#if UNITY_EDITOR
            Debug.Log("Vibrator.Fail ");
#elif UNITY_IOS
        if (SupportLevel >= 2)
        {
            Vibrator_hapticNotification(2);
        }
        else
        {
            Vibrator_tapticCancelled();
        }
#elif UNITY_ANDROID
         VibratorAndroid.Vibrate(75);
#endif
        }
    }

#if UNITY_ANDROID
class VibratorAndroid
{
    const string JavaClassName = "com.nopowerup.screw.utils.Vibrator";

    public static void Vibrate (long milliseconds)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            jc.CallStatic("vibrate", milliseconds);
        }
    }

    public static void Vibrate(long milliseconds, int amplitude)
    {
        using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
        {
            jc.CallStatic("vibrate", milliseconds, amplitude);
        }
    }
}
#endif

}