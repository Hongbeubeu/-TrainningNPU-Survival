using Ultimate.Core.Runtime.EventManager;

public class ChangeGameStateEvent : GameEvent
{
    public GameState CurrentState;
}