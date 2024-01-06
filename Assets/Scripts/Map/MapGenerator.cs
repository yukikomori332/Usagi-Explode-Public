using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MyProject
{
    public class MapGenerator : MonoBehaviour
    {
        [Serializable]
        public class Map
        {
            public Coordinate mapSize;
            
            public float minObstacleHeight;
            public float maxObstacleHeight;
            
            public Transform walls;

            public Coordinate mapCenter
            {
                get => new Coordinate(mapSize.x / 2 - 1, mapSize.y / 2 - 1); // 中心点から左下のタイル
            }
        }

        [Serializable]
        public class Coordinate
        {
            public int x;
            public int y;

            public Coordinate(int _x, int _y)
            {
                x = _x;
                y = _y;
            }
        }

        #region Inspector

        [Header("Map")]
        [SerializeField] private Map[] maps;

        [SerializeField] private int mapIndex;

        [SerializeField] private int[] targetWaveNumber;
        
        [SerializeField, Range(1.0f, 2.0f)] private float tileSize = 1.2f;

        [Header("Tile")]
        [SerializeField] private Transform tile;

        [SerializeField, Range(0, 1)] private float outlinePercent;     // タイルのアウトラインの割合

        [Header("Obstacle")]
        [SerializeField] private Transform obstacle;

        [SerializeField, Range(0, 1)] private float obstaclePercent;    // マップ全体に対する障害物の割合

        [Header("Color")]
        [SerializeField] private List<Color> colorPalette;

        [Header("Usable Items")]
        [SerializeField, Range(0, 2)] private int itemMaxCountToSpawn = 2;
        [SerializeField] private int itemTypes = 2;                     // 【要修正】アイテムの種類
        [SerializeField] private HealItem healItem;
        [SerializeField] private SpeedItem speedItem;

        #endregion

        #region Fields

        private Transform m_MapHolder;

        private MeshRenderer m_MeshRenderer;

        private Transform[,] m_MapTiles;

        private List<Coordinate> m_AllTileCoordinates = new List<Coordinate>();

        private Queue<Coordinate> m_ShuffledTileCoordinates;            // 先入れ先出し

        private Queue<Coordinate> m_ShuffledAccessibleTileCoordinates;  // 先入れ先出し

        private Map m_CurrentMap;

        private EnemySpawner m_EnemySpawner;

        #endregion

        #region MonoBehaviour

        private void OnDestroy()
        {
            m_EnemySpawner.FinishedWave -= OnFinishedWave;
        }

        #endregion

        #region Methods

        public void SetEnemySpawner(EnemySpawner enemySpawner)
        {
            m_EnemySpawner = enemySpawner;
            m_EnemySpawner.FinishedWave += OnFinishedWave;
        }

        public void GenerateMap()
        {
            m_CurrentMap = maps[mapIndex];                                                  // 現在のマップ
            m_MapTiles = new Transform[m_CurrentMap.mapSize.x , m_CurrentMap.mapSize.y];    // タイル座標

            string holderName = "Generated Map";

            // EditorがPlayモードでなければ
            if (!Application.isPlaying)
            {
                if (GetChildrenExtensions.GetChild(transform, holderName) != null)
                    DestroyImmediate(GetChildrenExtensions.GetChild(transform, holderName).gameObject);
            }
            else
            {
                if (GetChildrenExtensions.GetChild(transform, holderName) != null)
                    Destroy(GetChildrenExtensions.GetChild(transform, holderName).gameObject);
            }

            // mapHoloderインスタンスの作成
            m_MapHolder = new GameObject(holderName).transform;
            m_MapHolder.SetParent(transform);

            // 全てのタイルの座標を作成
            for (int x = 0; x < m_CurrentMap.mapSize.x; x++)
            {
                for (int y = 0; y < m_CurrentMap.mapSize.y; y++)
                {
                    m_AllTileCoordinates.Add(new Coordinate(x, y));
                }
            }
            m_ShuffledTileCoordinates = new Queue<Coordinate>(ArrayExtensions.Shuffle(m_AllTileCoordinates.ToArray()));
            
            // タイルのインスタンスを作成
            for (int x = 0; x < m_CurrentMap.mapSize.x; x++)
            {
                for (int y = 0; y < m_CurrentMap.mapSize.y; y++)
                {
                    Vector3 tilePosition = CoordinateToPosition(x, y);
                    Transform newTile = Instantiate(tile, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                    newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;

                    newTile.SetParent(m_MapHolder);
                    m_MapTiles[x, y] = newTile; // タイル座標の格納
                }
            }

            bool[,] obstacleMap = new bool[m_CurrentMap.mapSize.x, m_CurrentMap.mapSize.y];
            int obstacleCount = Mathf.RoundToInt(m_CurrentMap.mapSize.x * m_CurrentMap.mapSize.y * obstaclePercent);
            int currentObstacleCount = 0;
            List<Coordinate> accessibleTileCoordinates = new List<Coordinate>(m_AllTileCoordinates);
            Color obstacleColor;

            // 障害物の色を決定
            if (mapIndex == 0)
                obstacleColor = colorPalette[0];
            else
                obstacleColor = colorPalette[Random.Range(1, colorPalette.Count)];

            // 障害物のインスタンスを作成
            for (int i = 0; i < obstacleCount; i++)
            {
                Coordinate randomCoordinate = GetRandomCoordinate();
                obstacleMap[randomCoordinate.x, randomCoordinate.y] = true;
                currentObstacleCount++;

                // マップの中心座標でなく、隣接するタイルにアクセスできるなら
                if (m_CurrentMap.mapCenter.x != randomCoordinate.x && m_CurrentMap.mapCenter.y != randomCoordinate.y && IsNeighborTileAccessible(obstacleMap, currentObstacleCount))
                {
                    float obstacleHeight = (float)Random.Range(m_CurrentMap.minObstacleHeight, m_CurrentMap.maxObstacleHeight);
                    Vector3 obstaclePosition = CoordinateToPosition(randomCoordinate.x, randomCoordinate.y);
                    Transform newObstacle = Instantiate(obstacle, obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;
                    
                    newObstacle.eulerAngles = new Vector3(0, 90 * Random.Range(0, 4), 0);
                    newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

                    // 障害物の色を変更
                    newObstacle.TryGetComponent<MeshRenderer>(out m_MeshRenderer);
                    if (m_MeshRenderer != null)
                    {
                        Material tempMaterial = new Material(m_MeshRenderer.sharedMaterial);
                        float colorPercent = newObstacle.localScale.y / m_CurrentMap.mapSize.y;

                        tempMaterial.color = Color.Lerp(obstacleColor, Color.black, colorPercent);
                        m_MeshRenderer.sharedMaterial = tempMaterial;
                    }
                        
                    newObstacle.SetParent(m_MapHolder);

                    accessibleTileCoordinates.Remove(randomCoordinate); // 利用可能でないタイル座標の削除
                }
                else
                {
                    obstacleMap[randomCoordinate.x, randomCoordinate.y] = false;
                    currentObstacleCount--;
                }
            }

            // 利用可能なタイル座標を作成
            m_ShuffledAccessibleTileCoordinates = new Queue<Coordinate>(ArrayExtensions.Shuffle(accessibleTileCoordinates.ToArray()));

            // Mapを囲むWallsを作成
            m_CurrentMap.walls.gameObject.SetActive(true);
        }

        public Coordinate GetRandomCoordinate()
        {
            Coordinate randomCoordinate = m_ShuffledTileCoordinates.Dequeue();
            m_ShuffledTileCoordinates.Enqueue(randomCoordinate);
            return randomCoordinate;
        }

        public Transform GetRandomAccessibleTile()
        {
            Coordinate randomCoordinate = m_ShuffledAccessibleTileCoordinates.Dequeue();
            m_ShuffledAccessibleTileCoordinates.Enqueue(randomCoordinate);
            return m_MapTiles[randomCoordinate.x, randomCoordinate.y];
        }

        public Transform GetPlayerNearbyPosition(Vector3 playerPosition)
        {
            int x = Mathf.RoundToInt(playerPosition.x / tileSize + (m_CurrentMap.mapSize.x - 1) / 2f);
            int y = Mathf.RoundToInt(playerPosition.z / tileSize + (m_CurrentMap.mapSize.y - 1) / 2f);
            x = Mathf.Clamp(x, 0, m_MapTiles.GetLength(0));
            y = Mathf.Clamp(y, 0, m_MapTiles.GetLength(1));

            return m_MapTiles[x, y];
        }

        private Vector3 CoordinateToPosition(int x, int y)
        {
            return new Vector3(-m_CurrentMap.mapSize.x / 2f + 0.5f + x, 0, -m_CurrentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
        }

        private bool IsNeighborTileAccessible(bool[,] obstacleMap, int currentObstacleCount)
        {
            bool[,] flags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
            Queue<Coordinate> queue = new Queue<Coordinate>();
            queue.Enqueue(m_CurrentMap.mapCenter);
            flags[m_CurrentMap.mapCenter.x, m_CurrentMap.mapCenter.y] = true;

            int accessibleTileCount = 1;
            while (queue.Count > 0)
            {
                Coordinate coord = queue.Dequeue();

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        int neighborX = coord.x + x;
                        int neighborY = coord.y + y;
                        if (x == 0 || y == 0)
                        {
                            if (neighborX >= 0 && neighborX < obstacleMap.GetLength(0)
                                && neighborY >= 0 && neighborY < obstacleMap.GetLength(1))
                            {
                                if (!flags[neighborX, neighborY] && !obstacleMap[neighborX, neighborY])
                                {
                                    flags[neighborX, neighborY] = true;
                                    queue.Enqueue(new Coordinate(neighborX, neighborY));

                                    accessibleTileCount++;
                                }
                            }
                        }
                    }
                }
            }

            int targetAccessibleTileCount = m_CurrentMap.mapSize.x * m_CurrentMap.mapSize.y - currentObstacleCount;
            return targetAccessibleTileCount == accessibleTileCount;
        }

        private bool CheckTargetWaveNumber(int number)
        {
            if (mapIndex > targetWaveNumber.Length - 1) return false;

            if (targetWaveNumber[mapIndex] == number)
                return true;
            else
                return false;
        }

        private void OnFinishedWave(int waveNumber)
        {
            if (CheckTargetWaveNumber(waveNumber - 1))
            {
                m_CurrentMap.walls.gameObject.SetActive(false);
                mapIndex++;
            }

            GenerateMap();

            GenerateItems();
        }

        private void GenerateItems()
        {
            int spawnCount = Random.Range(0, itemMaxCountToSpawn);
            //Debug.Log($"アイテムを生成する数は{spawnCount}");

            for (int i = 0; i < spawnCount; i++)
            {
                Transform spawnTile = GetRandomAccessibleTile();

                int itemNumber = Random.Range(0, itemTypes);
                //Debug.Log($"生成するアイテム番号は{itemNumber}");
                switch (itemNumber)
                {
                    case 0:
                        HealItem spawnedHealItem = Instantiate(healItem, spawnTile.position + Vector3.up * 0.5f, Quaternion.identity) as HealItem;
                        spawnedHealItem.transform.eulerAngles = new Vector3(30, 0, 0);
                        //Debug.Log($"生成したアイテムは{spawnedHealItem}");
                        spawnedHealItem.transform.SetParent(m_MapHolder);
                        break;
                    case 1:
                        SpeedItem spawnedSpeedItem = Instantiate(speedItem, spawnTile.position + Vector3.up * 0.5f, Quaternion.identity) as SpeedItem;
                        spawnedSpeedItem.transform.eulerAngles = new Vector3(30, 0, 0);
                        //Debug.Log($"生成したアイテムは{spawnedSpeedItem}");
                        spawnedSpeedItem.transform.SetParent(m_MapHolder);
                        break;
                }
            }
        }

        #endregion
    }
}
