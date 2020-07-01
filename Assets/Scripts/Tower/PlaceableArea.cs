using GameDevHQ.Manager.GameManagerNS;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Tower.TowerPlacementNS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevHQ.FileBase.Gatling_Gun;
using GameDevHQ.FileBase.Missile_Launcher;
using GameDevHQ.Manager.UIManagerNS;
using GameDevHQ.FileBase.Dual_Gatling_Gun;
using GameDevHQ.FileBase.Missle_Launcher_Dual_Turret;

namespace GameDevHQ.Tower.PlaceableAreaNS
{
    public class PlaceableArea : MonoBehaviour
    {
        private GameObject _particleObj;
        private GameObject _placedTower;
        private bool _canTakeTower = true;
        private bool _inPlaceMode = false;

        public static event Action<bool> onCanPlaceHere;
        public static event Func<int> onGetSelectedTowerID;
        public static event Func<int, GameObject> onRequestTower;

        private void OnEnable()
        {
            TowerPlacement.onSelectTower += PlaceMode;
            Gatling_Gun.onDestroyed += RemoveTower;
            Missile_Launcher.onDestroyed += RemoveTower;
            Dual_Gatling_Gun.onDestroyed += RemoveTower;
            Missle_Launcher.onDestroyed += RemoveTower;
            UIManager.onUpgradeButtonClick += PlaceTower;
        }

        private void OnDisable()
        {
            TowerPlacement.onSelectTower -= PlaceMode;
            Gatling_Gun.onDestroyed -= RemoveTower;
            Missile_Launcher.onDestroyed -= RemoveTower;
            Dual_Gatling_Gun.onDestroyed -= RemoveTower;
            Missle_Launcher.onDestroyed -= RemoveTower;
            UIManager.onUpgradeButtonClick -= PlaceTower;
        }

        void Start()
        {
            _particleObj = transform.GetComponentInChildren<ParticleSystem>(true).gameObject;

            if (_particleObj != null)
            {
                _particleObj.SetActive(false);
            }
            else
            {
                Debug.LogError("Particle System GameObject is NULL.");
            }
        }

        private void OnMouseEnter()
        {
            if (_inPlaceMode == true)
            {
                onCanPlaceHere?.Invoke(_canTakeTower);
            }
        }

        private void OnMouseExit()
        {
            if (_inPlaceMode == true)
            {
                onCanPlaceHere?.Invoke(false);
            }
        }

        private void OnMouseDown()
        {
            if (_inPlaceMode == true && _canTakeTower == true)
            {
                int towerID = (int)(onGetSelectedTowerID?.Invoke());
                PlaceTower(towerID);
            }
            else if (_inPlaceMode == false && _canTakeTower == false)
            {
                Debug.Log("Upgrade Tower.");
                UIManager.Instance.PlacedTowerSelected(_placedTower);
            }
        }

        private void PlaceTower(int selectedTowerID, GameObject tower = null)
        {
            if (selectedTowerID < 0)
            {
                Debug.Log("Select a Tower to place.");
                return;
            }

            if (tower != null)
            {
                if (_placedTower != tower)
                {
                    return;
                }
            }

            GameObject towerToPlace;
            towerToPlace = onRequestTower?.Invoke(selectedTowerID);
            Debug.Log(towerToPlace.name);
            int towerCost = towerToPlace.GetComponent<ITower>().WarFundValue;
            bool haveFunds = GameManager.Instance.CheckFunds(towerCost);

            if (haveFunds)
            {
                towerToPlace.transform.position = _particleObj.transform.position;
                towerToPlace.SetActive(true);
                if (_placedTower != null)
                {
                    _placedTower.SetActive(false);
                    UIManager.Instance.ClearSelection();
                }
                _placedTower = towerToPlace;
                _canTakeTower = false;
                _particleObj.SetActive(false);
                onCanPlaceHere?.Invoke(_canTakeTower);
                GameManager.Instance.ChangeFunds(towerCost, false);
            }
            else
            {
                Debug.Log("Not enough in the War Fund to build Tower.");
            }
        }

        private void PlaceMode(bool inPlaceMode)
        {
            if (inPlaceMode == true)
            {
                if (_canTakeTower == false)
                {
                    return;
                }
                _particleObj.SetActive(true);
                _inPlaceMode = true;
            }
            else
            {
                _particleObj.SetActive(false);
                _inPlaceMode = false;
            }
        }

        public void RemoveTower(GameObject tower)
        {
            if (_placedTower == tower)
            {
                _placedTower = null;
                _canTakeTower = true;

                if (_inPlaceMode == true)
                {
                    _particleObj.SetActive(true);
                }
            }
        }

        public bool CheckForTower()
        {
            return _canTakeTower;
        }
    }
}

