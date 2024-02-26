using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Npu.Core
{
    [Serializable]
    public class CompoundConditions : ICondition
    {
        [SerializeField] private EvaluationMode mode;
        [SerializeField] private List<ValueCondition> conditions;

        public EvaluationMode Mode => mode;
        public IEnumerable<ValueCondition> Conditions => conditions;
        public bool Valid { get; private set; }

        public bool Meets => mode == EvaluationMode.All ? conditions.All(i => i.Meets()) : conditions.Any(i => i.Meets());
        public bool MeetsOrEmpty => conditions.Count == 0 || Meets;
        public event Action<ICondition> Changed;
        public string Description => conditions.FirstOrDefault(i => !i.Meets())?.Description;
        
        public void Setup()
        {
            Valid = true;
            foreach (var condition in conditions)
            {
                condition.Setup();
                if (condition.Valid)
                {
                    condition.Listen(OnConditionChanged, true);
                }
                else
                {
                    Valid = false;
                    Logger.Error<CompoundConditions>($"Invalid sub condition");
                }
            }
        }

        public void TearDown()
        {
            foreach (var condition in conditions)
            {
                condition.TearDown();
                condition.Listen(OnConditionChanged, false);
            }
        }

        public void Listen(Action<ICondition> listener, bool listen)
        {
            if (listen) Changed += listener;
            else Changed -= listener;
        }

        private void OnConditionChanged(ValueCondition condition)
        {
            Changed?.Invoke(this);
        }

        public enum EvaluationMode
        {
            All,
            Any
        }

        public void Log()
        {
            var s = $"Meets: {Meets}\n\t"
                    + string.Join("\n\t", conditions.Select(i => i.Description));
            Debug.Log(s);
        }

    }

}