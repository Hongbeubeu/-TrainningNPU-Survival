using UnityEngine;

public class ProgressBarComponent : MonoBehaviour
{
    public Transform Bar;

    public void SetProgress(float progress)
    {
        var scale = Bar.localScale;
        scale.x = Mathf.Clamp(progress, 0, 1);
        Bar.localScale = scale;
    }
}