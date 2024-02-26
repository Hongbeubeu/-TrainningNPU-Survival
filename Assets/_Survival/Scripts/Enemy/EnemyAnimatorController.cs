using UnityEngine;

public class EnemyAnimatorController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Animator _animator;

    public void SetDirection(Vector2 dir)
    {
        _renderer.flipX = dir.x switch
        {
            > 0 => true,
            < 0 => false,
            _ => _renderer.flipX
        };
    }
}