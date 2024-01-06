using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;

namespace MyProject
{
    public class EnemySpawner : MonoBehaviour
    {
        [Serializable]
        public class Wave
        {
            public bool infinite;
            public int enemyCount;
            public float timeBetweenSpawns;

            // 【拡張】エネミーのステータスを変更
            //public float moveSpeed;
            //public int hitsToKillPlayer;
            //public int float helath;
        }

        #region Inspector

        [Header("Wave")]
        [SerializeField] private Wave[] waves;

        [SerializeField] private Enemy enemy;

        [Header("Check Camping")]
        [SerializeField] private float timeBetweenCheckCamping = 2f;

        [SerializeField] private float campingThresholdDistance = 1.5f;

        [Header("HUD")]
        [SerializeField] private TextMeshProUGUI countText;

        #endregion

        #region Fields

        public event Action<int> FinishedWave;

        public event Action KilledEnemy;

        private int m_CurrentWaveNumber;

        private int m_EnemiesRemainingToSpawn; // スポーンさせる敵の数

        private int m_EnemiesRemainingAlive; // 生きている敵の数

        private float m_NextSpawnTime;

        private float m_CheckCampingTime;

        private Vector3 m_PrevPlayerPosition;

        private Vector3 m_InitPlayerPosition;

        private bool m_IsCamping;

        private Wave m_CurrentWave;

        private MapGenerator m_MapGenerator;

        private PlayerHealth m_PlayerHealth;

        #endregion

        #region MonoBehaviour

        private void OnDestroy()
        {
            GameplayManager.StartedGame -= OnStartedGame;
        }

        private void Start()
        {
            GameplayManager.StartedGame += OnStartedGame;
        }

        private void Update()
        {
            // 現在のウェーブが取得できないまたは、PlayerHealthが取得できない、Playerが死んでいれば
            if (m_CurrentWave == null || m_PlayerHealth == null || m_PlayerHealth.Dead) return;

            // Playerの停留を確認する
            m_CheckCampingTime += Time.deltaTime;
            if (m_CheckCampingTime > timeBetweenCheckCamping)
            {
                m_CheckCampingTime = 0;

                m_IsCamping = Vector3.Distance(m_PlayerHealth.transform.position, m_PrevPlayerPosition) < campingThresholdDistance;
                m_PrevPlayerPosition = m_PlayerHealth.transform.position;
            }

            // Enemyのスポーンが可能なら
            m_NextSpawnTime += Time.deltaTime;
            if ((m_EnemiesRemainingToSpawn > 0 || m_CurrentWave.infinite) && m_NextSpawnTime > m_CurrentWave.timeBetweenSpawns)
            {
                m_EnemiesRemainingToSpawn--;    // スポーンさせる敵の数
                m_NextSpawnTime = 0;            // スポーンタイマーの初期化

                StartCoroutine(SpawnEnemy());
            }
        }

        #endregion

        #region Methods

        public virtual void Initialize()
        {
            foreach (PlayerController playerController in PlayerController.playerControllers)
            {
                if (playerController.name == "Player")
                {
                    // プレイヤーのHPを取得
                    m_PlayerHealth = playerController.PlayerHealth;

                    m_PrevPlayerPosition = m_PlayerHealth.transform.position;
                    m_InitPlayerPosition = m_PlayerHealth.transform.position;
                }
            }
        }

        public void SetMapGenerator(MapGenerator mapGenerator)
        {
            m_MapGenerator = mapGenerator;
        }

        private IEnumerator SpawnEnemy()
        {
            float spawnDelay = 1f;
            float flashSpeed = 4f;
            float flashTimer = 0f;
            Transform spawnTile = m_IsCamping ?
                m_MapGenerator.GetPlayerNearbyPosition(m_PrevPlayerPosition) : m_MapGenerator.GetRandomAccessibleTile();

            spawnTile.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer);
            if (meshRenderer != null)
            {
                Material spawnMat = meshRenderer.material;
                Color defaultColor = spawnMat.color;
                Color flashColor = Color.red;

                while (spawnDelay > flashTimer)
                {
                    if (spawnMat != null)
                        spawnMat.color = Color.Lerp(defaultColor, flashColor, Mathf.PingPong(flashTimer * flashSpeed, 1f));

                    flashTimer += Time.deltaTime;

                    // シーンの遷移によるNull参照を防ぐ
                    if (spawnMat == null)
                    {
                        spawnMat.color = defaultColor;
                        break;
                    }

                    yield return null;
                }
                spawnMat.color = defaultColor;
                //if (spawnMat != null)
                    //spawnMat.color = defaultColor;
            }

            Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
            spawnedEnemy.Initialize();
            spawnedEnemy.DiedEnemy += OnEnemyDied;

            // 【拡張】エネミーのステータスを変更
            // - ここに処理を書く -
        }

        private void OnEnemyDied()
        {
            m_EnemiesRemainingAlive--; // 生きている敵の数

            KilledEnemy?.Invoke();

            // 生きているEnemyの残数を更新
            if (m_CurrentWave.infinite)
                countText.SetText("∞");
            else
                countText.SetText(m_EnemiesRemainingAlive.ToString());

            // Enemyが全滅していれば
            if (m_EnemiesRemainingAlive == 0)
            {
                NextWave();
            }
        }

        private void NextWave()
        {
            m_CurrentWaveNumber++;　// 現在のウェーブ数

            if (m_CurrentWaveNumber - 1 < waves.Length)
            {
                m_CurrentWave = waves[m_CurrentWaveNumber - 1];           // 現在のウェーブを変更

                m_EnemiesRemainingToSpawn = m_CurrentWave.enemyCount;     // スポーンさせる敵の数
                m_EnemiesRemainingAlive = m_EnemiesRemainingToSpawn;    // 生きている敵の数

                // 生きているEnemyの残数を更新
                if (m_CurrentWave.infinite)
                    countText.SetText("∞");
                else
                    countText.SetText(m_EnemiesRemainingAlive.ToString());

                // MapGeneratorにイベントを通知
                FinishedWave?.Invoke(m_CurrentWaveNumber);

                // PlayerのPositionをリセット
                ResetPlayerPosition();
            }
        }

        private void ResetPlayerPosition()
        {
            m_PlayerHealth.transform.position = m_InitPlayerPosition;
        }

        private void OnStartedGame()
        {
            NextWave();
        }

        #endregion
    }
}
