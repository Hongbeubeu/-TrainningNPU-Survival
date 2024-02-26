public class MapTabPanel : BaseUI
{
    public LevelButton[] LevelButtons;

    public override void Show()
    {
        base.Show();
        Init();
    }

    public void Init()
    {
        for (var i = 0; i < GameManager.Instance.EnemyMapData.EnemyByMapData.Count; i++)
        {
            if (i >= LevelButtons.Length)
                return;
            LevelButtons[i].SetInfo(this, i, i <= GameManager.Instance.PlayerData.LevelUnlocked);
        }
    }

    public void OnClickButton(int level)
    {
        GameController.Instance.StartGame(level);
        UIController.Instance.MenuGamePanel.Hide();
        UIController.Instance.InGamePanel.Show();
    }
}