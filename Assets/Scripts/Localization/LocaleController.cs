using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace MyProject
{
    public class LocaleController : MonoBehaviour
    {
        #region Methods

        public void SelectLocale(int localeIndex)
        {
            StartCoroutine(UpdateLocale(localeIndex));
        }

        private IEnumerator UpdateLocale(int localeIndex)
        {
            string localeName = localeIndex == 0 ? "ja" : "en";

            // 新規ロケールの作成
            LocalizationSettings.SelectedLocale = Locale.CreateLocale(localeName);
            yield return LocalizationSettings.InitializationOperation.Task;
        }

        #endregion
    }
}
