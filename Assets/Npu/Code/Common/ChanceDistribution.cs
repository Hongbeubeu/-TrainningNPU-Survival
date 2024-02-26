using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ChanceDistribution<T> where T : System.IConvertible
{
    public bool fulfill;
    public List<T> keys = new List<T>();
    public List<float> values = new List<float>();

    public T GetRandom(T defaultValue = default(T))
    {
        return Evaluate(Random.value, defaultValue);
    }

    public T Evaluate(float t, T defaultValue = default(T))
    {
        float t0 = 0;
        for (var i = 0; i < Mathf.Min(keys.Count, values.Count); i++)
        {
            t0 += values[i];
            if (t0 >= t) return keys[i];
        }
        if (fulfill && keys.Count >= 1) return keys[keys.Count - 1];
        return defaultValue;
    }
}


#if UNITY_EDITOR
public class ChanceDistributionDrawer<T> : PropertyDrawer
{
    int resizingIndex = -1;
    Vector2 resizeMousePrev;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var rowHeight = base.GetPropertyHeight(property, label);
        var rows = property.isExpanded ? 4 : 1;
        return rowHeight * rows;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var rows = property.isExpanded ? 4 : 1;
        var rowHeight = position.height / rows;
        var r = new Rect(position) { height = rowHeight };
        r.width -= 5;

