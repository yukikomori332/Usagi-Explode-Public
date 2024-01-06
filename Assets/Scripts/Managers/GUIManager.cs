using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MyProject
{
    public class GUIManager : MonoBehaviour
    {
        #region Inspector

        [Header("Screens")]
        [SerializeField] private MenuScreen mainMenuScreen;

        [SerializeField] private MenuScreen hudScreen;

        [SerializeField] private MenuScreen pauseScreen;

        [SerializeField] private MenuScreen resultScreen;

        [Header("Score Texts")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI resultScoreText;

        [Header("Speed Status")]
        [SerializeField] private CanvasGroup speedBoostStatus;
        [SerializeField] private TextMeshProUGUI speedBoostText;

        #endregion

        #region Fields

        PlayerMovement m_PlayerMovement;

        #endregion

        #region Methods

        public virtual void Initialize()
        {
            foreach (PlayerController playerController in PlayerController.playerControllers)
            {
                if (playerController.name == "Player")
                {
                    // プレイヤーの移動を取得
                    m_PlayerMovement = playerController.PlayerMovement;
                    m_PlayerMovement.UpdatedMoveSpeed += UpdateSpeedText;
                }
            }
        }

        public void ShowMainMenu()
        {
            mainMenuScreen.TurnOn();
        }

        public void ShowHUD()
        {
            hudScreen.TurnOn();
        }

        public void ShowPause()
        {
            pauseScreen.TurnOn();
        }

        public void ShowResult()
        {
            resultScreen.TurnOn();
        }

        public void HideMainMenu()
        {
            mainMenuScreen.TurnOff();
        }

        public void HideHUD()
        {
            hudScreen.TurnOff();
        }

        public void HidePause()
        {
            pauseScreen.TurnOff();
        }

        public void HideResult()
        {
            resultScreen.TurnOff();
        }

        public void UpdateScoreText(int score)
        {
            scoreText.SetText(score.ToString());
        }

        public void UpdateResultScoreText(int score)
        {
            resultScoreText.SetText(score.ToString());
        }

        public void UpdateSpeedText(float speedBoost)
        {
            speedBoostText.SetText("+ " + speedBoost.ToString("F1"));
            UpdateSpeedBoostStatus(true);
        }

        public void UpdateSpeedBoostStatus(bool status)
        {
            speedBoostStatus.alpha = status ? 1 : 0;
        }

        #endregion
    }
}
