using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyProject
{
    public class ScoreKeeper : MonoBehaviour
    {
        #region Fields

        public event Action<int> UpdatedScore;

        private int m_KillCount;

        private EnemySpawner m_EnemySpawner;

        #endregion

        #region Properties

        public int Score { get; private set; }

        #endregion

        #region MonoBehaviour

        private void OnDestroy()
        {
            m_EnemySpawner.KilledEnemy -= OnKilledEnemy;
        }

        #endregion

        #region Methods

        public void SetEnemySpawner(EnemySpawner enemySpawner)
        {
            m_EnemySpawner = enemySpawner;
            m_EnemySpawner.KilledEnemy += OnKilledEnemy;
        }

        private void OnKilledEnemy()
        {
            m_KillCount++;
            Score += m_KillCount;
            UpdatedScore?.Invoke(Score);
        }

        #endregion
    }
}
