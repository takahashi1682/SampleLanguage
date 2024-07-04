using System;
using Projects.Utils;
using TMPro;

namespace Projects.Localization
{
    [Serializable]
    public class LocalizedTextPair : AbstractPair<ELanguage, TMP_FontAsset>
    {
        public LocalizedTextPair(ELanguage key, TMP_FontAsset value) : base(key, value)
        {
        }
    }
}