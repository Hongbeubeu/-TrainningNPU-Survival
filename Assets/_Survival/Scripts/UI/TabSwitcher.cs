using UnityEngine;

public class TabSwitcher : MonoBehaviour
{
    [SerializeField] private BaseUI[] _tabs;

    public void OnSwitchTab(int index)
    {
        for (var i = 0; i < _tabs.Length; i++)
        {
            if (i != index)
            {
                _tabs[i].Hide();
            }
            else
            {
                _tabs[i].Show();
            }
        }
    }
}