        property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, label, true);
        if (property.isExpanded)
        {
            ValidateProp(property);

            EditorGUI.indentLevel++;
            r.position += new Vector2(0, rowHeight);

            var fulfillProp = property.FindPropertyRelative("fulfill");
            EditorGUI.PropertyField(r, fulfillProp);

            r.position += new Vector2(0, rowHeight);
            r.height *= 2;
            var keys = property.FindPropertyRelative("keys");
            var values = property.FindPropertyRelative("values");
            DoElements(r, keys, values, fulfillProp.boolValue);


            r.position += new Vector2(0, r.height);
            r.height = rowHeight;
            if (GUI.Button(EditorGUI.IndentedRect(r), "Even distribution"))
            {
                Evenly(property.FindPropertyRelative("values"));
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    void DoElements(Rect rect, SerializedProperty keys, SerializedProperty values, bool fulfill)
    {
        var ec = Event.current;
        var mousePos = ec.mousePosition;
        //mousePos.y = Screen.height - mousePos.y;

        var count = Mathf.Min(keys.arraySize, values.arraySize);
        float totalValues = 1;

        var enumNames = keys.enumDisplayNames;
        var r = EditorGUI.IndentedRect(rect);
        GUI.Box(r, "", EditorStyles.helpBox);

        var xPos = r.xMin;
        float accuVal = 0;
        float resizingValue = 0;
        for (var i = 0; i < count; i++)
        {
            var val = values.GetArrayElementAtIndex(i).floatValue;
            if (i == count - 1 && fulfill) val = 1 - accuVal;
            accuVal += val;
            var aWidth = val / totalValues * r.width;
            var aRect = new Rect(r);
            aRect.xMin = xPos;
            aRect.width = aWidth;
            xPos += aWidth;
            GUI.Box(aRect, "");

            var aRectName = new Rect(aRect) { height = aRect.height / 2f };
            var aRectValue = new Rect(aRectName);
            aRectValue.position += new Vector2(0, aRectName.height);
            aRectValue.xMin += 5;
            aRectValue.xMax -= 5;

            var enumIndex = keys.GetArrayElementAtIndex(i).enumValueIndex;
            var name = enumIndex >= 0 && enumIndex < enumNames.Length ? enumNames[enumIndex] : "enum";
            var text = name + "\n" + (val * 100).ToString("N1") + "%";

            var style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(aRectName, name, style);
            var newVal = val;
            if (i < count - 1)
            {
                newVal = EditorGUI.DelayedFloatField(aRectValue, GUIContent.none, val, style);
            }
            else
            {
                EditorGUI.LabelField(aRectValue, val.ToString(), style);
            }
            if (newVal != val) ModifyChance(values, i, newVal - val, fulfill);

            if (i == count - 1 && fulfill)
            {
                // no resizing
            }
            else
            {

                var cursorRect = new Rect(aRect);
                cursorRect.xMin = cursorRect.xMax - 4;
                cursorRect.width = 8;
                EditorGUIUtility.AddCursorRect(cursorRect, MouseCursor.SplitResizeLeftRight);
                if (ec.type == EventType.MouseDown && cursorRect.Contains(mousePos))
                {
                    resizingIndex = i;
                    resizeMousePrev = mousePos;
                }
                else if (ec.type == EventType.MouseUp)
                {
                    resizingIndex = -1;
                }

                if (resizingIndex == i && ec.type == EventType.MouseDrag)
                {
                    var xMove = mousePos.x - resizeMousePrev.x;
                    xMove = xMove / r.width * totalValues;
                    resizingValue = xMove;
                    resizeMousePrev = mousePos;
                }
            }
        }

        if (resizingValue != 0)
        {
            ModifyChance(values, resizingIndex, resizingValue, fulfill);
            EditorUtility.SetDirty(values.serializedObject.targetObject);
        }
    }

    void ModifyChance(SerializedProperty values, int index, float change, bool fulfill)
    {
        var totalSum = SumFloat(values);
        var accuSum = SumFloat(values, index + 1);
        var count = values.arraySize;
        var prop = values.GetArrayElementAtIndex(index);
        if (change > 0)
        {
            float allowedChange = 0;
            var nextProp = values.GetArrayElementAtIndex(index);
            for (var i = index + 1; i < count; i++)
            {
                nextProp.Next(false);
                var val = nextProp.floatValue;
                var allowingChange = Mathf.Min(val, change - allowedChange);

                nextProp.floatValue -= allowingChange;
                allowedChange += allowingChange;
            }
            allowedChange += Mathf.Min(change - allowedChange, 1 - totalSum);
            prop.floatValue += allowedChange;
        }
        else if (change < 0)
        {
            if (index >= count - 1 && fulfill) change = 0;
            else
            {
                change = Mathf.Min(accuSum, Mathf.Abs(change));

                float allowedChange = 0;
                for (var i = index; i >= 0; i--)
                {
                    var aProp = values.GetArrayElementAtIndex(i);
                    var val = aProp.floatValue;
                    var allowingChange = Mathf.Min(val, change - allowedChange);

                    aProp.floatValue -= allowingChange;
                    allowedChange += allowingChange;
                }

                if (index < count - 1)
                {
                    var nextProp = values.GetArrayElementAtIndex(index + 1);
                    nextProp.floatValue += change;
                }
            }
        }

    }

    float SumFloat(SerializedProperty props, int maxCount = -1)
    {
        float s = 0;
        for (var i = 0; i < (maxCount >= 0 ? maxCount : props.arraySize); i++)
        {
            s += props.GetArrayElementAtIndex(i).floatValue;
        }
        return s;
    }

    void ValidateProp(SerializedProperty prop)
    {
        var keys = prop.FindPropertyRelative("keys");
        var values = prop.FindPropertyRelative("values");

        for (var i = 0; i < keys.enumNames.Length; i++)
        {
            var has = false;
            for (var j = 0; j < keys.arraySize; j++)
            {
                if (keys.GetArrayElementAtIndex(j).enumValueIndex == i)
                {
                    has = true;
                    break;
                }
            }
            if (!has)
            {
                keys.arraySize++;
                keys.GetArrayElementAtIndex(keys.arraySize - 1).enumValueIndex = i;
            }
        }

        var anyPositiveValue = false;
        var valuesCount = values.arraySize;
        for (var i = 0; i < valuesCount; i++)
        {
            var valueProp = values.GetArrayElementAtIndex(i);
            anyPositiveValue |= valueProp.floatValue > 0;
            if (i == valuesCount - 1 && !anyPositiveValue) valueProp.floatValue = 1;
        }
        for (var i = values.arraySize; i < keys.arraySize; i++)
        {
            values.arraySize++;
            values.GetArrayElementAtIndex(i).floatValue = 0;
        }
    }

    void Evenly(SerializedProperty values)
    {
        var count = values.arraySize;
        if (count > 0)
        {
            var valueEach = 1f / count;
            var firstElement = values.GetArrayElementAtIndex(0);
            firstElement.floatValue = valueEach;
            for (var i = 1; i < count; i++)
            {
                firstElement.Next(false);
                firstElement.floatValue = valueEach;
            }
        }
    }
}
#endif