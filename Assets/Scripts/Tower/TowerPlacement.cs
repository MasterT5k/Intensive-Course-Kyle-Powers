using GameDevHQ.Manager.UIManagerNS;
using GameDevHQ.Tower.PlaceableAreaNS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevHQ.Tower.TowerPlacementNS
{
    public class TowerPlacement : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _decoyObjects = null;

        private bool _inTowerPlaceMode = false;
        private int _selectedTowerID = -1;
        private GameObject _currentDecoy;
        private Camera _myCamera;
        private GameObject _rangeIndicator;

        public static event Action<bool> OnSelectTower;

        private void OnEnable()
        {
            PlaceableArea.OnCanPlaceHere += CanPlaceHere;
            PlaceableArea.OnGetSelectedTowerID += GetSelectedTowerID;
            UIManager.OnTowerButtonClick += TowerButtonClick;
        }

        private void OnDisable()
        {
            PlaceableArea.OnCanPlaceHere -= CanPlaceHere;
            PlaceableArea.OnGetSelectedTowerID -= GetSelectedTowerID;
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
            if (Input.GetMouseButtonDown(1))
            {
                _currentDecoy.SetActive(false);
                _currentDecoy = null;
                _rangeIndicator = null;
                _selectedTowerID = -1;
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
            }
        }

        public int GetSelectedTowerID()
        {
            return _selectedTowerID;
        }

        public void CanPlaceHere(bool canPlace)
        {
            if (canPlace == true)
            {
                if (_rangeIndicator != null)
                {
                    _rangeIndicator.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);
                }
            }
            else
            {
                if (_rangeIndicator != null)
                {
                    _rangeIndicator.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
                }
            }
        }

        public void TowerButtonClick(int selectedTowerID)
        {
            if (_currentDecoy != null)
            {
                _currentDecoy.SetActive(false);
            }

            _currentDecoy = _decoyObjects[selectedTowerID];
            _currentDecoy.SetActive(true);
            _rangeIndicator = _currentDecoy.transform.Find("Attack Range").gameObject;
            _selectedTowerID = selectedTowerID;
            _inTowerPlaceMode = true;
            OnSelectTower?.Invoke(_inTowerPlaceMode);
        }
    }
}

