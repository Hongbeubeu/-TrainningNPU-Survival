using System;
using UnityEngine;

namespace Npu.Formula
{

    [Serializable]
    public class ConditionalActivator : IActivator
    {
        [SerializeField] private string name;
        [SerializeField] public ValueCondition condition;
        [SerializeField, Box] public Activator[] activators;
        
        public bool Activated { get; private set; }
        
        public void Setup()
        {
            Activated = false;
            condition.Setup();
            foreach (var i in activators) i.Setup();
        }

        public void TearDown()
        {
            condition.Listen(OnConditionChanged, false);
            condition.TearDown();
            foreach (var i in activators) i.TearDown();
        }

        public void Begin()
        {
            condition.Listen(OnConditionChanged, false);
            
            if (condition.Meets())
            {
                Activate(true);
            }
            else
            {
                condition.Listen(OnConditionChanged, true);
            }
            
        }

        private void OnConditionChanged(ValueCondition condition)
        {
            if (Activated || !condition.Meets()) return;
            
            Activate(true);
            condition.Listen(OnConditionChanged, false);
        }

        public void Activate(bool active)
        {
            Activated = active;
            foreach (var i in activators) i.Activate(active);
        }
        
        public void ActivateSilent(bool active)
        {
            Activated = active;
            foreach (var i in activators) i.ActivateSilent(active);
        }
        
    }
    
    
}