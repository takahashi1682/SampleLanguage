using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using TMPro;
using UnityEngine;

namespace Projects.Localization
{
    public class LocalizedTextManager : AbstractSingletonBehaviour<LocalizedTextManager>
    {
        [SerializeField] private TextAsset _languageTable;
        [SerializeField] private SerializableReactiveProperty<ELanguage> _currentLanguage = new(0);
        [SerializeField] private bool _isAutoLoad = true;
        [SerializeField] private bool _isShowGUI = true;
        [SerializeField] private LocalizedFontDictionary _textFontDictionary;

        private readonly Dictionary<string, string> _tags = new();
        private readonly Dictionary<string, string> _texts = new();

        public ReadOnlyReactiveProperty<ELanguage> CurrentLanguage => _currentLanguage;
        public TMP_FontAsset CurrentFont => _textFontDictionary.Dictionary.GetValueOrDefault(_currentLanguage.Value);

        private bool IsLoaded { get; set; }
        protected override bool IsDontDestroyOnLoad => true;

        protected override void Awake()
        {
            base.Awake();
            if (_isAutoLoad)
            {
                LoadText(_currentLanguage.Value);
            }

            _currentLanguage.AddTo(this);
            _currentLanguage.Subscribe(LoadText).AddTo(this);
        }

#if UNITY_EDITOR
        private void OnGUI()
        {
            if (!_isShowGUI) return;

            var languages = Enum.GetValues(typeof(ELanguage));
            GUILayout.Box("Change Language", GUILayout.Width(170), GUILayout.Height(25 * (languages.Length + 2)));
            var screenRect = new Rect(10, 25, 150, 25 * (languages.Length + 1));

            GUILayout.BeginArea(screenRect);
            foreach (ELanguage language in languages)
            {
                if (GUILayout.RepeatButton(language.ToString()))
                {
                    Instance.LoadText(language);
                }
            }

            GUILayout.EndArea();
        }
#endif

        private void LoadText(ELanguage lang)
        {
            if (IsLoaded)
            {
                _tags.Clear();
                _texts.Clear();
            }

            var allLines = _languageTable.text.Split('\n').ToList();
            allLines.RemoveAt(0);

            var langNum = (int)lang + 1;
            foreach (var elements in allLines.Select(line => line.Replace("%%", "\n").Split(',').ToList())
                         .Where(elements => elements[0] != string.Empty))
            {
                _texts.Add(elements[0], elements[langNum].Replace("*", ","));
            }

            _currentLanguage.Value = lang;

            IsLoaded = true;
        }

        public string GetText(string id)
        {
            if (!_texts.TryGetValue(id, out var result))
            {
                Debug.Log($"[TextSystem] message id {id} is not found. ");
                return string.Empty;
            }

            return result;
        }

        public string GetReplaced(string id, string buf)
        {
            return _texts[id].Replace("###", buf);
        }

        public string GetReplaced(string id, string buf, string buf2)
        {
            return _texts[id].Replace("#1#", buf).Replace("#2#", buf2);
        }

        public string GetTagged(string id)
        {
            var result = _texts[id];
            if (result.Contains('#'))
            {
                foreach (var t in _tags.Where(t => result.Contains(t.Key)))
                {
                    result = result.Replace($"#{t.Key}#", t.Value);
                }
            }

            return result;
        }

        public void SetTag(string textTag, string text) => _tags[textTag] = text;

        public void SetFont(ELanguage language, TMP_FontAsset font) =>
            _textFontDictionary.Dictionary.TryAdd(language, font);
    }
}