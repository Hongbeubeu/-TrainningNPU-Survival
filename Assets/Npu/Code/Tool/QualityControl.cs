using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR

#endif

public class QualityControl : MonoBehaviour 
{
    public Text currentLevel;
    public Slider alphaSlider;

    void OnEnable ()
    {
        ShowCurrentQualityLevel();
        alphaSlider.value = GetComponent<Image>().color.a;

        LogShadow();

        var properties = Properties;
        var sms = properties.Aggregate("", (a, i) => a + string.Format("\t{0} ({1}) = {2}\n", i.Name, i.PropertyType, i.GetValue(null)));
        Debug.Log("Quality Settings Properties: \n" + sms);
    }

    public void ShowCurrentQualityLevel ()
    {
        var current = QualitySettings.GetQualityLevel();
        currentLevel.text = QualitySettings.names[current];

        GetComponentsInChildren<QualitySettingProperty>().ToList().ForEach(i => i.UpdateOptions());
    }

    public void IncreaseQualityLevel ()
    {
        QualitySettings.IncreaseLevel();
        ShowCurrentQualityLevel();
    }

    public void DecreaseQualityLevel()
    {
        QualitySettings.DecreaseLevel();
        ShowCurrentQualityLevel();
    }

    public static PropertyInfo[] Properties
    {
        get
        {
            var type = typeof(QualitySettings);
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetProperty).ToArray();
        }
    }

    public static PropertyInfo[] EnumProperties
    {
        get
        {
            var type = typeof(QualitySettings);
            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetProperty).Where(i => i.PropertyType.IsEnum).ToArray();
        }
    }

    public static PropertyInfo GetProperty(string name)
    {
        return Properties.FirstOrDefault(i => i.Name.Equals(name));
    }

    public void OnSliderChanged(float value)
    {
        var c = GetComponent<Image>().color;
        c.a = value;
        GetComponent<Image>().color = c;
    }

    public static void LogShadow ()
    {
        var msg = @"    QualitySettings.shadowmaskMode: {0},
            QualitySettings.shadows: {1},
            QualitySettings.shadowResolution: {2},
            QualitySettings.shadowProjection: {3},
            QualitySettings.shadowDistance: {4},
            QualitySettings.shadowCascades: {5},
            QualitySettings.shadowNearPlaneOffset: {6}";

        Debug.LogFormat(msg,
            QualitySettings.shadowmaskMode,
            QualitySettings.shadows,
            QualitySettings.shadowResolution,
            QualitySettings.shadowProjection,
            QualitySettings.shadowDistance,
            QualitySettings.shadowCascades,
            QualitySettings.shadowNearPlaneOffset
             );

        var lights = GameObject.FindObjectsOfType<Light>();
        foreach (var l in lights)
        {
            msg = @"    l.renderMode: {0},
                l.shadows: {1}, 
                l.shadowResolution: {2},
                l.shadowStrength: {3}";
            Debug.LogFormat(msg,
                l.renderMode,
                l.shadows, 
                l.shadowResolution,
                l.shadowStrength
                );
        }
    }
}




public class EnumPropertyDropdown : EnumDropdown
{
    PropertyInfo field;
    object target;

    public void SetUp (object target, PropertyInfo field)
    {
        if (!field.PropertyType.IsEnum)
        {
            throw new Exception(string.Format("{0} is not an enum field", field.Name));
        }

        this.target = target;
        this.field = field;

        SetUp(field.PropertyType);
    }

    public void OnValueChanged (int index)
    {

    }
}

public abstract class AbstractDropdown : MonoBehaviour
{
    public Dropdown dropdown;

    public abstract List<Dropdown.OptionData> Options { get; }

    public virtual void SetUp()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(Options);
    }
}

public class EnumDropdown : AbstractDropdown
{
    protected Type @enum;

    List<Dropdown.OptionData> options;
    public override List<Dropdown.OptionData> Options
    {
        get
        {
            if (options == null)
            {
                options = Enum.GetNames(@enum).Select(i => new Dropdown.OptionData(i)).ToList();
            }

            return options;
        }
    }

    public void SetUp(Type @enum)
    {
        this.@enum = @enum;
        base.SetUp();
    }
}