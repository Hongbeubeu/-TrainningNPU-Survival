using Ultimate.Core.Runtime.EventManager;
using UnityEngine;
using UnityEngine.UI;

public class XPBarPanel : MonoBehaviour
{
    public Scrollbar Scrollbar;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<PlayerChangeXPEvent>(OnPlayerChangeXP);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<PlayerChangeXPEvent>(OnPlayerChangeXP);
    }


    public void OnPlayerChangeXP(PlayerChangeXPEvent e)
    {
        var value = e.CurrentXP / e.MaxXP;
        value = Mathf.Clamp(value, 0f, 1f);
        Scrollbar.size = value;
    }
}