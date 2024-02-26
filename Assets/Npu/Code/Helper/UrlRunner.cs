using UnityEngine;

namespace Npu.Helper
{
    public class UrlRunner : MonoBehaviour
    {
        [SerializeField] private string url;

        public void Run()
        {
            Application.OpenURL(url);
        }
    }
    
}
