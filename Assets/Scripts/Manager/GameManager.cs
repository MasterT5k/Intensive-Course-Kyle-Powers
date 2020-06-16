using GameDevHQ.Enemy.EnemyClassNS;
using GameDevHQ.Manager.UIManagerNS;
using GameDevHQ.Other.MonoSingletonNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Manager.GameManager
{
    public class GameManager : MonoSingleton<GameManager>
    {
        [SerializeField]
        private int _startingWarFunds = 0;
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
    }
}

