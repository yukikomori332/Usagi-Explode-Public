using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MyProject
{
    public enum GameState
    {
        MainMenu,
        Gameplay,
        Pause,
    }

    public class GameplayManager : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private GUIManager GUIManager;

        [SerializeField] private ScoreKeeper scoreKeeper;

        [SerializeField] private EnemySpawner enemySpawner;

        [SerializeField] private MapGenerator mapGenerator;

        [SerializeField] private PlayerController playerController;

        #endregion

        #region Fields

        public static GameState ActiveState = GameState.MainMenu;

        public static event Action StartedGame;

        private int m_Score;

        private float m_WaitSecond = 1.0f;

        private WaitForSeconds m_WaitForSeconds;

        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            // 各コンポーネントの初期化
            playerController.Initialize();

            enemySpawner.Initialize();

            GUIManager.Initialize();

            // イベントの登録
            playerController.PlayerHealth.PlayerDied += OnPlayerDied;

            scoreKeeper.UpdatedScore += OnUpdatedScore;

            // 待機時間を初期化
            m_WaitForSeconds = new WaitForSeconds(m_WaitSecond);
        }

        private void Start()
        {
            // 各コンポーネントの依存関係を設定
            enemySpawner.SetMapGenerator(mapGenerator);

            mapGenerator.SetEnemySpawner(enemySpawner);

            scoreKeeper.SetEnemySpawner(enemySpawner);

            // スコアの初期化
            GUIManager.UpdateScoreText(m_Score);

            // GameStateの状態を確認
            if (ActiveState == GameState.MainMenu)
            {
                GUIManager.ShowMainMenu();
            }
            else if (ActiveState == GameState.Gameplay)
            {
                StartCoroutine(StartGame());
            }
        }

        private void OnDestroy()
        {
            playerController.PlayerHealth.PlayerDied -= OnPlayerDied;
            scoreKeeper.UpdatedScore -= OnUpdatedScore;
        }

        #endregion

        #region Methods

        public void Pause()
        {
            Time.timeScale = 0;

            GUIManager.ShowPause();
        }

        public void UnPause()
        {
            Time.timeScale = 1;

            GUIManager.HidePause();
        }

        public void ClickToStart()
        {
            StartCoroutine(StartGame());
        }

        public IEnumerator StartGame()
        {
            if (ActiveState == GameState.Gameplay)
                yield return m_WaitForSeconds;

            // MenuScreenのStart()の処理を待つ
            GUIManager.HideMainMenu();
            GUIManager.ShowHUD();

            if (ActiveState == GameState.MainMenu)
                yield return m_WaitForSeconds;

            // PlayerController, EnemySpawnerのStart()の処理を待って、イベントを通知
            StartedGame?.Invoke();
        }

        public void RetryGame()
        {
            ActiveState = GameState.Gameplay;
        }

        public void ReturnMainMenu()
        {
            ActiveState = GameState.MainMenu;
        }

        private void OnUpdatedScore(int score)
        {
            m_Score = score;

            GUIManager.UpdateScoreText(m_Score);
        }

        private void OnPlayerDied()
        {
            GUIManager.UpdateResultScoreText(m_Score);

            GUIManager.HideHUD();
            
            GUIManager.ShowResult();
        }

        #endregion
    }
}
