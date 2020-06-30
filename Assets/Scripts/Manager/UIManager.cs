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
        [Header("Stats Fields")]
        [SerializeField]
        private Text _fundsText = null;
        [SerializeField]
        private Text _livesCount = null;
        [SerializeField]
        private Text _statusText = null;
        [SerializeField]
        private Text _waveCount = null;
        [SerializeField]
        private Text _versionText = null;
        [SerializeField]
        private string _versionNumber = "v0.1";
        [SerializeField]
        private GameObject _levelStatus = null;
        [SerializeField]
        private Text _levelStatusText = null;

        [Header("Tower Fields")]
        [SerializeField]
        private GameObject _gunUpgrade = null;
        [SerializeField]
        private Text _gatlingFunds = null;
        [SerializeField]
        private GameObject _missileUpgrade = null;
        [SerializeField]
        private Text _missileFunds = null;
        [SerializeField]
        private GameObject _dismantleWeapon = null;
        [SerializeField]
        private Text _dismantleFunds = null;
        [SerializeField]
        private Button _galtingButton = null;
        private Text _gatlingCost;
        [SerializeField]
        private Button _missileButton = null;
        private Text _missileCost;

        [Header("Playback Fields")]
        [SerializeField]
        private Image _pauseButton = null;
        private Sprite _pauseNormalSprite = null;
        [SerializeField]
        private Sprite _pauseActiveSprite = null;
        [SerializeField]
        private Image _playButton = null;
        private Sprite _playNormalSprite;
        [SerializeField]
        private Sprite _playActiveSprite = null;
        [SerializeField]
        private Image _fastForwardButton = null;
        private Sprite _fastForwardNormalSprite;
        [SerializeField]
        private Sprite _fastForwardActiveSprite = null;
        [SerializeField]
        private Image _restartButton = null;
        [SerializeField]
        private Sprite _restartPressedSprite = null;

        [Header("UI Background")]
        [SerializeField]
        private Image _armoryImage = null;
        private Sprite _armoryNormalSprite;
        [SerializeField]
        private Sprite _armoryCautionSprite = null;
        [SerializeField]
        private Sprite _armoryWarningSprite = null;
        private Image _levelStatusImage;
        private Sprite _levelStatusNormalSprite;
        [SerializeField]
        private Sprite _levelStatusCautionSprite = null;
        [SerializeField]
        private Sprite _levelStatusWarningSprite = null;
        [SerializeField]
        private Image _livesWaveImage = null;
        private Sprite _livesWaveNormalSprite;
        [SerializeField]
        private Sprite _livesWaveCautionSprite = null;
        [SerializeField]
        private Sprite _livesWaveWarningSprite = null;
        [SerializeField]
        private Image _playbackImage = null;
        private Sprite _playbackNormalSprite;
        [SerializeField]
        private Sprite _playbackCautionSprite = null;
        [SerializeField]
        private Sprite _playbackWarningSprite = null;
        [SerializeField]
        private Image _restartImage = null;
        private Sprite _restartNormalSprite;
        [SerializeField]
        private Sprite _restartCautionSprite = null;
        [SerializeField]
        private Sprite _restartWarningSprite = null;
        [SerializeField]
        private Image _warFundsImage = null;
        private Sprite _warFundsNormalSprite;
        [SerializeField]
        private Sprite _warFundsCautionSprite = null;
        [SerializeField]
        private Sprite _warFundsWarningSprite = null;

        private bool _towerSelected;
        private GameObject _selectedTower;
        private int _upgradeCost;

        public static event Action<int> onTowerButtonClick;
        public static event Action<int, GameObject> onUpgradeButtonClick;

        private void Start()
        {
            _gunUpgrade.SetActive(false);
            _missileUpgrade.SetActive(false);
            _dismantleWeapon.SetActive(false);
            _levelStatus.SetActive(false);

            if (_galtingButton != null)
                _gatlingCost = _galtingButton.GetComponentInChildren<Text>();

            if (_missileButton != null)
                _missileCost = _missileButton.GetComponentInChildren<Text>();

            if (_pauseButton != null)
                _pauseNormalSprite = _pauseButton.sprite;

            if (_playButton != null)
                _playNormalSprite = _playButton.sprite;

            if (_fastForwardButton != null)
                _fastForwardNormalSprite = _fastForwardButton.sprite;

            if (_armoryImage != null)
                _armoryNormalSprite = _armoryImage.sprite;

            if (_livesWaveImage != null)
                _livesWaveNormalSprite = _livesWaveImage.sprite;

            if (_playbackImage != null)
                _playbackNormalSprite = _playbackImage.sprite;

            if (_restartImage != null)
                _restartNormalSprite = _restartImage.sprite;

            if (_warFundsImage != null)
                _warFundsNormalSprite = _warFundsImage.sprite;

            if (_levelStatus != null)
            {
                _levelStatusImage = _levelStatus.GetComponent<Image>();
                _levelStatusNormalSprite = _levelStatusImage.sprite;
            }

            _playButton.sprite = _playActiveSprite;
            _versionText.text = _versionNumber;
        }

        public void SetWarFundText(int warFunds)
        {
            _fundsText.text = warFunds.ToString();
        }

        public void SetLivesCount(int lives)
        {
            _livesCount.text = lives.ToString();
        }

        public void SetWaveCount(int currentWave, int totalWaves)
        {
            _waveCount.text = currentWave + "/" + totalWaves;
        }

        public void SetBaseTowerCosts(int towerID, int towerCost)
        {
            if (towerID == 0)
            {
                _gatlingCost.text = "$" + towerCost;
            }
            else if (towerID == 1)
            {
                _missileCost.text = "$" + towerCost;
            }
            else
            {
                Debug.Log("No Button Cost to Update.");
            }
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
            if (_selectedTower == null)
            {
                Debug.Log("Selected Tower is NULL");
            }
            ITower tower = _selectedTower.GetComponent<ITower>();
            int towerID = tower.TowerID;

            switch (towerID)
            {
                case 0:
                    UpdateFunds(_gatlingFunds, 2);
                    UpdateFunds(_dismantleFunds, towerID);
                    if (GameManager.Instance.CheckFunds(_upgradeCost) == true)
                    {
                        _towerSelected = true;
                        _gunUpgrade.SetActive(true);
                    }
                    _dismantleWeapon.SetActive(true);
                    break;
                case 1:                    
                    UpdateFunds(_missileFunds, 3);
                    UpdateFunds(_dismantleFunds, towerID); 
                    if (GameManager.Instance.CheckFunds(_upgradeCost) == true)
                    {
                        _towerSelected = true;
                        _missileUpgrade.SetActive(true);
                    }
                    _dismantleWeapon.SetActive(true);
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
            if (towerID > 1)
            {
                _upgradeCost = amount;
            }
        }

        public void ClearSelection()
        {
            _towerSelected = false;
            _selectedTower = null;
            _gunUpgrade.SetActive(false);
            _missileUpgrade.SetActive(false);
            _dismantleWeapon.SetActive(false);
        }

        public void PlayBackButtonClick(int speed)
        {
            switch (speed)
            {
                case 0:
                    if (Time.timeScale != speed)
                    {
                        _pauseButton.sprite = _pauseActiveSprite;
                        _playButton.sprite = _playNormalSprite;
                        _fastForwardButton.sprite = _fastForwardNormalSprite;
                        Time.timeScale = speed;
                    }
                    else
                    {
                        Debug.Log("Already in Pause Mode.");
                    }
                    break;
                case 1:
                    if (Time.timeScale != speed)
                    {
                        _pauseButton.sprite = _pauseNormalSprite;
                        _playButton.sprite = _playActiveSprite;
                        _fastForwardButton.sprite = _fastForwardNormalSprite;
                        Time.timeScale = speed;
                    }
                    else
                    {
                        Debug.Log("Already in Play Mode.");
                    }
                    break;
                case 2:
                    if (Time.timeScale != speed)
                    {
                        _pauseButton.sprite = _pauseNormalSprite;
                        _playButton.sprite = _playNormalSprite;
                        _fastForwardButton.sprite = _fastForwardActiveSprite;
                        Time.timeScale = speed;
                    }
                    else
                    {
                        Debug.Log("Already in FastForward Mode.");
                    }
                    break;
                default:
                    break;
            }
        }

        public void UpdateLivesUI(int lives)
        {
            _livesCount.text = lives.ToString();

            if (lives > 60)
            {
                _armoryImage.sprite = _armoryNormalSprite;
                _levelStatusImage.sprite = _levelStatusNormalSprite;
                _livesWaveImage.sprite = _livesWaveNormalSprite;
                _playbackImage.sprite = _playbackNormalSprite;
                _restartImage.sprite = _restartNormalSprite;
                _warFundsImage.sprite = _warFundsNormalSprite;
                _statusText.text = "Good";
            }
            else if (lives > 20)
            {
                _armoryImage.sprite = _armoryCautionSprite;
                _levelStatusImage.sprite = _levelStatusCautionSprite;
                _livesWaveImage.sprite = _livesWaveCautionSprite;
                _playbackImage.sprite = _playbackCautionSprite;
                _restartImage.sprite = _restartCautionSprite;
                _warFundsImage.sprite = _warFundsCautionSprite;
                _statusText.text = "Caution";
            }
            else
            {
                _armoryImage.sprite = _armoryWarningSprite;
                _levelStatusImage.sprite = _levelStatusWarningSprite;
                _livesWaveImage.sprite = _livesWaveWarningSprite;
                _playbackImage.sprite = _playbackWarningSprite;
                _restartImage.sprite = _restartWarningSprite;
                _warFundsImage.sprite = _warFundsWarningSprite;
                _statusText.text = "Warning!";
            }
        }

        public void UpdateWaveCount(int currentWave, int numberOfWaves)
        {
            _waveCount.text = currentWave + " / " + numberOfWaves;
        }

        public void LevelEnd(bool alive)
        {
            if (alive == true)
            {
                _levelStatus.SetActive(true);
                _levelStatusText.text = "LEVEL\nCOMPLETE";
            }
            else
            {
                _levelStatus.SetActive(true);
                _levelStatusText.text = "LEVEL\nFAILED";
            }
        }

        public void RestartButtonClick()
        {
            _restartButton.sprite = _restartPressedSprite;
            GameManager.Instance.RestartLevel();
        }
    }
}

