// #define USE_PACKAGE

#if USE_PACKAGE
#if UNITY_IOS
using Unity.Notifications.iOS;
#else
using Unity.Notifications.Android;
#endif
#endif

using System;
using UnityEngine;

#if UNITY_IOS
using NotificationServices = UnityEngine.iOS.NotificationServices;
using LocalNotification = UnityEngine.iOS.LocalNotification;
#endif

namespace Npu
{
    public class Notifications
    {
        public static void Register(bool forRemote = false)
        {
#if UNITY_IOS
        NotificationServices.RegisterForNotifications(
            UnityEngine.iOS.NotificationType.Alert | UnityEngine.iOS.NotificationType.Badge | UnityEngine.iOS.NotificationType.Sound,
            forRemote);
#endif
        }

        public static void Schedule(int code, long seconds, string message, Repeat repeat = Repeat.None)
        {
#if UNITY_EDITOR
            return;
#endif

#if UNITY_IOS
        var notif = new LocalNotification();
		notif.fireDate = DateTime.Now.AddSeconds(seconds);
		notif.alertBody = message;
		notif.applicationIconBadgeNumber = 1;
        switch (repeat)
        {
            case Repeat.Minute:
                notif.repeatInterval = UnityEngine.iOS.CalendarUnit.Minute;
                break;

            case Repeat.Hour:
                notif.repeatInterval = UnityEngine.iOS.CalendarUnit.Hour;
                break;

            case Repeat.Week:
                notif.repeatInterval = UnityEngine.iOS.CalendarUnit.Week;
                break;

            case Repeat.Year:
                notif.repeatInterval = UnityEngine.iOS.CalendarUnit.Year;
                break;
        }

		NotificationServices.ScheduleLocalNotification(notif);
#elif UNITY_ANDROID
            NotificationAndroid.Schedule(code, (int) seconds, "", message, "noti_forklift_small", "notification_large");
#endif
        }

        public static void Clear(int code)
        {
#if UNITY_EDITOR
            return;
#endif

#if UNITY_IOS
		NotificationServices.CancelAllLocalNotifications();
		NotificationServices.ClearLocalNotifications();
#elif UNITY_ANDROID
            NotificationAndroid.Cancel(code);
#endif
        }

        public enum Repeat
        {
            None,
            Minute,
            Hour,
            Day,
            Week,
            Year
        }
    }

#if UNITY_ANDROID
    class NotificationAndroid
    {
#if USE_PACKAGE
    public const string ChannelID = "update_reminder";
    public const string ChannelName = "Updates & Reminders";
    public const string ChannelDescription = "Updates & Reminders";

    static void RegisterChannel()
    {
        var c = new AndroidNotificationChannel()
        {
            Id = ChannelID,
            Name = ChannelName,
            Importance = Importance.High,
            Description = ChannelDescription
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);
    }

    public static int Schedule(int code, int seconds, string title, string message, string iconName, string largeIconName)
    {
        RegisterChannel();

        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = message;
        notification.FireTime = System.DateTime.Now.AddSeconds(seconds);
        

        return AndroidNotificationCenter.SendNotification(notification, ChannelID);
    }

     public static void Cancel(int code)
     {
        //  AndroidNotificationCenter.CancelNotification(code);
        CancelAlll();
     }

     public static void CancelAlll()
     {
         AndroidNotificationCenter.CancelAllNotifications();
     }
#else


        const string JavaClassName = "com.nopowerup.screw.notification.Notification";

        public static void Schedule(int code, int seconds, string title, string message, string iconName,
            string largeIconName)
        {
            // Java public static void schedule (int code, int seconds, String title, String message, String iconName)
            using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
            {
                jc.CallStatic("schedule", code, seconds, title, message, iconName, largeIconName);
            }
        }

        public static void Cancel(int code)
        {
            // Java public static void cancel (int code)
            using (AndroidJavaClass jc = new AndroidJavaClass(JavaClassName))
            {
                jc.CallStatic("cancel", code);
            }
        }


#endif
    }
#endif
}