using GameDevHQ.Enemy.EnemyClassNS;
using GameDevHQ.Manager.GameManagerNS;
using GameDevHQ.Manager.UIManagerNS;
using GameDevHQ.Other.MonoSingletonNS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Manager.SpawnManagerNS
{
    public class SpawnManager : MonoSingleton<SpawnManager>
    {
        [SerializeField]
        private Transform _startPoint = null;
        [SerializeField]
        private Transform _endPoint = null;
        [SerializeField]
        private float _spawnDelay = 1f;
        [SerializeField]
        private List<Wave> _waves = new List<Wave>();
        private int _currentWave = 1;
        private int _spawnedEnemies = 0;
        private int _activeEnemies = 0;
        private bool _wavesDone = false;
        private bool _firstWave = true;
        private bool _spawning = false;
        private WaitForSeconds _spawnYield;

        public static event Func<int,GameObject> onGetEnemy;

        private void OnEnable()
        {
            EnemyClass.onDisabled += Despawn;
            EnemyClass.onGetEndPoint += GetEndPoint;
        }

        private void OnDisable()
        {
            EnemyClass.onDisabled -= Despawn;
            EnemyClass.onGetEndPoint -= GetEndPoint;
        }

        private void Start()
        {
            _spawnYield = new WaitForSeconds(_spawnDelay);
        }

        public void StartSpawn()
        {
            StartCoroutine(SpawnCoroutine());
        }

        private IEnumerator SpawnCoroutine()
        {
            if (_spawning == true)
            {
                yield break;
            }

            _spawning = true;

            Wave wave = _waves[_currentWave - 1];

            int amountToSpawn = wave.enemiesToSpawn.Count;
            _spawnedEnemies = 0;

            SendWaveInfo(_currentWave, _waves.Count);

            while (_spawnedEnemies < amountToSpawn)
            {
                int enemyID = wave.enemiesToSpawn[_spawnedEnemies].enemyClass.GetEnemyID();
                _spawnedEnemies++;
                _activeEnemies++;

                yield return _spawnYield;

                GameObject obj = onGetEnemy?.Invoke(enemyID);

                if (obj == null)
                {
                    Debug.LogError("Didn't get an Enemy back from Pool Manager.");
                    yield break;
                }
                obj.transform.position = _startPoint.position;
                obj.transform.rotation = _startPoint.rotation;
                obj.SetActive(true);
            }

            _currentWave++;

            if (_firstWave == true)
            {
                _firstWave = false;
            }

            if (_currentWave > _waves.Count)
            {
                _wavesDone = true;
            }

            _spawning = false;
        }

        public void Despawn()
        {
            if (_firstWave == true)
            {
                return;
            }

            _activeEnemies--;

            if (_activeEnemies < 1)
            {
                if (_wavesDone == true)
                {
                    GameManager.Instance.WaveComplete(true);
                    return;
                }
                GameManager.Instance.WaveComplete(false);
            }
        }

        public Transform GetStartPoint()
        {
            return _startPoint;
        }

        public Transform GetEndPoint()
        {
            return _endPoint;
        }

        public float GetSpawnDelay()
        {
            return _spawnDelay;
        }

        public List<Wave> GetWaves()
        {
            return _waves;
        }

        public void SendWaveInfo(int currentWave, int numberOfWaves)
        {
            UIManager.Instance.UpdateWaveCount(currentWave, numberOfWaves);
        }

        public void UpdateSpawnManager(Transform startPoint, Transform endPoint, float spawnDelay, List<Wave> waves)
        {
            _startPoint = startPoint;
            _endPoint = endPoint;
            _spawnDelay = spawnDelay;
            List<Wave> tempWaves = waves;
            _waves.Clear();
            for (int i = 0; i < tempWaves.Count; i++)
            {
                _waves.Add(tempWaves[i]);
            }
        }
    }
}

