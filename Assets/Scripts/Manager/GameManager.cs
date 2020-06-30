using GameDevHQ.Enemy.EnemyClassNS;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Manager.PoolManagerNS;
using GameDevHQ.Manager.UIManagerNS;
using GameDevHQ.Other.MonoSingletonNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Manager.GameManagerNS
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]
        private int _startingWarFunds = 0;
        [SerializeField]
        private float _refundPercent = 0.75f;
        private int _currentWarFunds;

        private void OnEnable()
        {
            EnemyClass.onDestroyed += EnemyDestroyed;
        }

        private void OnDisable()
        {
            EnemyClass.onDestroyed -= EnemyDestroyed;
        }

        // Start is called before the first frame update
        void Start()
        {
            _currentWarFunds = _startingWarFunds;
            UIManager.Instance.SetWarFundText(_currentWarFunds);
            ChangeTowerButtonCosts();
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
    }
}

