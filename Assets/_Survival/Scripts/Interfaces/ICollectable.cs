using UnityEngine;

public interface ICollectable
{
    public CollectableItemType GetItemType();
    public float GetValue();
    public void Destroy();

    public Vector3 GetPosition();
}