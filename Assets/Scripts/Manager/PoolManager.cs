using GameDevHQ.Manager.SpawnManagerNS;
using GameDevHQ.Other.MonoSingletonNS;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Tower.PlaceableAreaNS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameDevHQ.Enemy.EnemyClassNS;

namespace GameDevHQ.Manager.PoolManagerNS
{
    public class PoolManager : MonoSingleton<PoolManager>
    {
        [SerializeField]
        private Enemies _enemyList = null;
        [SerializeField]
        private GameObject _enemyContainer = null;
        [SerializeField]
        private int _baseNumberOfEachEnemy = 10;
        private List<GameObject> _enemyPool = new List<GameObject>();

        [SerializeField]
        private GameObject[] _towerPrefabs = null;
        [SerializeField]
        private GameObject _towerContainer = null;
        [SerializeField]
        private int _baseNumberOfEachTower = 4;
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
            _enemyList.SetClassList();

            _enemyPool = GenerateEnemies(_baseNumberOfEachEnemy);
            _towerPool = GenerateTowers(_baseNumberOfEachTower);

            foreach (var tower in _towerPrefabs)
            {
                tower.GetComponent<ITower>().Init();
            }
        }

        List<GameObject> GenerateEnemies(int baseSpawnCount)
        {
            for (int i = 0; i < baseSpawnCount; i++)
            {
                for (int n = 0; n < _enemyList.enemyScriptables.Length; n++)
                {
                    GameObject obj = Instantiate(_enemyList.GetEnemyPrefab(n), _enemyContainer.transform);
                    obj.SetActive(false);
                    _enemyPool.Add(obj);
                }
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

        public GameObject RequestInactiveEnemy(int enemyID)
        {
            GameObject selectedObj = _enemyPool.FirstOrDefault((enemy) => (enemy.activeInHierarchy == false && enemy.GetComponent<EnemyClass>().GetEnemyID() == enemyID));

            if (selectedObj == null)
            {
                selectedObj = Instantiate(_enemyList.GetEnemyPrefab(enemyID), _enemyContainer.transform);
                _enemyPool.Add(selectedObj);
                //Debug.Log("Created new Enemy Prefab.");
                return selectedObj;
            }
            else
            {
                return selectedObj;
            }
        }

        public GameObject RequestInactiveTower(int towerID)
        {
            GameObject selectedObj = _towerPool.FirstOrDefault((tower) => (tower.activeInHierarchy == false && tower.GetComponent<ITower>().TowerID == towerID));

            if (selectedObj == null)
            {
                foreach (var tower in _towerPrefabs)
                {
                    if (tower.GetComponent<ITower>().TowerID == towerID)
                    {
                        selectedObj = Instantiate(tower, _towerContainer.transform);
                        _towerPool.Add(selectedObj);
                        selectedObj.SetActive(false);
                        //Debug.Log("Created new Tower Prefab of Type: " + towerID);
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

        public int GetTowerCost(int towerID)
        {
            int towerCost = 0;

            for (int i = 0; i < _towerPool.Count; i++)
            {
                ITower tower = _towerPool[i].GetComponent<ITower>();
                if (tower.TowerID == towerID)
                {
                    towerCost = tower.WarFundValue;
                    break;
                }
            }
            return towerCost;
        }
    }
}

