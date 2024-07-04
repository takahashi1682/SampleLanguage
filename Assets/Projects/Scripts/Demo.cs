using Projects.Localization;
using R3;
using UnityEngine;

namespace Projects
{
    public class Demo : MonoBehaviour
    {
        [SerializeField] private SerializableReactiveProperty<ELanguage> _language = new();

        private void Start()
        {
            _language.AddTo(this);
            _language.Subscribe(LocalizedTextManager.Instance.LoadText).AddTo(this);
        }
    }
}