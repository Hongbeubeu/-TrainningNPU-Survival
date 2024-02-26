using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player _player;
    private Vector2 _normalizedDir;
    public float CurrentSpeed;

    private float _coolDown;
    private float _duration;
    private bool _isBurstSpeed;

    public void SetInfo(Player player)
    {
        _player = player;
        ResetData();
    }

    public void ResetData()
    {
        CurrentSpeed = _player.CurrentData.Speed + _player.CurrentData.SpeedUp * _player.CurrentData.Speed;
        _coolDown = _player.CurrentData.SpeedBurstCoolDown;
        _duration = 0f;
        _isBurstSpeed = false;
        transform.position = Vector3.zero;
        GameController.Instance.InfiniteBackground.SetMiddle(transform.position);
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.CurrentGameState == GameState.Pause)
        {
            _normalizedDir = Vector2.zero;
            return;
        }

        _normalizedDir = (Vector2.up * InputController.Instance.Joystick.Vertical +
                          Vector2.right * InputController.Instance.Joystick.Horizontal).normalized;
    }

    private void Update()
    {
        if (GameController.Instance.CurrentGameState == GameState.Pause)
            return;
        if (_isBurstSpeed)
        {
            if (_duration >= _player.CurrentData.SpeedBurstDuration)
            {
                DownSpeed();
                _duration = 0f;
            }

            _duration += Time.deltaTime;
        }
        else
        {
            if (_coolDown <= 0)
            {
                BurstSpeed();
                _coolDown = _player.CurrentData.SpeedBurstCoolDown;
            }

            _coolDown -= Time.deltaTime;
        }

        _player.AnimatorController.SetInfo(_normalizedDir);
        _player.PlayerMotionDirection.SetInfo(_normalizedDir);
        transform.Translate(_normalizedDir * (CurrentSpeed * Time.deltaTime));
        GameController.Instance.InfiniteBackground.SetMiddle(transform.position);
    }

    private void BurstSpeed()
    {
        _isBurstSpeed = true;
        CurrentSpeed = _player.CurrentData.Speed + _player.CurrentData.SpeedBurst * _player.CurrentData.Speed;
    }

    private void DownSpeed()
    {
        _isBurstSpeed = false;
        CurrentSpeed = _player.CurrentData.Speed + _player.CurrentData.SpeedUp * _player.CurrentData.Speed;
    }
}