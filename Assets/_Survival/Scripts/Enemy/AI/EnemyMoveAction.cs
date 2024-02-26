using Ultimate.Core.Runtime.FSM;

public class EnemyMoveAction : FSMAction
{
    private Enemy _aiController;
    private AIBehaviour _aiBehaviour;

    public EnemyMoveAction(FSMState owner, Enemy aiController, AIBehaviour aiBehaviour) : base(owner)
    {
        _aiController = aiController;
        _aiBehaviour = aiBehaviour;
    }
}