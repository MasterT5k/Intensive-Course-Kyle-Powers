using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Other.MonoSingletonNS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameDevHQ.Manager.UIManagerNS
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField]
        private Text _fundsText = null;
        [SerializeField]
        private GameObject _gatlingUpgrade = null;
        [SerializeField]
        private GameObject _missileUpgrade = null;
        [SerializeField]
        private GameObject _dismantleWeapon = null;
        private bool _selectedTower;

        public static event Action<int> onTowerButtonClick;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            _gatlingUpgrade.SetActive(false);
            _missileUpgrade.SetActive(false);
            _dismantleWeapon.SetActive(false);
        }

        public void SetWarFundText(int warFunds)
        {
            _fundsText.text = warFunds.ToString();
        }

        public void TowerButtonClick(int selectedTower)
        {
            onTowerButtonClick?.Invoke(selectedTower);
        }

        public void PlacedTowerSelected(GameObject tower)
        {
            if (_selectedTower == true)
            {
                Debug.Log("Tower already selected.");
                return;
            }

            int towerID = tower.GetComponent<ITower>().TowerID;

            if (towerID == 0)
            {
                _selectedTower = tower;
                _gatlingUpgrade.SetActive(true);
            }
            else if (towerID == 1)
            {
                _selectedTower = tower;
                _missileUpgrade.SetActive(true);
            }
            else
            {
                Debug.Log("Tower can't be Upgraded.");
            }
        }

        public void UpgradeButtonClick(int towerID)
        {
            if (towerID < 0)
            {
                if (_gatlingUpgrade.activeInHierarchy == true)
                {
                    _gatlingUpgrade.SetActive(false);
                }

                if (_missileUpgrade.activeInHierarchy == true)
                {
                    _missileUpgrade.SetActive(false);
                }

                _selectedTower = false;
            }
            else if (towerID == 2)
            {
                Debug.Log("Place Dual Gatling Gun.");
            }
            else if (towerID == 3)
            {
                Debug.Log("Place Dual Missile Launcher.");
            }            
        }
    }
}

