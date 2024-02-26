using Ultimate.Core.Runtime.Singleton;

public class UIController : Singleton<UIController>
{
    public InGamePanel InGamePanel;
    public MenuGamePanel MenuGamePanel;

    public override void Init()
    {
    }

    public void ResetGame()
    {
        GameController.Instance.ResetGame();
        InGamePanel.Hide();
        MenuGamePanel.Show();
    }
}