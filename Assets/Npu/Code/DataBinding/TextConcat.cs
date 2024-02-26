using UnityEngine;
using UnityEngine.UI;

namespace Npu
{
    public class TextConcat : MonoBehaviour
    {
        public string separator = "\n";

        Text _text;
        Text Text
        {
            get
            {
                if (_text == null) _text = GetComponent<Text>();
                return _text;
            }
        }

        public void Clear(object data)
        {
            Text.text = string.Format("{0}", data);
        }

        public void Append(object txt)
        {
            var str = Text.text;
            str = string.Format("{0}{1}{2}", str, string.IsNullOrEmpty(str) ? "" : separator, txt);
            Text.text = str;
        }
    }
}
