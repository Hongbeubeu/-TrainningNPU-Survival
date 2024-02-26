using TMPro;
using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _counterText;
    [SerializeField] private float _interval;
    private float _coolDown;

    private void Update()
    {
        if (_coolDown <= 0)
        {
            _counterText.SetText($"FPS: {(int)(1f / Time.unscaledDeltaTime)}");
            _coolDown = _interval;
        }

        _coolDown -= Time.deltaTime;
    }
}