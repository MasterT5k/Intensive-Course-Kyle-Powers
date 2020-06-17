using GameDevHQ.Enemy.EnemyClassNS;
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
        private int _baseSpawnCount = 10;
        [SerializeField]
        private int _numberOfWaves = 3;
        private int _currentWave = 1;
        private int _spawnedEnemies = 0;
        private int _activeEnemies = 0;
        private bool _wavesDone = false;
        private bool _firstWave = true;

        public static event Func<GameObject> onGetEnemy;

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
            StartCoroutine(SpawnCoroutine());
        }



        private IEnumerator SpawnCoroutine()
        {
            int amountToSpawn = _baseSpawnCount * _currentWave;
            _spawnedEnemies = 0;

            while (_spawnedEnemies < amountToSpawn)
            {
                _spawnedEnemies++;
                _activeEnemies++;

                yield return new WaitForSeconds(_spawnDelay);

                GameObject obj = onGetEnemy?.Invoke();

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

            if (_currentWave > _numberOfWaves)
            {
                _wavesDone = true;
            }
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
                    return;
                }

                StartCoroutine(SpawnCoroutine());
            }
        }

        public Transform GetEndPoint()
        {
            return _endPoint;
        }

        public int GetBaseSpawnCount()
        {
            return _baseSpawnCount;
        }

        public int GetNumberOfWaves()
        {
            return _numberOfWaves;
        }
    }
}

