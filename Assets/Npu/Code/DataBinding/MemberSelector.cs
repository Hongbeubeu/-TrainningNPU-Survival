using System;
using UnityEngine;

namespace Npu
{
    
    [Serializable]
    public class MemberSelector
    {
        [SerializeField] protected UnityEngine.Object target;
        [SerializeField] protected string member;

        protected MemberInfoFacade _memberInfoFacade;

        public MemberSelector(UnityEngine.Object target, string member)
        {
            this.target = target;
            this.member = member;
        }

        public virtual void Setup(UnityEngine.Object context = null)
        {
            FindMemberInfo(context);
        }

        protected void FindMemberInfo(UnityEngine.Object context = null)
        {
            _memberInfoFacade = new MemberInfoFacade();
            _memberInfoFacade.Init(TargetType, member, context);
        }

        public string MemberName => member;
        public virtual object Target => target;

        public virtual Type TargetType => target?.GetType();

        public virtual bool IsValid => TargetType != null && _memberInfoFacade.IsValid;

        public object GetValue()
        {
            return _memberInfoFacade.GetValue(target);
        }

        public object GetValue(object target)
        {
            return _memberInfoFacade.GetValue(target);
        }
        
        public void SetValue(object value)
        {
            _memberInfoFacade.SetValue(Target, value);
        }
        
        public object ForceGetValue()
        {
            Setup();
            return _memberInfoFacade.GetValue(Target);
        }

        public override string ToString()
        {
            return $"Target: {target}\n{_memberInfoFacade})";
        }

        public MemberSelector Clone() => new MemberSelector(target, member = (string)member.Clone());

    }
}