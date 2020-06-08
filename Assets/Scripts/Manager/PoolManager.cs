using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolManager : MonoSingleton<PoolManager>
{
    [SerializeField]
    private GameObject[] _enemyPrefabs = null;
    [SerializeField]
    private GameObject _enemyContainer = null;
    private List<GameObject> _enemyPool = new List<GameObject>();

    private void OnEnable()
    {
        SpawnManager.GetEnemy += RequestInactiveEnemy;
    }

    private void OnDisable()
    {
        SpawnManager.GetEnemy -= RequestInactiveEnemy;
    }

    void Start()
    {
        int baseSpawnCount = SpawnManager.Instance.GetBaseSpawnCount();
        int numberOfWaves = SpawnManager.Instance.GetNumberOfWaves();

        _enemyPool = GenerateEnemies(baseSpawnCount, numberOfWaves);
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
}
