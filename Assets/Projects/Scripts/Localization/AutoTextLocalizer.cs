using R3;
using TMPro;
using UnityEngine;

namespace Projects.Localization
{
    /// <summary>
    ///     テキストのローカライズ機能
    /// </summary>
    public class AutoTextLocalizer : MonoBehaviour
    {
        [SerializeField] private string _textId;

        private TextMeshProUGUI _targetText;
        private LocalizedTextManager _textSystem;

        private void Start()
        {
            _textSystem = LocalizedTextManager.Instance;
            _targetText = GetComponent<TextMeshProUGUI>();

            _textSystem.CurrentLanguage
                .Where(_ => !string.IsNullOrEmpty(_textId))
                .Subscribe(_ => UpdateDisplay()).AddTo(this);
        }

        public void UpdateDisplay()
        {
            _targetText.SetText(_textSystem.GetText(_textId));
            _targetText.font = _textSystem.CurrentFont;
        }

        public void SetTextId(string textId) => _textId = textId;
    }
}