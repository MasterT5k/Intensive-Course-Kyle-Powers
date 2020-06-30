using GameDevHQ.Enemy.EnemyClassNS;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Manager.PoolManagerNS;
using GameDevHQ.Manager.SpawnManagerNS;
using GameDevHQ.Manager.UIManagerNS;
using GameDevHQ.Other.MonoSingletonNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameDevHQ.Manager.GameManagerNS
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]
        private int _startingLives = 100;
        [SerializeField]
        private int _startingWarFunds = 0;
        [SerializeField]
        private float _refundPercent = 0.75f;
        private int _currentLives;
        private int _currentWarFunds;

        private void OnEnable()
        {
            EnemyClass.onDestroyed += EnemyDestroyed;
            EnemyClass.onReachedEnd += EnemyReachedEnd;
        }

        private void OnDisable()
        {
            EnemyClass.onDestroyed -= EnemyDestroyed;
            EnemyClass.onReachedEnd -= EnemyReachedEnd;
        }

        // Start is called before the first frame update
        void Start()
        {
            _currentLives = _startingLives;
            _currentWarFunds = _startingWarFunds;
            UIManager.Instance.SetLivesCount(_currentLives);
            UIManager.Instance.SetWarFundText(_currentWarFunds);
            ChangeTowerButtonCosts();
            StartCoroutine(WaveComplete());
        }

        public void ChangeFunds(int amount, bool addFunds)
        {
            if (addFunds == true)
            {
                _currentWarFunds += amount;
            }
            else
            {
                _currentWarFunds -= amount;
            }
            UIManager.Instance.SetWarFundText(_currentWarFunds);
        }

        private void EnemyDestroyed(int currencyValue)
        {
            ChangeFunds(currencyValue, true);
        }

        private void EnemyReachedEnd(int livesCost)
        {
            ChangeLives(livesCost, false);
        }

        public void TowerDismantled(GameObject tower)
        {
            int towerCost = tower.GetComponent<ITower>().WarFundValue;
            int refund = Mathf.RoundToInt(towerCost * _refundPercent);
            ChangeFunds(refund, true);
            tower.SetActive(false);
        }

        public bool CheckFunds(int cost)
        {
            if (_currentWarFunds >= cost)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ChangeTowerButtonCosts()
        {
            int towerCost = PoolManager.Instance.GetTowerCost(0);
            UIManager.Instance.SetBaseTowerCosts(0, towerCost);

            towerCost = PoolManager.Instance.GetTowerCost(1);
            UIManager.Instance.SetBaseTowerCosts(1, towerCost);
        }

        public void ChangeLives(int amount, bool addLives)
        {
            if (addLives == true)
            {
                _currentLives += amount;
            }
            else
            {
                _currentLives -= amount;
                if (_currentLives < 1)
                {
                    _currentLives = 0;
                    LevelFinished();
                }
                UIManager.Instance.UpdateLivesUI(_currentLives);
            }
        }

        public void LevelFinished()
        {
            if (_currentLives > 0)
            {
                UIManager.Instance.LevelEnd(true);
            }
            else
            {
                UIManager.Instance.LevelEnd(false);
            }
        }

        public void RestartLevel()
        {
            SceneManager.LoadScene(0);
        }

        public IEnumerator WaveComplete(bool wavesDone = false)
        {
            yield return new WaitForSeconds(5f);
            SpawnManager.Instance.StartSpawn();
        }
    }
}

