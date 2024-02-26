#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using Npu.Common;
using Object = UnityEngine.Object;
using SOW = Npu.EditorSupport.SceneOrganizerWindow;

/***
 * @authors: Thanh Le (William)  
 */

namespace Npu.EditorSupport
{
    public abstract class ASub
    {
        protected virtual bool DefaultOpened => false;

        protected virtual List<GameObject> SelectedGameObjects => SOW.SelectedGameObjects;
        protected virtual GameObject ActiveGameObject => SOW.ActiveGameObject;
        protected virtual bool NoObjects => SOW.NoObjects;
        protected virtual bool TooFewObjects => SOW.TooFewObjects;

        protected virtual bool IsActive(GameObject go) => SOW.IsActive(go);
        protected virtual void Edit(Object o) => SOW.Edit(o);

        protected void AxisButtons(string format, int sign, System.Action<Axis> action)
        {
            if (string.IsNullOrWhiteSpace(format)) format = "{0}";

            foreach (var ax in AxisUtils.Axes)
            {
                if (GUILayout.Button(string.Format(format, $"{ax}{(sign > 0 ? "+" : sign < 0 ? "-" : "")}")))
                {
                    action?.Invoke(ax);
                }
            }
        }

        protected virtual bool Section(string label, System.Action onGUI)
        {
            EditorGUIUtils.Section(label, onGUI, TextAnchor.MiddleLeft, $"SceneOrganizerSection_{label}");
            
            // bool defaultVal = DefaultOpened;
            //
            // string prefKey = $"SceneOrganizerSection_{label}";
            // bool isExpanded = EditorPrefs.GetBool(prefKey, defaultVal);
            //
            // bool labelPressed = EditorGUIUtils.MiniBoxedSection(label, () =>
            // {
            //     if (isExpanded) onGUI?.Invoke();
            // });
            //
            // if (labelPressed)
            // {
            //     EditorPrefs.SetBool(prefKey, isExpanded = !isExpanded);
            //     if (isExpanded == defaultVal) EditorPrefs.DeleteKey(prefKey);
            // }
            // return isExpanded;

            return true;
        }

        public enum BoundsStep { Min, Mid, Max }
        protected static BoundsStep[] BoundsSteps = new[] { BoundsStep.Min, BoundsStep.Mid, BoundsStep.Max, };

        public enum CenterMode { Origin, AvgPositions, BoundsCenter, ActiveObject, }

        public abstract void OnGUI();
    }
}

#endif