using System;
using Projects.Utils;
using TMPro;

namespace Projects.Localization
{
    [Serializable]
    public class LocalizedFontDictionary : SerializableDictionary<ELanguage, TMP_FontAsset, LocalizedTextPair>
    {
    }
}