using GameDevHQ.Manager.GameManager;
using GameDevHQ.Tower.ITowerNS;
using GameDevHQ.Tower.TowerPlacementNS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Tower.PlaceableAreaNS
{
    public class PlaceableArea : MonoBehaviour
    {
        private GameObject _particleObj;
        private GameObject _placedTower;
        private bool _canTakeTower = true;
        private bool _inPlaceMode = false;

        public static event Action<bool> OnCanPlaceHere;
        public static event Func<int> OnGetSelectedTowerID;
        public static event Func<int, GameObject> OnRequestTower;

        private void OnEnable()
        {
            TowerPlacement.OnSelectTower += PlaceMode;
        }

        private void OnDisable()
        {
            TowerPlacement.OnSelectTower -= PlaceMode;
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
                OnCanPlaceHere?.Invoke(_canTakeTower);
            }
        }

        private void OnMouseExit()
        {
            if (_inPlaceMode == true)
            {
                OnCanPlaceHere?.Invoke(false);
            }
        }

        private void OnMouseDown()
        {
            int selectedTowerID = -1;

            if (_inPlaceMode == true)
            {
                selectedTowerID = (int)(OnGetSelectedTowerID?.Invoke());

                if (selectedTowerID < 0)
                {
                    Debug.Log("Select a Tower to place.");
                    return;
                }

                GameObject towerToPlace;
                towerToPlace = OnRequestTower?.Invoke(selectedTowerID);
                int towerCost = towerToPlace.GetComponent<ITower>().WarFundValue;
                bool haveFunds = GameManager.Instance.CheckFunds(towerCost);

                if (haveFunds && _canTakeTower == true)
                {                    
                    towerToPlace.transform.position = _particleObj.transform.position;
                    towerToPlace.SetActive(true);
                    _placedTower = towerToPlace;
                    _canTakeTower = false;
                    _particleObj.SetActive(false);
                    OnCanPlaceHere?.Invoke(_canTakeTower);
                    GameManager.Instance.ChangeFunds(towerCost, false);
                }
                else
                {
                    Debug.Log("Not enough in the War Fund to build Tower.");
                }
            }
            else if(_placedTower != null)
            {
                //Reserved for selecting placed tower and upgrading it.
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

        public bool CheckForTower()
        {
            return _canTakeTower;
        }
    }
}

