using Ultimate.Core.Runtime.FSM;

public class EnemyIdleAction : FSMAction
{
    private Enemy _aiController;
    private AIBehaviour _aiBehaviour;

    public EnemyIdleAction(FSMState owner, Enemy aiController, AIBehaviour aiBehaviour) : base(owner)
    {
        _aiController = aiController;
        _aiBehaviour = aiBehaviour;
    }

    public override void OnEnter()
    {
    }

    public override void OnUpdate(float dt)
    {
    }

    public override void OnDestroy()
    {
        _aiController = null;
        _aiBehaviour = null;
    }
}