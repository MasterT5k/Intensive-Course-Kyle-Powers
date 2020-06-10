using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _decoyObjects = null;
    [SerializeField]
    private GameObject[] _towerPrefabs = null;

    private bool _inTowerPlaceMode = false;
    private bool _canPlaceHere = false;
    private int _towerToSpawn = -1;
    private GameObject _currentDecoy;
    private Camera _myCamera;
    private GameObject _rangeIndicator;

    public static event Action<bool> onTowerMode;

    // Start is called before the first frame update
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

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _inTowerPlaceMode = true;
            onTowerMode(_inTowerPlaceMode);
        }

        if (Input.GetMouseButtonDown(1))
        {
            _inTowerPlaceMode = false;
            onTowerMode(_inTowerPlaceMode);
        }

        if (_inTowerPlaceMode == true)
        {
            PlacingTower();
        }
        else if (_inTowerPlaceMode == false && _currentDecoy != null)
        {
            _currentDecoy.SetActive(false);
            _currentDecoy = null;
            _towerToSpawn = -1;
        }
    }

    private void PlacingTower()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (_currentDecoy != null)
            {
                _currentDecoy.SetActive(false);
            }
            _towerToSpawn = 0;
            _currentDecoy = _decoyObjects[_towerToSpawn];
            _currentDecoy.SetActive(true);
            _rangeIndicator = _currentDecoy.transform.Find("Attack Range").gameObject;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (_currentDecoy != null)
            {
                _currentDecoy.SetActive(false);
            }
            _towerToSpawn = 1;
            _currentDecoy = _decoyObjects[_towerToSpawn];
            _currentDecoy.SetActive(true);
            _rangeIndicator = _currentDecoy.transform.Find("Attack Range").gameObject;
        }

        Ray rayOrigin = _myCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(rayOrigin, out hitInfo))
        {
            if (_currentDecoy != null)
            {
                _currentDecoy.transform.position = hitInfo.point;
            }

            if (hitInfo.collider.tag == "Placement")
            {
                _canPlaceHere = true;

                if (_rangeIndicator != null)
                {
                    _rangeIndicator.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);
                }
                else
                {
                    Debug.LogError("Renderer not found.");
                }
            }
            else
            {
                _canPlaceHere = false;

                if (_rangeIndicator != null)
                {
                    _rangeIndicator.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (_canPlaceHere == true)
                {
                    switch (_towerToSpawn)
                    {
                        case 0:
                            Instantiate(_towerPrefabs[_towerToSpawn], hitInfo.point, Quaternion.identity);
                            break;
                        case 1:
                            Instantiate(_towerPrefabs[_towerToSpawn], hitInfo.point, Quaternion.identity);
                            break;
                        default:
                            Debug.Log("Select a Tower to place.");
                            break;
                    }
                }
                else
                {
                    Debug.Log("You can't place that here!");
                }
            }
        }
    }
}
