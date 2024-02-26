using System;
using Npu.Common;
using UnityEngine;
using Npu.Helper;

namespace Npu
{
    public static partial class Settings
    {
        private const string KeyMusicVolume = "__music_volume__";
        private const string KeySfxVolume = "__sfx_volume__";
        private const string KeySfxEnabled = "__sfx_enabled__";
        private const string KeySfx3DEnabled = "__sfx_3d_enabled__";
        private const string KeyMusicEnabled = "__music_enabled__";
        private const string KeyVibrateEnabled = "__vibrate_enabled__";
        private const string KeyNotificationEnabled = "__notification_enabled__";
        private const string KeyGameLaunchCount = "__launch_count__";
        private const string KeyCurrentVersion = "__current_version__";
        private const string KeyRated = "__current_rated_";
        private const string KeyShowRatePopup = "__current_show_rate_popup__";
        private const string KeyMaxMilkingStagePlayed = "__max_milking_stage_played__";
        private const string HasReadTOS = "__TOS_read_";
        private const string HasRequestDeleteAccount = "__delete_acc_requested_";

        public static void Initialize()
        {
            if (FirstInstalledVersion < 0) FirstInstalledVersion = Utils.GetAppVersionCode();
            PreviousVersion = CurrentVersion;
            CurrentVersion = Utils.GetAppVersionCode();
            // Save install time if needed
            SaveInstallTimeIfNeeded();
            LaunchCount++;
            PlayerPrefs.Save();
        }

        public static int PreviousVersion { get; private set; }
        public static int CurrentVersion 
        { 
            get => PlayerPrefs.GetInt(KeyCurrentVersion, -1); 
            private set => PlayerPrefs.SetInt(KeyCurrentVersion, value);
        }
        
        private static readonly PlayerPrefsValue<int> _firstInstalledVersion = new PlayerPrefsValue<int>("_first_installed_", PlayerPrefs.GetInt, PlayerPrefs.SetInt, -1);
        public static int FirstInstalledVersion
        {
            get => _firstInstalledVersion.Value;
            private set => _firstInstalledVersion.Value = value;
        }
        
        public static bool GameUpdated => PreviousVersion > 0 && PreviousVersion != CurrentVersion;
        public static bool GameFirstLaunch => PreviousVersion < 0;

        public static int LaunchCount 
        {
            get => PlayerPrefs.GetInt(KeyGameLaunchCount, 0);
            private set => PlayerPrefs.SetInt(KeyGameLaunchCount, value);
        }

        public static int VersionCode => Utils.GetAppVersionCode();
        public static string VersionName => Utils.GetAppVersionName();
        public static string VersionString => $"{VersionName}-{VersionCode}";
        public static string Os
        {
            get
            {
#if UNITY_ANDROID
                return "android";
#else
                return "ios";
#endif
            }
        }
        
        public static string DeviceId => SystemInfo.deviceUniqueIdentifier;

        public static string DeviceModel => SystemInfo.deviceModel;
        
        private static void SaveInstallTimeIfNeeded()
        {
            if (InstallTimeTicks <= 0) InstallTimeTicks = TimeUtils.CurrentTicks;
        }
        
        private static long? installTimeTicks;
        private static PlayerPrefsValue<long> _installTime = new PlayerPrefsValue<long>("AnalyticsInstallTime", PrefsGetLong, PrefsSetLong, -1);

        public static long InstallTimeTicks
        {
            get => _installTime.Value;
            private set => _installTime.Value = value;
        }

        public static DateTime InstallTime => TimeUtils.TicksToDateTime(InstallTimeTicks);

        public static float HoursSinceInstall => (float) TimeUtils.TimespanHours(TimeUtils.CurrentTicks, InstallTimeTicks);

        public static float DaysSinceInstall =>
            (float) TimeUtils.TimespanDays(TimeUtils.CurrentTicks, InstallTimeTicks);
        
        public static bool PrefsGetBool(string key, bool defaultValue = false)
        {
            var val = PlayerPrefs.GetString(key, defaultValue ? "1" : "0");
            return int.Parse(val) > 0;
        }

        public static void PrefsSetBool(string key, bool value)
        {
            PlayerPrefs.SetString(key, value ? "1" : "0");
        }

