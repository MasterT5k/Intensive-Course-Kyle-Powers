using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableArea : MonoBehaviour
{
    private GameObject _particleObj;
    private bool _hasTower = false;

    private void OnEnable()
    {
        TowerPlacement.onTowerMode += PlaceMode;
    }

    private void OnDisable()
    {
        TowerPlacement.onTowerMode -= PlaceMode;
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
            if (_hasTower == true)
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
            _hasTower = true;
            _particleObj.SetActive(false);
        }
    }
}
