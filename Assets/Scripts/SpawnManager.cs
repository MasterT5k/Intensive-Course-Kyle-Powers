using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [SerializeField]
    private GameObject[] _enemyPrefabs = null;
    [SerializeField]
    private GameObject _enemyContainer = null;
    [SerializeField]
    private Transform _startPoint = null;
    [SerializeField]
    private Transform _endPoint = null;
    [SerializeField]
    private float _spawnDelay = 1f;
    [SerializeField]
    private int _baseSpawnCount = 10;
    [SerializeField]
    private int _numberOfWaves = 3;
    private int _currentWave = 1;
    private int _spawnedEnemies = 0;
    private bool _wavesDone = false;
    private List<GameObject> _enemyPool = new List<GameObject>();

    private void Start()
    {
        _enemyPool = GenerateEnemies();
        StartCoroutine(SpawnCoroutine());
    }

    List<GameObject> GenerateEnemies()
    {
        int enemiesToCreate = _baseSpawnCount * _numberOfWaves;

        for (int i = 0; i < enemiesToCreate; i++)
        {
            int randomEnemy = Random.Range(0, _enemyPrefabs.Length);

            GameObject obj = Instantiate(_enemyPrefabs[randomEnemy], _enemyContainer.transform);
            obj.SetActive(false);
            _enemyPool.Add(obj);
        }
        return _enemyPool;
    }

    private IEnumerator SpawnCoroutine()
    {
        int amountToSpawn = _baseSpawnCount * _currentWave;
        
        while (_spawnedEnemies < amountToSpawn)
        {
            _spawnedEnemies++;

            int inactiveEnemy = -1;

            for (int i = 0; i < _enemyPool.Count; i++)
            {
                GameObject selectedObj = _enemyPool[i];
                if (selectedObj.activeInHierarchy == false)
                {
                    inactiveEnemy = i;
                }
            }

            yield return new WaitForSeconds(_spawnDelay);

            if (inactiveEnemy < 0)
            {
                int randomEnemy = Random.Range(0, _enemyPrefabs.Length);

                GameObject obj = Instantiate(_enemyPrefabs[randomEnemy], _enemyContainer.transform);
                _enemyPool.Add(obj);
                obj.transform.position = _startPoint.position;
                obj.transform.rotation = _startPoint.rotation;
            }
            else
            {
                GameObject obj = _enemyPool[inactiveEnemy];
                obj.transform.position = _startPoint.position;
                obj.transform.rotation = _startPoint.rotation;
                obj.SetActive(true);
            }

        }
        _currentWave++;

        if (_currentWave > _numberOfWaves)
        {
            _wavesDone = true;
        }
    }

    public void Despawn()
    {
        _spawnedEnemies--;
        if (_spawnedEnemies < 1)
        {
            if (_wavesDone == true)
            {
                return;
            }
            StartCoroutine(SpawnCoroutine());
        }
    }

    public Transform GetEndPoint()
    {
        return _endPoint;
    }
}
