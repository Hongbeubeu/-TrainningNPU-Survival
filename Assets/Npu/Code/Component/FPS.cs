using UnityEngine;
using TMPro;

namespace Npu.Common
{

    public class FPS : MonoBehaviour
    {
        public TMP_Text text;

        private float deltaTime = 0.0f;

        private void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

            var msec = deltaTime * 1000.0f;
            var fps = 1.0f / deltaTime;
            text.text = fps.ToString("f0");
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Toggle()
        {
            gameObject.SetActive(!gameObject.activeInHierarchy);
        }
    }

}