using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Manager.GameManagerNS;
using GameDevHQ.Manager.PoolManagerNS;
using GameDevHQ.Manager.SpawnManagerNS;
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
        private Text _levelStatusText = null;
        [SerializeField]
        private Text _countdownText = null;
        [SerializeField]
        private Text _timerText = null;

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
        [SerializeField]
        private Image _playButton = null;
        [SerializeField]
        private Image _fastForwardButton = null;
        [SerializeField]
        private Image _restartButton = null;
        [SerializeField]
        private UIButtons _uIButtons = null;

        [Header("UI Background")]
        [SerializeField]
        private Image _armoryImage = null;
        [SerializeField]
        private Image _countdownImage = null;
        [SerializeField]
        private Image _levelStatusImage = null;
        [SerializeField]
        private Image _livesWaveImage = null;
        [SerializeField]
        private Image _playbackImage = null;
        [SerializeField]
        private Image _restartImage = null;
        [SerializeField]
        private Image _warFundsImage = null;
        [SerializeField]
        private UIBackground _uIBackground = null;

        private bool _towerSelected;
        private GameObject _selectedTower;
        private int _upgradeCost;

        public static event Action<int> onTowerButtonClick;
        public static event Action<int, GameObject> onUpgradeButtonClick;

        private void Start()
        {
            _versionText.text = _versionNumber;
            _gunUpgrade.SetActive(false);
            _missileUpgrade.SetActive(false);
            _dismantleWeapon.SetActive(false);
            _levelStatusImage.gameObject.SetActive(false);
            _countdownImage.gameObject.SetActive(false);            

            if (_galtingButton != null)
                _gatlingCost = _galtingButton.GetComponentInChildren<Text>();

            if (_missileButton != null)
                _missileCost = _missileButton.GetComponentInChildren<Text>();

            if (Time.timeScale == 1)
                _playButton.sprite = _uIButtons.play.pressed;
            else
                PlayBackButtonClick(1);            
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
                    UpdateFunds(_dismantleFunds, towerID, _selectedTower);
                    if (GameManager.Instance.CheckFunds(_upgradeCost) == true)
                    {
                        _towerSelected = true;
                        _gunUpgrade.SetActive(true);
                    }
                    _dismantleWeapon.SetActive(true);
                    break;
                case 1:                    
                    UpdateFunds(_missileFunds, 3);
                    UpdateFunds(_dismantleFunds, towerID, _selectedTower); 
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
                    UpdateFunds(_dismantleFunds, towerID, _selectedTower);
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

        public void UpdateFunds(Text funds, int towerID, GameObject tower = null)
        {
            int amount;
            if (funds == _dismantleFunds && tower != null)
            {
                amount = GameManager.Instance.TowerRefund(tower);
                funds.text = amount.ToString();
            }
            else
            {
                amount = PoolManager.Instance.GetTowerCost(towerID);
                funds.text = amount.ToString();
            }
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
                        _pauseButton.sprite = _uIButtons.pause.pressed;
                        _playButton.sprite = _uIButtons.play.normal;
                        _fastForwardButton.sprite = _uIButtons.fastForward.normal;
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
                        _pauseButton.sprite = _uIButtons.pause.normal;
                        _playButton.sprite = _uIButtons.play.pressed;
                        _fastForwardButton.sprite = _uIButtons.fastForward.normal;
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
                        _pauseButton.sprite = _uIButtons.pause.normal;
                        _playButton.sprite = _uIButtons.play.normal;
                        _fastForwardButton.sprite = _uIButtons.fastForward.pressed;
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
                _armoryImage.sprite = _uIBackground.armory.normal;
                _countdownImage.sprite = _uIBackground.countdown.normal;
                _levelStatusImage.sprite = _uIBackground.levelStatus.normal;
                _livesWaveImage.sprite = _uIBackground.livesWave.normal;
                _playbackImage.sprite = _uIBackground.playback.normal;
                _restartImage.sprite = _uIBackground.restart.normal;
                _warFundsImage.sprite = _uIBackground.warFunds.normal;
                _statusText.text = "Good";
            }
            else if (lives > 20)
            {
                _armoryImage.sprite = _uIBackground.armory.caution;
                _countdownImage.sprite = _uIBackground.countdown.caution;
                _levelStatusImage.sprite = _uIBackground.levelStatus.caution;
                _livesWaveImage.sprite = _uIBackground.livesWave.caution;
                _playbackImage.sprite = _uIBackground.playback.caution;
                _restartImage.sprite = _uIBackground.restart.caution;
                _warFundsImage.sprite = _uIBackground.warFunds.caution;
                _statusText.text = "Caution";
            }
            else
            {
                _armoryImage.sprite = _uIBackground.armory.warning;
                _countdownImage.sprite = _uIBackground.countdown.warning;
                _levelStatusImage.sprite = _uIBackground.levelStatus.warning;
                _livesWaveImage.sprite = _uIBackground.livesWave.warning;
                _playbackImage.sprite = _uIBackground.playback.warning;
                _restartImage.sprite = _uIBackground.restart.warning;
                _warFundsImage.sprite = _uIBackground.warFunds.warning;
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
                _levelStatusImage.gameObject.SetActive(true);
                _levelStatusText.text = "LEVEL\nCOMPLETE";
            }
            else
            {
                _levelStatusImage.gameObject.SetActive(true);
                _levelStatusText.text = "LEVEL\nFAILED";
            }
        }

        public void RestartButtonClick()
        {
            _restartButton.sprite = _uIButtons.restart.pressed;
            GameManager.Instance.RestartLevel();
        }

        public void StartCountdown(float time, bool firstWave = false)
        {
            if (firstWave == true)
            {
                _countdownText.text = "STARTING IN";
            }
            else
            {
                _countdownText.text = "NEXT WAVE";
            }

            StartCoroutine(CountdownCoroutine(time));
        }

        private IEnumerator CountdownCoroutine(float time)
        {
            _countdownImage.gameObject.SetActive(true);

            while(time > 0)
            {
                time -= 1 * Time.deltaTime;
                _timerText.text = time.ToString("0.0") + "s      ";
                yield return new WaitForEndOfFrame();
            }

            SpawnManager.Instance.StartSpawn();
            yield return new WaitForSeconds(0.25f);
            _timerText.text = "STARTED";
            yield return new WaitForSeconds(1.25f);
            _countdownImage.gameObject.SetActive(false);
        }
    }
}

