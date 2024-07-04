using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Projects.Localization
{
    public class LocalizedTextManager : AbstractSingletonBehaviour<LocalizedTextManager>
    {
        [SerializeField] private LocalizedTextDictionary _textDictionary;

        private readonly ReactiveProperty<ELanguage> _currentLanguage = new(0);

        private readonly Dictionary<string, string> _tags = new();

        private readonly Dictionary<string, string> _texts = new();
        protected override bool IsDontDestroyOnLoad => true;

        private bool IsLoaded { get; set; }
        public ReadOnlyReactiveProperty<ELanguage> CurrentLanguage => _currentLanguage;

        protected override void Awake()
        {
            base.Awake();
            _currentLanguage.AddTo(this);
        }

        public void LoadText(ELanguage lang)
        {
            LoadText(lang, "LanguageTable.csv");
        }

        private void LoadText(ELanguage lang, string path)
        {
            if (IsLoaded)
            {
                _texts.Clear();
                _tags.Clear();
            }

            var allText = Addressables.LoadAssetAsync<TextAsset>(path).WaitForCompletion();
            var allLines = allText.text.Split('\n').ToList();
            Addressables.Release(allText);

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
            return _texts[id];
        }

        private bool CheckTextIsValid(string id)
        {
            if (!_texts.ContainsKey(id))
            {
                Debug.Log($"[TextSystem] message id {id} is not found. ");
                return false;
            }

            return true;
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
            _textDictionary.Dictionary.TryAdd(language, font);
    }
}