using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
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
        int inactiveEnemy = -1;

        for (int i = 0; i < _enemyPool.Count; i++)
        {
            GameObject selectedObj = _enemyPool[i];
            if (selectedObj != null && selectedObj.activeInHierarchy == false)
            {
                inactiveEnemy = i;
            }
        }

        if (inactiveEnemy < 0)
        {
            int randomEnemy = Random.Range(0, _enemyPrefabs.Length);

            GameObject obj = Instantiate(_enemyPrefabs[randomEnemy], _enemyContainer.transform);
            _enemyPool.Add(obj);
            return obj;
        }
        else
        {
            GameObject obj = _enemyPool[inactiveEnemy];
            return obj;
        }
    }
}
