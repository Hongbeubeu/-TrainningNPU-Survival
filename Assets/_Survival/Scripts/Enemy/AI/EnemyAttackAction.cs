using Ultimate.Core.Runtime.FSM;

public class EnemyAttackAction : FSMAction
{
    private Enemy _aiController;
    private AIBehaviour _aiBehaviour;

    public EnemyAttackAction(FSMState owner, Enemy aiController, AIBehaviour aiBehaviour) : base(owner)
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