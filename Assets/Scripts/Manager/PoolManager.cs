using GameDevHQ.Manager.SpawnManagerNS;
using GameDevHQ.Other.MonoSingletonNS;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Tower.PlaceableAreaNS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameDevHQ.Manager.PoolManagerNS
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        [SerializeField]
        private GameObject[] _enemyPrefabs = null;
        [SerializeField]
        private GameObject _enemyContainer = null;
        private List<GameObject> _enemyPool = new List<GameObject>();

        [SerializeField]
        private GameObject[] _towerPrefabs = null;
        [SerializeField]
        private GameObject _towerContainer = null;
        [SerializeField]
        private int _baseNumberOfEachType = 4;
        private List<GameObject> _towerPool = new List<GameObject>();

        private void OnEnable()
        {
            SpawnManager.onGetEnemy += RequestInactiveEnemy;
            PlaceableArea.onRequestTower += RequestInactiveTower;
        }

        private void OnDisable()
        {
            SpawnManager.onGetEnemy -= RequestInactiveEnemy;
            PlaceableArea.onRequestTower -= RequestInactiveTower;
        }

        void Start()
        {
            int baseSpawnCount = SpawnManager.Instance.GetBaseSpawnCount();
            int numberOfWaves = SpawnManager.Instance.GetNumberOfWaves();

            _enemyPool = GenerateEnemies(baseSpawnCount, numberOfWaves);
            _towerPool = GenerateTowers(_baseNumberOfEachType);
        }

        List<GameObject> GenerateEnemies(int baseSpawnCount, int numberOfWaves)
        {
            int enemiesToCreate = baseSpawnCount * numberOfWaves;

            for (int i = 0; i < enemiesToCreate; i++)
            {
                int randomEnemy = Random.Range(0, _enemyPrefabs.Length);

                GameObject obj = Instantiate(_enemyPrefabs[randomEnemy], _enemyContainer.transform);
                obj.SetActive(false);
                _enemyPool.Add(obj);
            }
            return _enemyPool;
        }

        List<GameObject> GenerateTowers(int baseNumber)
        {
            int towersToCreate = baseNumber;

            for (int i = 0; i < towersToCreate; i++)
            {
                for (int n = 0; n < _towerPrefabs.Length; n++)
                {
                    GameObject obj = Instantiate(_towerPrefabs[n], _towerContainer.transform);
                    obj.SetActive(false);
                    _towerPool.Add(obj);
                }
            }
            return _towerPool;
        }

        public GameObject RequestInactiveEnemy()
        {
            GameObject selectedObj = _enemyPool.FirstOrDefault((enemy) => enemy.activeInHierarchy == false);

            if (selectedObj == null)
            {
                int randomEnemy = Random.Range(0, _enemyPrefabs.Length);

                selectedObj = Instantiate(_enemyPrefabs[randomEnemy], _enemyContainer.transform);
                _enemyPool.Add(selectedObj);
                Debug.Log("Created new Enemy Prefab.");
                return selectedObj;
            }
            else
            {
                return selectedObj;
            }
        }

        public GameObject RequestInactiveTower(int towerID)
        {
            GameObject selectedObj= _towerPool.FirstOrDefault((tower) => (tower.activeInHierarchy == false && tower.GetComponent<ITower>().TowerID == towerID));

            if (selectedObj == null)
            {
                foreach (var tower in _towerPrefabs)
                {
                    if (tower.GetComponent<ITower>().TowerID == towerID)
                    {
                        selectedObj = Instantiate(tower, _towerContainer.transform);
                        _towerPool.Add(selectedObj);
                        selectedObj.SetActive(false);
                        Debug.Log("Created new Tower Prefab of Type: " + towerID);
                        break;
                    }
                }
                return selectedObj;
            }
            else
            {
                return selectedObj;
            }
        }
    }
}

