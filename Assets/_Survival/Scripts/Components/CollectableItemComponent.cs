using DG.Tweening;
using UnityEngine;

public class CollectableItemComponent : MonoBehaviour, ICollectable
{
    public CollectableItemType ItemType;
    public SpriteRenderer ItemSpriteRenderer;
    public float Value;

    public void SetInfo(CollectableItemType type, float value = 0)
    {
        ItemType = type;
        Value = value;
        ItemSpriteRenderer.sprite = GameManager.Instance.CollectableSprites[(int)ItemType];
        GameController.Instance.AddCollectable(this);
    }

    public void MoveTo(Vector3 des)
    {
        var distance = Vector2.Distance(des, transform.position);
        transform.DOMove(des, distance / 10f).SetEase(Ease.InBack).OnComplete(SelfDestroy);
    }

    public void Destroy()
    {
        GameManager.Instance.ObjectPooler.DestroyCollectableItem(gameObject);
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    private void SelfDestroy()
    {
        GameController.Instance.RemoveCollectable(this);
        Destroy();
    }

    #region Implement ICollectable

    public CollectableItemType GetItemType()
    {
        return ItemType;
    }

    public float GetValue()
    {
        return Value;
    }

    #endregion
}