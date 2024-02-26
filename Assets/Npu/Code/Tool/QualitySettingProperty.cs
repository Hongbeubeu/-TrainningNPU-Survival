using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;

#endif

public class QualitySettingProperty : MonoBehaviour
{
    [HideInInspector] public Dropdown dropdown;
    [HideInInspector] public InputField inputfield;
    [HideInInspector] public string propertyName;
    [HideInInspector] public bool options;
    [HideInInspector] public Toggle toggle;
    public int[] optionValues = new int[0];

    PropertyInfo property;
    public PropertyInfo Property
    {
        get 
        { 
            if (property == null) property = QualityControl.GetProperty(propertyName);
            if (property == null) Debug.LogErrorFormat("No property named '{0}'", propertyName);
            return property;
        }
    }

    public Selectable Control
    {
        get; private set;
    }

    private void OnEnable()
    {
        UpdateOptions();
    }

    public void UpdateOptions ()
    {
        if (Property == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // If enum field
        if (Property.PropertyType.IsEnum)
        {
            if (dropdown == null)
            {
                Debug.LogErrorFormat("No dropdown {0} ({1})", gameObject.name, property.Name);
                return;
            }
            inputfield?.gameObject?.SetActive(false);
            toggle?.gameObject?.SetActive(false);

            dropdown.gameObject.SetActive(true);
            dropdown.ClearOptions();
            dropdown.AddOptions(Enum.GetNames(Property.PropertyType).ToList());
            Control = dropdown;

            var current = Property.GetValue(null);
            var values = Enum.GetValues(Property.PropertyType);
            var index = Array.IndexOf(values, current);
            dropdown.value = index;
        }

        // If int field
        if (Property.PropertyType == typeof(int) || Property.PropertyType == typeof(long) 
        || Property.PropertyType == typeof(float) || Property.PropertyType == typeof(double))
        {
            if (options)
            {
                if (dropdown == null)
                {
                    Debug.LogErrorFormat("No dropdown {0} ({1})", gameObject.name, property.Name);
                    return;
                }

                inputfield?.gameObject?.SetActive(false);
                toggle?.gameObject?.SetActive(false);
                dropdown.gameObject.SetActive(true);
                Control = dropdown;

                var current = (int) property.GetValue(null);
                dropdown.ClearOptions();
                dropdown.AddOptions(optionValues.Select(i => i.ToString()).ToList());
                dropdown.value = Array.IndexOf(optionValues, current);
            }
            else
            {
                if (inputfield == null)
                {
                    Debug.LogErrorFormat("No input field {0} ({1})", gameObject.name, property.Name);
                    return;
                }

                toggle?.gameObject?.SetActive(false);
                dropdown?.gameObject?.SetActive(false);
                inputfield.gameObject.SetActive(true);
                inputfield.text = property.GetValue(null).ToString();
                Control = inputfield;
            }
        }

        if (Property.PropertyType == typeof(bool))
        {
            dropdown?.gameObject?.SetActive(false);
            inputfield?.gameObject?.SetActive(false);

            if (toggle == null)
            {
                Debug.LogErrorFormat("No toogle {0} ({1})", gameObject.name, property.Name);
                return;
            }

            toggle.gameObject.SetActive(true);
            toggle.isOn = (bool)property.GetValue(null);
            Control = toggle;
        }

        if (Control != null && Property.SetMethod == null) Control.interactable = false;
    }

    public void OnDropdownChanged (int index)
    {
        if (Property != null && index >= 0)
        {
            var selected = 0;
            if (property.PropertyType.IsEnum)
            {

                if (property.SetMethod != null)
                {
                    var values = Enum.GetValues(Property.PropertyType);
                    selected = (int)values.GetValue(index);
                    Property.SetValue(null, selected);
                    Debug.LogFormat("QualitySettings set {0} to {1}", property.Name, selected);
                }
                else
                {
                    Debug.LogErrorFormat("No set accessor for property {0}", property.Name);
                }
            }
            else if (options && optionValues != null)
            {

                if (property.SetMethod != null)
                {
                    selected = optionValues[index];
                    Property.SetValue(null, selected);
                    Debug.LogFormat("QualitySettings set {0} to {1}", property.Name, selected);
                }
                else
                {
                    Debug.LogErrorFormat("No set accessor for property {0}", property.Name);
                }
            }
        }
    }

    public void OnInputFieldEndEdit (string text)
    {
        if (Property == null) return;

        if (!options && (Property.PropertyType == typeof(int) || Property.PropertyType == typeof(long)))
        {
            try
            {

                if (property.SetMethod != null)
                {
                    var value = int.Parse(text);
                    property.SetValue(null, value);
                    Debug.LogFormat("QualitySettings set {0} to {1}", property.Name, value);
                }
                else
                {
                    Debug.LogErrorFormat("No set accessor for property {0}", property.Name);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        if (Property.PropertyType == typeof(float) || Property.PropertyType == typeof(double))
        {
            try
            {
                if (property.SetMethod != null)
                {
                    var value = float.Parse(text);
                    property.SetValue(null, value);
                    Debug.LogFormat("QualitySettings set {0} to {1}", property.Name, value);
                }
                else
                {
                    Debug.LogErrorFormat("No set accessor for property {0}", property.Name);
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }

    public void OnToggleChanged (bool value)
    {
        if (Property == null) return;

        if (Property.PropertyType == typeof(bool))
        {
            try
            {
                if (property.SetMethod != null)
                {
                    property.SetValue(null, value);
                    Debug.LogFormat("QualitySettings set {0} to {1}", property.Name, value);
                }
                else
                {
                    Debug.LogErrorFormat("No set accessor for property {0}", property.Name);
                }

            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(QualitySettingProperty))]
public class QualitySettingPropertyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        var dropdown = target as QualitySettingProperty;
        var options = QualityControl.Properties.Select(i => i.Name).ToArray();
        var selected = Array.IndexOf(options, dropdown.propertyName);
        var selection = EditorGUILayout.Popup("Property", selected, options);
        if (selected != selection)
        {
            dropdown.propertyName = options[selection];
            EditorUtility.SetDirty(dropdown);

            if (Application.isPlaying)
                dropdown.UpdateOptions();
        }

        var p = QualityControl.GetProperty(dropdown.propertyName);
        if (p == null) return;

        serializedObject.Update();

        if (p.PropertyType.IsEnum)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dropdown"));
            return;
        }
        
        if (p.PropertyType == typeof(int))
        {
            if (dropdown.options)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dropdown"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("options"));

            if (dropdown.options)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("optionValues"), new GUIContent("Values"), true);
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("inputfield"), new GUIContent("Input"));
            }
        }

        if (p.PropertyType == typeof(float) || p.PropertyType == typeof(double))
        {
            dropdown.options = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("inputfield"), new GUIContent("Input"));
        }

        if (p.PropertyType == typeof(bool))
        {
            dropdown.options = false;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("toggle"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif