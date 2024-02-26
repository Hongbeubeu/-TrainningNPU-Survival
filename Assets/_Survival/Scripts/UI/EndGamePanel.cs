using TMPro;
using UnityEngine;

public class EndGamePanel : BaseUI
{
    [SerializeField] private TextMeshProUGUI _text;

    public void SetInfo(bool isWin)
    {
        GameController.Instance.IsEndGame = true;
        _text.SetText(isWin ? "Victory" : "Lose");
        Show();
    }

    public void OnHomeBtnClick()
    {
        Hide();
        UIController.Instance.MenuGamePanel.Show();
        UIController.Instance.MenuGamePanel.MapTabPanel.Show();
    }
}