        public static long PrefsGetLong(string key, long defaultValue = 0)
        {
            var val = PlayerPrefs.GetString(key, defaultValue.ToString());
            return long.TryParse(val, out var value) ? value : defaultValue;
        }

        public static void PrefsSetLong(string key, long value)
        {
            PlayerPrefs.SetString(key, value.ToString());
        }

        public static double PrefsGetDouble(string key, double defaultValue=0)
        {
            var val = PlayerPrefs.GetString(key, defaultValue.ToString("R"));
            return double.TryParse(val, out var value) ? value : defaultValue;
        }

        public static void PrefsSetDouble(string key, double value)
        {
            PlayerPrefs.SetString(key, value.ToString("R"));
        }

        public static Vector3 PrefsGetVector3(string key)
        {
            var s = PlayerPrefs.GetString(key, "");

            return s.ToVector3();
        }

        public static void PrefsSetVector3(string key, Vector3 point)
        {
            PlayerPrefs.SetString(key, point.ToSerializeString());
        }

        public static PlayerPrefsValue<bool> Rated { get; } = new PlayerPrefsValue<bool>(
            KeyRated, PrefsGetBool, PrefsSetBool, false);
        
        public static PlayerPrefsValue<bool> ShowRatePopup { get; } = new PlayerPrefsValue<bool>(
            KeyShowRatePopup, PrefsGetBool, PrefsSetBool, false);
        
        public static PlayerPrefsValue<int> MaxMilkingStagePlayed { get;} = new PlayerPrefsValue<int>(
            KeyMaxMilkingStagePlayed, PlayerPrefs.GetInt, PlayerPrefs.SetInt, 0);
        
        
        // public static bool Rated
        // {
        //     get => UniversalSettings.Get<bool>(KeyRated);
        //     set => UniversalSettings.Set(KeyRated, value);
        // }
        
        // public static bool ShowRatePopup
        // {
        //     get => UniversalSettings.Get<bool>(KeyShowRatePopup);
        //     set => UniversalSettings.Set(KeyShowRatePopup, value);
        // }
        

        public static PlayerPrefsValue<float> MusicVolume { get; } = new PlayerPrefsValue<float>(
            KeyMusicVolume, PlayerPrefs.GetFloat, PlayerPrefs.SetFloat, 1.0f);
        
        public static PlayerPrefsValue<float> SfxVolume { get;  } = new PlayerPrefsValue<float>(
            KeySfxVolume, PlayerPrefs.GetFloat, PlayerPrefs.SetFloat, 1.0f);
        
        public static PlayerPrefsValue<bool> MusicEnabled { get; } = new PlayerPrefsValue<bool>(
            KeyMusicEnabled, PrefsGetBool, PrefsSetBool, true);
        
        public static PlayerPrefsValue<bool> SfxEnabled { get; } = new PlayerPrefsValue<bool>(
            KeySfxEnabled, PrefsGetBool, PrefsSetBool, true);
        
        public static PlayerPrefsValue<bool> Sfx3DEnabled { get; } = new PlayerPrefsValue<bool>(
            KeySfx3DEnabled, PrefsGetBool, PrefsSetBool, true);
        
        public static PlayerPrefsValue<bool> VibrateEnabled { get; } = new PlayerPrefsValue<bool>(
            KeyVibrateEnabled, PrefsGetBool, PrefsSetBool, true);

        public static PlayerPrefsValue<bool> NotificationEnabled { get; } = new PlayerPrefsValue<bool>(
            KeyNotificationEnabled, PrefsGetBool, PrefsSetBool, true);
        
        public static PlayerPrefsValue<bool> TOSRead { get; } = new PlayerPrefsValue<bool>(
            HasReadTOS, PrefsGetBool, PrefsSetBool, false);
        
        public static PlayerPrefsValue<bool> DeleteAccRequested { get; } = new PlayerPrefsValue<bool>(
            HasRequestDeleteAccount, PrefsGetBool, PrefsSetBool, false);
        
        public static bool Cheated 
        {
            get => SecuredPrefsGetBool("__cheat_is_bad__");
            set => SecuredPrefsSetBool("__cheat_is_bad__", value);
        }
    }
}
