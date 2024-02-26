using System;
using UnityEngine;


namespace Npu.Formula
{
    public interface IActivator
    {
        bool Activated { get; }
        void Setup();
        void TearDown();
        void Activate(bool active);
    }
    
    [Serializable]
    public class Activator : IActivator
    {
        [SerializeField] public ParameterSelector src;
        [SerializeField] public ParameterSelector dst;
        [SerializeField] public Mode mode;
        
        public bool Activated { get; private set; }
        private ParameterValve Valve { get; set; }
        
        public virtual void Setup()
        {
            Activated = false;
            Valve = null;
        }

        public virtual void TearDown()
        {
            ActivateSilent(false);
        }
        
        public void Activate(bool activate)
        {
            if (src.Value == null || dst.Value == null)
            {
                Logger.Error<Activator>($"Invalid src {src.Container} or dst {dst.Container}");
                return;
            }
            Activate(src.Value, dst.Value, activate);
        }
        
        public void ActivateSilent(bool activate)
        {
            if (src.Value == null || dst.Value == null) return;
            
            Activate(src.Value, dst.Value, activate);
        }

        private void Activate(IParameter src, IParameter dst, bool activate)
        {
            if (activate == Activated)
            {
                Logger._Error<Activator>($"Already {activate}");
                return;
            }
            
            switch (mode)
            {
                case Mode.Valve:
                    if (!(dst is Parameter))
                    {
                        Logger.Error<Activator>($"Failed to valve from {src.Name} ({src}) to {dst.Name} ({dst})");
                        break;
                    }
                    
                    if (activate)
                    {
                        if (Valve == null) Valve = new ParameterValve(src, dst);
                        Valve.Release();
                    }
                    else
                    {
                        Valve?.Block();
                        Valve = null;
                    }
                    break;
                
                case Mode.Bridge:
                    if (dst is BridgeParameter bp)
                    {
                        if (activate) bp.Connect(src);
                        else bp.Disconnect();
                    }
                    else
                    {
                        Logger.Error<Activator>($"Failed to bridge from {src.Name} ({src}) to {dst.Name} ({dst})");
                    }
                    break;
                    
                case Mode.Contribute:
                    if (dst is AbstractEvaluator ev)
                    {
                        if (activate) ev.Add(src);
                        else ev.Remove(src);
                    }
                    else
                    {
                        Logger.Error<Activator>($"Failed to contribute from {src.Name} ({src}) to {dst.Name} ({dst})");
                    }
                    break;
            }

            Activated = activate;
        }
        
        public enum Mode
        {
            Contribute,
            Valve,
            Bridge
        }
    }
}