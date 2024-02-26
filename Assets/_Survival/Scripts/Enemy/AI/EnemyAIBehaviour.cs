using Ultimate.Core.Runtime.FSM;
using UnityEngine;

public class EnemyAIBehaviour : AIBehaviour
{
    protected FSMState _idleState, _attackState, _moveState;
    protected EnemyIdleAction _idleAction;
    protected EnemyAttackAction _attackAction;
    protected EnemyMoveAction _moveAction;
    public AIState CurrentState { get; private set; }

    public EnemyAIBehaviour(Enemy controller) : base(controller)
    {
        Init();
    }

    protected override void InitFSM()
    {
        _fsm = new FSM("Enemy FSM");
        _idleState = _fsm.AddState((int)AIState.Idle);
        _attackState = _fsm.AddState((int)AIState.Attack);
        _moveState = _fsm.AddState((int)AIState.Move);

        _idleAction = new EnemyIdleAction(_idleState, _aiController, this);
        _attackAction = new EnemyAttackAction(_attackState, _aiController, this);
        _moveAction = new EnemyMoveAction(_attackState, _aiController, this);

        _idleState.AddTransition((int)FSMTransition.ToAttack, _attackState);
        _idleState.AddTransition((int)FSMTransition.ToMove, _moveState);

        _attackState.AddTransition((int)FSMTransition.ToIdle, _idleState);
        _attackState.AddTransition((int)FSMTransition.ToMove, _moveState);

        _moveState.AddTransition((int)FSMTransition.ToAttack, _attackState);
        _moveState.AddTransition((int)FSMTransition.ToIdle, _idleState);
    }

    public override void ChangeState(AIState newState)
    {
        if ((int)newState == _fsm.GetCurrentState())
        {
            return;
        }

        CurrentState = newState;
        switch (newState)
        {
            case AIState.Idle:
                _fsm.ChangeToState(_idleState);
                break;
            case AIState.Move:
                _fsm.ChangeToState(_moveState);
                break;
            case AIState.Attack:
                _fsm.ChangeToState(_attackState);
                break;
            default:
                Debug.LogError($"FSM does not contain state {newState.ToString()}");
                break;
        }
    }
}