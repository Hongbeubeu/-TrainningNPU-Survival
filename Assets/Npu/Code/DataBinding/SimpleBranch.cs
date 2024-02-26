using UnityEngine;

namespace Npu
{
    public class SimpleBranch : MonoBehaviour
    {
        [SerializeField, MemberSelector(returnType = typeof(bool))]
        private MemberSelector condition;

        [SerializeField, MemberSelector(parameterType = typeof(string))]
        private MemberSelector target;

        [SerializeField] private string text;
        [SerializeField] private string defaultText;

        private string _defaultText;

        public string DefaultText
        {
            get => _defaultText ?? defaultText;
            set
            {
                _defaultText = value;
                Bind();
            }
        }


        public string Text
        {
            get => text;
            set
            {
                text = value;
                Bind();
            }
        }

        private void Bind()
        {
            OptionalInit();

            target.SetValue((bool) condition.GetValue() ? Text : DefaultText);
        }

        public void _Bind(object unused)
        {
            Bind();
        }

        private bool initialized;
        private void OptionalInit()
        {
            if (initialized) return;

            initialized = true;
            condition.Setup();
            target.Setup();
        }
    }
}
