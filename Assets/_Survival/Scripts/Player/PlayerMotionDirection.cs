using UnityEngine;

public class PlayerMotionDirection : MonoBehaviour
{
    [SerializeField] private float _radius;
    [SerializeField] private SpriteRenderer _renderer;

    public void SetInfo(Vector2 dir)
    {
        if (dir == Vector2.zero)
        {
            _renderer.enabled = false;
            return;
        }

        _renderer.enabled = true;
        var pos = dir * _radius;
        transform.localPosition = pos;
        transform.up = dir;
    }
}