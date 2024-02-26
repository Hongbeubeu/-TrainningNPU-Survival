//#define ENABLED

#if ENABLED
using AppLovinMax.Scripts.IntegrationManager.Editor;
using AppsFlyerSDK;
using Facebook.Unity;
using GameAnalyticsSDK;
#endif

#if ENABLED
public class PluginVersions
{
    [MenuItem("Npu/Tools/Print Plugin Versions")]
    public static void Versions()
    { 
        var b = new StringBuilder();

        b.AppendFormat("GameAnalytics Unity: {0}", GameAnalyticsSDK.Setup.Settings.VERSION);
        b.AppendFormat("\nAppsflyer Unity: {0}", AppsFlyer.kAppsFlyerPluginVersion);
        b.AppendFormat("\nFacebook SDK Unity: {0}", FacebookSdkVersion.Build);
        
        var path = Path.Combine(BuildToolUtils.ProjectPath, "Assets/Firebase/m2repository/com/google/firebase/firebase-analytics-unity");
        if (Directory.Exists(path))
        {
            foreach (var i in Directory.EnumerateDirectories(path))
            {
                b.AppendFormat("\nFirebase Unity: {0}", Path.GetFileName(i));
            }
        }

        AppLovinEditorCoroutine.StartCoroutine(AppLovinIntegrationManager.Instance.LoadPluginData(data =>
        {
            if (data != null) PrintMaxVersions(data, b);
            else Debug.LogError("Failed to load MAX Plugin Data");
        }));
    }
    
    private static void PrintMaxVersions(PluginData data, StringBuilder b)
    {
        var max = data.AppLovinMax;
        b.Append("\n\nMAX");
        b.AppendFormat("\n\tUnity: {0}", max.CurrentVersions.Unity);
        b.AppendFormat("\n\tiOS: {0}", max.CurrentVersions.Ios);
        b.AppendFormat("\n\tAndroid: {0}", max.CurrentVersions.Android);

        b.Append("\n\nMAX Adapters");
        foreach (var network in data.MediatedNetworks.Where(i => !string.IsNullOrEmpty(i.CurrentVersions.Unity)))
        {
            b.AppendFormat("\n\t{0}: Unity {1}, iOS {2}, Android {3}", 
                network.DisplayName, 
                network.CurrentVersions.Unity, 
                network.CurrentVersions.Ios,
                network.CurrentVersions.Android);
        }
        
        
        Debug.Log(b.ToString());
    }
    

}

#endif
