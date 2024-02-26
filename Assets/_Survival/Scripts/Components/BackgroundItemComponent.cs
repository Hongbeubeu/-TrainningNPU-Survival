using UnityEngine;

public class BackgroundItemComponent : MonoBehaviour
{
    public void SetPosition(Vector2 worldPos)
    {
        transform.position = worldPos;
    }
}