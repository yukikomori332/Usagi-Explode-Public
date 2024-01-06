using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyProject
{   
    public class SceneLoader : MonoBehaviour
    {
        #region Inspector

        [SerializeField] string nextSceneName = "";

        #endregion

        #region Fields



        #endregion

        #region Properties



        #endregion

        #region MonoBehaviour



        #endregion

        #region Methods

        public void SetNextSceneName(string name)
        {
            nextSceneName = name;
        }

        public void LoadScene()
        {
            SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
        }

        #endregion
    }   
}
