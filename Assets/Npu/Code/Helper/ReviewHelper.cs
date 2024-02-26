using System;
using System.Runtime.InteropServices;

#if UNITY_ANDROID && false
using Google.Play.Review;
#endif

namespace Npu.Helper
{
    public static class ReviewHelper
    {
        #if UNITY_IPHONE
        [DllImport ("__Internal")] private static extern void Utils_showRate ();
        #endif
        
        public static void ShowSystemReview(Action<bool> callback)
        {
#if UNITY_IPHONE            
            Utils_showRate();
            callback?.Invoke(true);
#endif
            
#if UNITY_ANDROID && false
            ShowAndroidReviewFlow(callback);
#endif
        }

#if UNITY_ANDROID && false
        private static ReviewManager _reviewManager;
        private static ReviewManager ReviewManager => _reviewManager ?? (_reviewManager = new ReviewManager());
        
        
        private static void ShowAndroidReviewFlow(Action<bool> callback)
        {
            Executor.Instance.StartCoroutine(DoAndroidReviewFlow(callback));
        }

        private static IEnumerator DoAndroidReviewFlow(Action<bool> callback)
        {
            var requestFlowOperation = ReviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Logger.Error(nameof(ReviewHelper), $"Failed to request flow: {requestFlowOperation.Error.ToString()}");
                callback?.Invoke(false);
                yield break;
            }
            var playReviewInfo = requestFlowOperation.GetResult();
            var launchFlowOperation = ReviewManager.LaunchReviewFlow(playReviewInfo);
            yield return launchFlowOperation;
            playReviewInfo = null; // Reset the object
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                Logger.Error(nameof(ReviewHelper), $"Failed to launch flow: {launchFlowOperation.Error.ToString()}");
                yield break;
            }
            callback?.Invoke(true);
        }
#endif        
    }
}