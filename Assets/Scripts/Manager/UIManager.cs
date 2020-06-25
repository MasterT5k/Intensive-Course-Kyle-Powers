using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Manager.GameManagerNS;
using GameDevHQ.Manager.PoolManagerNS;
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
        private GameObject _gunUpgrade = null;
        [SerializeField]
        private Text _gunFunds = null;
        [SerializeField]
        private GameObject _missileUpgrade = null;
        [SerializeField]
        private Text _missileFunds = null;
        [SerializeField]
        private GameObject _dismantleWeapon = null;
        [SerializeField]
        private Text _dismantleFunds = null;
        private bool _towerSelected;
        private GameObject _selectedTower;

        public static event Action<int> onTowerButtonClick;
        public static event Action<int, GameObject> onUpgradeButtonClick;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            _gunUpgrade.SetActive(false);
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

        public void PlacedTowerSelected(GameObject selectedTower)
        {
            if (_towerSelected == true)
            {
                Debug.Log("Tower already selected.");
                return;
            }

            _selectedTower = selectedTower;
            ITower tower = _selectedTower.GetComponent<ITower>();
            int towerID = tower.TowerID;

            switch (towerID)
            {
                case 0:
                    _towerSelected = true;
                    _gunUpgrade.SetActive(true);
                    UpdateFunds(_gunFunds, 2);
                    _dismantleWeapon.SetActive(true);
                    UpdateFunds(_dismantleFunds, towerID);
                    break;
                case 1:
                    _towerSelected = true;
                    _missileUpgrade.SetActive(true);
                    UpdateFunds(_missileFunds, 3);
                    _dismantleWeapon.SetActive(true);
                    UpdateFunds(_dismantleFunds, towerID);
                    break;
                default:
                    Debug.Log("Tower can't be Upgraded.");
                    _dismantleWeapon.SetActive(true);
                    UpdateFunds(_dismantleFunds, towerID);
                    break;
            }
        }

        public void UpgradeButtonClick(int towerID)
        {
            switch (towerID)
            {
                case -1:
                    ClearSelection();
                    break;
                case 2:
                    Debug.Log("Place Dual Gatling Gun.");
                    onUpgradeButtonClick?.Invoke(2, _selectedTower);
                    break;
                case 3:
                    Debug.Log("Place Dual Missile Launcher.");
                    onUpgradeButtonClick?.Invoke(3, _selectedTower);
                    break;
                default:
                    break;
            }       
        }

        public void DismantleButtonClick(bool dismantle)
        {
            if (dismantle == true)
            {
                GameManager.Instance.TowerDismantled(_selectedTower);
                ClearSelection();
            }
            else
            {
                ClearSelection();
            }
        }

        public void UpdateFunds(Text funds, int towerID)
        {
            int amount = PoolManager.Instance.GetTowerCost(towerID);
            funds.text = amount.ToString();
        }

        public void ClearSelection()
        {
            _towerSelected = false;
            _selectedTower = null;
            _gunUpgrade.SetActive(false);
            _missileUpgrade.SetActive(false);
            _dismantleWeapon.SetActive(false);
        }
    }
}

