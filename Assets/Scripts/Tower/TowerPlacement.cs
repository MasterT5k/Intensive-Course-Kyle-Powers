using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _decoyObjects = null;

    private bool _inTowerPlaceMode = false;
    private int _selectedTower = -1;
    private GameObject _currentDecoy;
    private Camera _myCamera;
    private GameObject _rangeIndicator;

    public static event Action<bool> OnSelectTower;
    public static event Action<PlaceableArea> OnPlaceTower;
    public static event Func<int, GameObject> OnRequestTower;
    //public static event Func<bool> OnCheckForTower;

    private void OnEnable()
    {
        UIManager.OnTowerButtonClick += TowerButtonClick;
    }

    private void OnDisable()
    {
        UIManager.OnTowerButtonClick -= TowerButtonClick;
    }

    void Start()
    {
        _myCamera = Camera.main;

        for (int i = 0; i < _decoyObjects.Length; i++)
        {
            if (_decoyObjects[i].activeInHierarchy == true)
            {
                _decoyObjects[i].SetActive(false);
            }
        }
    }

    void Update()
    {
        if (_inTowerPlaceMode == true)
        {
            PlacingTower();
        }        
    }

    private void PlacingTower()
    {
        if (Input.GetMouseButtonDown(1) && _inTowerPlaceMode == true)
        {
            _currentDecoy.SetActive(false);
            _currentDecoy = null;
            _selectedTower = -1;
            _inTowerPlaceMode = false;
            OnSelectTower(_inTowerPlaceMode);
        }

        Ray rayOrigin = _myCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(rayOrigin, out hitInfo))
        {
            if (_currentDecoy != null)
            {
                _currentDecoy.transform.position = hitInfo.point;
            }

            bool canPlace;

            if (hitInfo.collider.tag == "Placement")
            {
                canPlace = hitInfo.collider.GetComponent<PlaceableArea>().CheckForTower();

                if (_rangeIndicator != null && canPlace == true)
                {
                    _rangeIndicator.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);
                }
                else
                {
                    _rangeIndicator.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
                }
            }
            else
            {
                canPlace = false;

                if (_rangeIndicator != null)
                {
                    _rangeIndicator.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (canPlace == true)
                {
                    if (_selectedTower < 0)
                    {
                        Debug.Log("Select a Tower to place.");
                        return;
                    }

                    GameObject towerToPlace;
                    towerToPlace = OnRequestTower?.Invoke(_selectedTower);
                    int towerCost = towerToPlace.GetComponent<ITower>().WarFundValue;
                    bool haveFunds = GameManager.Instance.CheckFunds(towerCost);

                    if (haveFunds)
                    {
                        PlaceableArea clickedArea = hitInfo.collider.GetComponent<PlaceableArea>();
                        OnPlaceTower?.Invoke(clickedArea);                        
                        towerToPlace.transform.position = hitInfo.point;
                        towerToPlace.SetActive(true);
                        GameManager.Instance.ChangeFunds(towerCost, false);
                    }
                    else
                    {
                        Debug.Log("Not enough in the War Fund to build Tower.");
                    }
                }
                else
                {
                    Debug.Log("You can't place that here!");
                }
            }
        }
    }

    public void TowerButtonClick(int selectedTower)
    {
        if (_currentDecoy != null)
        {
            _currentDecoy.SetActive(false);
        }

        _currentDecoy = _decoyObjects[selectedTower];
        _currentDecoy.SetActive(true);
        _rangeIndicator = _currentDecoy.transform.Find("Attack Range").gameObject;
        _selectedTower = selectedTower;
        _inTowerPlaceMode = true;
        OnSelectTower?.Invoke(_inTowerPlaceMode);
    }
}
