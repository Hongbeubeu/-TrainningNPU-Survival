using System.Collections.Generic;
using System.Linq;
using Npu.Core;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Npu.Formula
{
    [CreateAssetMenu(fileName = "StepFormula", menuName = "Formula/StepFormula", order = 0)]
    public class StepFormula : AbstractFormula
    {
        [SerializeField] [HideInInspector] internal List<Step> steps = new List<Step>();
        [SerializeField] EvaluationMode evaluationMode = EvaluationMode.Single;

        IEnumerable<Step> OrderedSteps => steps.OrderBy(s => s.level);

        public override SecuredDouble Evaluate(int n)
        {
            if (evaluationMode == EvaluationMode.Single)
            {
                return GetStep(n)?.value ?? 0;
            }
            else if (evaluationMode == EvaluationMode.Sum)
            {
                return GetSum(0, n, false);
            }
            else if (evaluationMode == EvaluationMode.Product)
            {
                return GetProduct(0, n, false);
            }
            return 0;
        }

        protected Step GetStep(int n)
        {
            return OrderedSteps.LastOrDefault(s => s.level <= n);
        }

        public override SecuredDouble AggregatedValue(int n0, int count)
        {
            if (evaluationMode == EvaluationMode.Single)
            {
                return GetSum(n0, n0 + count, false);
            }
            return base.AggregatedValue(n0, count);
        }

        // 400 times faster than brute
        SecuredDouble GetSum(int from, int to, bool atStepsOnly)
        {
            if (atStepsOnly)
            {
                return steps.Where(s => s.level >= from && s.level <= to).Select(s => s.value.Value).DefaultIfEmpty(0).Sum();
            }
            else
            {
                SecuredDouble d = 0;

                var properSteps = steps.Where(s => s.level >= from && s.level <= to).OrderBy(s => s.level);

                var lastStep = GetStep(from);
                foreach (var st in properSteps)
                {
                    if (lastStep != null) d += lastStep.value * (st.level - Mathf.Max(lastStep.level, from));
                    lastStep = st;
                }
                if (lastStep != null) d += lastStep.value * (to - lastStep.level + 1);
                return d;
            }
        }

        // 400 times faster than brute
        SecuredDouble GetProduct(int from, int to, bool atStepsOnly)
        {
            if (atStepsOnly)
            {
                return steps.Where(s => s.level >= from && s.level <= to).Select(s => s.value.Value).DefaultIfEmpty(1).Aggregate((d1, d2) => d1 * d2);
            }
            else
            {
                SecuredDouble d = 1;

                var properSteps = steps.Where(s => s.level >= from && s.level <= to).OrderBy(s => s.level);

                var lastStep = GetStep(from);
                foreach (var st in properSteps)
                {
                    if (lastStep != null) d *= System.Math.Pow(lastStep.value, (st.level - Mathf.Max(lastStep.level, from)));
                    lastStep = st;
                }
                if (lastStep != null) d *= System.Math.Pow(lastStep.value, (to - lastStep.level + 1));
                return d;
            }
        }

        //SecuredDouble GetProduct(int n)
        //{
        //    var acc = new SecuredDouble(1f);

        //    for (int i = 0; i <= n; i++)
        //    {
        //        acc *= GetStep(i)?.value ?? 1f;
        //    }

        //    return acc;
        //}

        [System.Serializable]
        public class Step
        {
            public int level;
            public SecuredDouble value;
        }

        public enum EvaluationMode
        {
            Single,
            Sum,
            Product,
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(StepFormula), true)]
    [CanEditMultipleObjects]
    public class StepFormulaEditor : Editor
    {
        static HashSet<int> badElementIndexes = new HashSet<int>();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.UpdateIfRequiredOrScript();

            var stepsProp = serializedObject.FindProperty(nameof(StepFormula.steps));
            UpdateBadElementIndexes(stepsProp);

            var detailedLabel = new GUIContent(stepsProp.displayName);
            if (badElementIndexes?.Count > 0) detailedLabel.text = detailedLabel.text + " <color=red>(unsorted/dupe)</color>";
            var titleLineRect = GUILayoutUtility.GetRect(detailedLabel, EditorStyles.largeLabel);
            var labelRect = new Rect(titleLineRect) { xMax = titleLineRect.xMax - 80 };
            stepsProp.isExpanded = EditorGUI.Foldout(labelRect, stepsProp.isExpanded, detailedLabel, true, new GUIStyle(EditorStyles.foldout) { richText = true });

            if (stepsProp.isExpanded)
            {
                EditorGUI.indentLevel++;
                var toRemoveIndexes = new List<int>();

                ForeachArrayElement(stepsProp, (stepProp, index) =>
                {
                    var rect = GUILayoutUtility.GetRect(GUIContent.none, new GUIStyle(), GUILayout.ExpandWidth(true), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    rect = EditorGUI.IndentedRect(rect);

                    ColoredGUIDrawing(badElementIndexes?.Contains(index) ?? false ? (Color?)Color.red : null, () =>
                    {
                        var subRect = new Rect(rect) { xMin = rect.xMin + 40 };
                        var r1 = new Rect(subRect) { xMax = subRect.xMin + subRect.width * 0.4f };
                        var r2 = new Rect(subRect) { xMin = r1.xMax, xMax = r1.xMax + 25 };
                        var r3 = new Rect(subRect) { xMin = r2.xMax };
                        EditorGUI.PropertyField(r1, stepProp.FindPropertyRelative(nameof(StepFormula.Step.level)), GUIContent.none);
                        GUI.Label(r2, "->");
                        EditorGUI.PropertyField(r3, stepProp.FindPropertyRelative(nameof(StepFormula.Step.value)), GUIContent.none);
                    });

                    var btnRects = Split(new Rect(rect) { width = 40 }, 2);
                    ColoredGUIDrawing(RemoveBtnColor, () =>
                    {
                        if (GUI.Button(Grown(btnRects[0], -1), "-"))
                        {
                            toRemoveIndexes.Add(index);
                        }
                    });
                    ColoredGUIDrawing(AddBtnColor, () =>
                    {
                        if (GUI.Button(Grown(btnRects[1], -1), "+")) stepsProp.InsertArrayElementAtIndex(index);
                    });
                });

                EditorGUI.indentLevel--;

                foreach (var i in toRemoveIndexes)
                {
                    stepsProp.DeleteArrayElementAtIndex(i);
                }
            }

            if (true) // draw main buttons
            {
                var mainBtnRects = Split(new Rect(titleLineRect) { xMin = titleLineRect.xMax - 80 }, 4);
                ColoredGUIDrawing(RemoveBtnColor, () =>
                {
                    if (GUI.Button(Grown(mainBtnRects[0], -0.5f), "x")) stepsProp.ClearArray();
                    if (GUI.Button(Grown(mainBtnRects[1], -0.5f), "-") && stepsProp.arraySize > 0) stepsProp.DeleteArrayElementAtIndex(stepsProp.arraySize - 1);
                });
                ColoredGUIDrawing(AddBtnColor, () =>
                {
                    if (GUI.Button(Grown(mainBtnRects[2], -0.5f), "+")) stepsProp.InsertArrayElementAtIndex(stepsProp.arraySize);
                });
                ColoredGUIDrawing(UtilsBtnColor, () =>
                {
                    if (GUI.Button(Grown(mainBtnRects[3], -0.5f), "⇅")) SortStepsProp(stepsProp);
                });
            }

            serializedObject.ApplyModifiedProperties();

        }

        void SortStepsProp(SerializedProperty stepsProp)
        {
            SortSerializedArray(stepsProp, p => p.FindPropertyRelative(nameof(StepFormula.Step.level)).intValue);
        }

        void UpdateBadElementIndexes(SerializedProperty stepsProp)
        {
            badElementIndexes.Clear();
            var arrSize = GetMinArraySize(stepsProp);
            if (arrSize > 1)
            {
                for (var i = 0; i < arrSize - 1; i++)
                {
                    if (GetStepVal(stepsProp, i) >= GetStepVal(stepsProp, i + 1))
                    {
                        badElementIndexes.Add(i);
                        badElementIndexes.Add(i + 1);
                    }
                }
            }
        }


        static Color UtilsBtnColor = Color.Lerp(new Color32(11, 255, 111, 255), Color.white, 0.5f);
        static Color RemoveBtnColor = Color.Lerp(new Color32(255, 89, 89, 255), Color.white, 0.35f);
        static Color AddBtnColor = Color.Lerp(new Color32(127, 232, 255, 255), Color.white, 0.35f);

        static void ForeachArrayElement(SerializedProperty arrProp, System.Action<SerializedProperty, int> action)
        {
            var count = 0;
            var endProp = arrProp.GetEndProperty();
            var eleProp = arrProp.Copy();
            eleProp.Next(true); eleProp.Next(true); eleProp.Next(false);
            while (!SerializedProperty.EqualContents(eleProp, endProp))
            {
                action?.Invoke(eleProp, count);
                if (!eleProp.Next(false)) break;
                count++;
            }
        }

        static void SortSerializedArray(SerializedProperty arrProp, System.Func<SerializedProperty, System.IComparable> comparableGetter)
        {
            var props = new List<SerializedProperty>();
            for (var i = 0; i < arrProp.arraySize; i++) props.Add(arrProp.GetArrayElementAtIndex(i));

            var oldIndexDict = props.Select((p, i) => (p, i)).ToDictionary(pi => pi.p, pi => pi.i);
            var newIndexDict = props.OrderBy(comparableGetter).Select((p, i) => (p, i)).ToDictionary(pi => pi.p, pi => pi.i);

            foreach (var kvp in newIndexDict)
            {
                var prop = kvp.Key;
                var newIndex = kvp.Value;
                var oldIndex = oldIndexDict[prop];
                if (newIndex != oldIndex)
                {
                    if (newIndex < oldIndex)
                        foreach (var lesserProp in props.Where(p => oldIndexDict[p] < oldIndex && oldIndexDict[p] >= newIndex))
                            oldIndexDict[lesserProp]++;
                    else if (newIndex > oldIndex)
                        foreach (var higherProp in props.Where(p => oldIndexDict[p] > oldIndex && oldIndexDict[p] <= newIndex))
                            oldIndexDict[higherProp]--;
                    arrProp.MoveArrayElement(oldIndex, newIndex);
                }
            }

        }

        static int GetMinArraySize(SerializedProperty arrProp)
        {
            var arrSizeProp = arrProp.FindPropertyRelative("Array.size");
            if (arrSizeProp.hasMultipleDifferentValues)
            {
                var count = 0;
                ForeachArrayElement(arrProp, (p, i) => count++);
                return count;
            }
            else return arrSizeProp.intValue;
        }

        int? GetStepVal(SerializedProperty stepsProp, int i)
        {
            return GetStepVal(stepsProp.GetArrayElementAtIndex(i));
        }

        int? GetStepVal(SerializedProperty prop)
        {
            return prop.hasMultipleDifferentValues ? null : (int?)prop.FindPropertyRelative(nameof(StepFormula.Step.level)).intValue;
        }

        static void ColoredGUIDrawing(Color? c, System.Action drawCallback)
        {
            var c0 = GUI.color;
            if (c.HasValue) GUI.color = c.Value;
            drawCallback?.Invoke();
            GUI.color = c0;
        }

        // left to right, bottom to top
        static Rect[] Split(Rect rect, int cols = 1, int rows = 1)
        {
            rows = Mathf.Max(1, rows);
            cols = Mathf.Max(1, cols);

            var size = new Vector2(rect.width / cols, rect.height / rows);
            var rs = new Rect[rows * cols];

            for (var y = 0; y < rows; y++)
            {
                for (var x = 0; x < cols; x++)
                {
                    var cx = rect.position.x + (x + 0.5f) * size.x;
                    var cy = rect.position.y + (y + 0.5f) * size.y;

                    var index = y * cols + x;
                    rs[index] = new Rect(new Vector2(cx, cy) - size / 2f, size);
                }
            }
            return rs;
        }


        static Rect Grown(Rect r, float f)
        {
            return Grown(r, Vector2.one * f);
        }

        static Rect Grown(Rect r, float x, float y)
        {
            return Grown(r, new Vector2(x, y));
        }

        static Rect Grown(Rect r, Vector2 half)
        {
            return new Rect(r.position - half, r.size + half * 2);
        }

    }
#endif

}