using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class BaseUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rect;
    [SerializeField] protected bool _isShowOnStart;

    private void OnValidate()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _rect = GetComponent<RectTransform>();
    }

    public virtual void Start()
    {
        if (_isShowOnStart)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    public virtual void Show()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.blocksRaycasts = true;
        _rect.anchoredPosition = Vector2.zero;
    }

    public virtual void Hide()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.blocksRaycasts = false;
    }
}