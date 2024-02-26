using UnityEngine;

namespace Npu
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] float speed = -0.5f;
        [SerializeField] Vector3 rotary = new Vector3(0, 0, 1);


        private void Update()
        {
            transform.Rotate(rotary * (speed * Time.deltaTime * 100f));
        }
    }

}