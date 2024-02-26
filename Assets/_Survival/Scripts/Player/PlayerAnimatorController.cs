using System;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _renderer;
    private static readonly int Speed = Animator.StringToHash("Speed");

    public void SetInfo(Vector2 dir)
    {
        SetDirection(dir);
        SetAnim(dir != Vector2.zero ? PlayerAnimState.Run : PlayerAnimState.Idle);
    }

    public void SetAnim(PlayerAnimState state)
    {
        switch (state)
        {
            case PlayerAnimState.Idle:
                SetIdle();
                break;
            case PlayerAnimState.Run:
                SetRun();
                break;
        }
    }

    public void SetDirection(Vector2 dir)
    {
        _renderer.flipX = dir.x switch
        {
            < 0 => true,
            > 0 => false,
            _ => _renderer.flipX
        };
    }

    public void SetRun()
    {
        _animator.SetFloat(Speed, 1f);
    }

    public void SetIdle()
    {
        _animator.SetFloat(Speed, 0f);
    }
}