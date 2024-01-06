using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject
{
    public class ApplicationController : MonoBehaviour
    {
        #region MonoBehaviour

        private void OnApplicationPause(bool pauseStatus)
        {
            Time.timeScale = pauseStatus ? 0 : 1;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            Time.timeScale = hasFocus ? 1 : 0;
        }

        #endregion

        #region Methods

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        }

        #endregion
    }
}
