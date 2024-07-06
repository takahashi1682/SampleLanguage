using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Projects.Localization
{
    [Serializable]
    public struct LanguageFontPair
    {
        [SerializeField] private ELanguage _language;
        [SerializeField] private TMP_FontAsset _font;

        public ELanguage Language => _language;
        public TMP_FontAsset Font => _font;
    }

    /// <summary>
    ///  ローカライズテキストマネージャ
    /// </summary>
    public class LocalizedTextManager : AbstractSingletonBehaviour<LocalizedTextManager>
    {
        [SerializeField] private string _prefKey = "KeyLanguage";
        [SerializeField] private string _localizationTableKey = "StringTable";
        [SerializeField] private SerializableReactiveProperty<ELanguage> _currentLanguage = new(0);
        [SerializeField] private bool _isAutoLoad = true;
        [SerializeField] private bool _isShowGUI = true;
        [SerializeField] private LanguageFontPair[] _fontDictionary;

        private readonly Dictionary<string, string> _stringTable = new();

        public ReadOnlyReactiveProperty<ELanguage> CurrentLanguage => _currentLanguage;
        public TMP_FontAsset CurrentFont =>
            _fontDictionary.FirstOrDefault(x => x.Language == _currentLanguage.Value).Font;

        private bool IsLoaded { get; set; }
        protected override bool IsDontDestroyOnLoad => true;

        protected override void Awake()
        {
            base.Awake();

            // 設定値読み込み
            _currentLanguage.Value = LoadLanguage();

            // テーブル自動読み込み
            if (_isAutoLoad) LoadStringTable(_currentLanguage.Value);

            // 言語変更時の処理
            _currentLanguage.AddTo(this);
            _currentLanguage.Skip(1).Subscribe(x =>
            {
                // テーブル読み込み
                LoadStringTable(x);

                // 言語変更時に保存
                SaveLanguage(x);
            }).AddTo(this);
        }

        /// <summary>
        ///  言語設定
        /// </summary>
        /// <param name="language"></param>
        public void SetLanguage(ELanguage language) => _currentLanguage.Value = language;

        /// <summary>
        ///  テーブル読み込み
        /// </summary>
        /// <param name="lang"></param>
        private void LoadStringTable(ELanguage lang)
        {
            if (IsLoaded) _stringTable.Clear();

            var text = LoadResources(_localizationTableKey);
            var allLines = text.Split('\n').ToList();
            allLines.RemoveAt(0);

            var langNum = (int)lang + 1;
            foreach (var elements in allLines.Select(line => line.Replace("%%", "\n").Split(',').ToList())
                         .Where(elements => elements[0] != string.Empty))
            {
                _stringTable.Add(elements[0], elements[langNum].Replace("*", ","));
            }

            IsLoaded = true;
        }

        /// <summary>
        ///  テキスト取得
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetText(string key)
        {
            if (!_stringTable.TryGetValue(key, out var result))
            {
                Debug.Log($"[TextSystem] message id {key} is not found. ");
                return string.Empty;
            }

            return result;
        }

        /// <summary>
        ///  リソース読み込み
        /// </summary>
        /// <returns></returns>
        private string LoadResources(string path)
        {
            var result = string.Empty;

            var stringTable = Addressables.LoadAssetAsync<TextAsset>(path).WaitForCompletion();
            if (stringTable != null)
            {
                result = stringTable.text;
            }
            else
            {
                Debug.LogError($"[LoadResources] Unable to load resource at path: {_localizationTableKey}");
            }

            Addressables.Release(stringTable);

            return result;
        }

        /// <summary>
        ///  言語設定を読み込み
        /// </summary>
        /// <returns></returns>
        public ELanguage LoadLanguage() => (ELanguage)PlayerPrefs.GetInt(_prefKey, 0);

        /// <summary>
        ///  言語設定を保存
        /// </summary>
        /// <param name="language"></param>
        public void SaveLanguage(ELanguage language)
        {
            PlayerPrefs.SetInt(_prefKey, (int)language);
            PlayerPrefs.Save();
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
                    Instance.SetLanguage(language);
                }
            }

            GUILayout.EndArea();
        }
#endif
    }
}