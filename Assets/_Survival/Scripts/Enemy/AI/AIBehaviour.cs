using System;
using Ultimate.Core.Runtime.FSM;

public abstract class AIBehaviour : IDisposable
{
    protected Enemy _aiController;
    protected FSM _fsm;
    protected bool _isCalculating = false;

    public AIBehaviour(Enemy aiController)
    {
        _aiController = aiController;
    }

    public void Init()
    {
        InitFSM();
    }

    // Update is called once per frame
    public void Update(float dt)
    {
        if (_isCalculating)
        {
            _fsm.Update(dt);
        }
    }

    #region FSM

    public AIState GetCurrentState()
    {
        return (AIState)_fsm.GetCurrentState();
    }

    protected abstract void InitFSM();
    public abstract void ChangeState(AIState newState);

    public virtual void StartCalculating()
    {
        _isCalculating = true;
    }


    public virtual void Stop()
    {
        _isCalculating = false;
    }

    #endregion FSM

    public virtual void Dispose()
    {
        _aiController = null;
        _fsm.Destroy();
        _fsm = null;
    }
}

//    public enum AiState
//    {
//        Idle,
//        Move,
//        Attack,
//        Relax,
//        Dacing
//    }
//
//    public enum FSMTranstion
//    {
//        ToAttack,
//        ToIdle,
//        ToMove,
//        ToRelax,
//        ToDacing
//    }