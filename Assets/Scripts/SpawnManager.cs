using System.Collections;
using System.Linq;
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
    private float _spawnDelay = 1f;
    [SerializeField]
    private int _baseSpawnCount = 10;
    private int _currentWave = 1;
    private int _spawnedEnemies = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator SpawnCoroutine()
    {
        int amountToSpawn = _baseSpawnCount * _currentWave;
        
        while (_spawnedEnemies < amountToSpawn)
        {
            _spawnedEnemies++;
            int randomEnemy = Random.Range(0, _enemyPrefabs.Length);

            yield return new WaitForSeconds(_spawnDelay);

            GameObject obj = Instantiate(_enemyPrefabs[randomEnemy], _startPoint.position, _startPoint.rotation);
            obj.transform.SetParent(_enemyContainer.transform);
        }
        _currentWave++;
        Debug.Log("Spawning is finished.");
    }
}
