using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Npu.Core;

namespace Npu.Formula
{

    public abstract class AbstractEvaluator : IEvaluator
    {
        public event System.Action<IParameter> Changed = delegate { };
        protected List<IParameter> parameters = new List<IParameter>();

        protected bool dirty = true;
        protected SecuredDouble value;

        protected AbstractEvaluator(string name)
        {
            Name = name;
        }

        public abstract SecuredDouble Evaluate();

        public virtual string Name
        {
            get;
            set;
        }

        public virtual SecuredDouble Value
        {
            get { return Evaluate(); }
            set { Debug.LogError("Setting value for Evaluator is not allowed"); }
        }

        public virtual string Description => string.Format("{0} ({1})", Name, GetType());
        public virtual string LongDescription => Description;

        public void Add(IParameter param)
        {
            if (parameters.Contains(param))
            {
                Debug.LogErrorFormat("[{0}] already contains {1}", Description, param.Description);
                return;
            }

            param.Changed += OnParameterChanged;
            parameters.Add(param);

            OnParameterChanged(param);
        }

        public void Remove(IParameter param)
        {
            param.Changed -= OnParameterChanged;
            parameters.Remove(param);

            OnParameterChanged(param);
        }

        public void RemoveAll()
        {
            foreach (var p in parameters)
            {
                p.Changed -= OnParameterChanged;
            }

            parameters.Clear();
            dirty = true;
            Changed.Invoke(this);
        }

        protected void OnParameterChanged(IParameter param)
        {
            dirty = true;
            Changed.Invoke(this);
        }

        protected void TriggerChanged(IParameter param)
        {
            Changed.Invoke(param);
        }

        public IEnumerator ParameterEnumerator
        {
            get { return parameters.GetEnumerator(); }
        }

        public List<IParameter> Parameters
        {
            get { return parameters; }
        }
    }

    public interface IEvaluator : IParameter
    {
        SecuredDouble Evaluate();
    }

}