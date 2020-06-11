using GameDevHQ.Tower.TowerPlacementNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Tower.PlaceableAreaNS
{
    public class PlaceableArea : MonoBehaviour
    {
        private GameObject _particleObj;
        private bool _canTakeTower = true;

        private void OnEnable()
        {
            TowerPlacement.OnSelectTower += PlaceMode;
            TowerPlacement.OnPlaceTower += ReceiveTower;
            //TowerPlacement.OnCheckForTower += CheckForTower;
        }

        private void OnDisable()
        {
            TowerPlacement.OnSelectTower -= PlaceMode;
            TowerPlacement.OnPlaceTower -= ReceiveTower;
            //TowerPlacement.OnCheckForTower -= CheckForTower;
        }

        // Start is called before the first frame update
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

        private void PlaceMode(bool inPlaceMode)
        {
            if (inPlaceMode == true)
            {
                if (_canTakeTower == false)
                {
                    return;
                }
                _particleObj.SetActive(true);

            }
            else
            {
                _particleObj.SetActive(false);
            }
        }

        private void ReceiveTower(PlaceableArea placeable)
        {
            if (placeable == this)
            {
                _canTakeTower = false;
                _particleObj.SetActive(false);
            }
        }

        public bool CheckForTower()
        {
            return _canTakeTower;
        }
    }
}

