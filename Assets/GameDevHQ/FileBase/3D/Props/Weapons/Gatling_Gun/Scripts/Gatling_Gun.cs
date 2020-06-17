using GameDevHQ.Enemy.EnemyClassNS;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Tower.TowerPlacementNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameDevHQ.FileBase.Gatling_Gun
{
    /// <summary>
    /// This script will allow you to view the presentation of the Turret and use it within your project.
    /// Please feel free to extend this script however you'd like. To access this script from another script
    /// (Script Communication using GetComponent) -- You must include the namespace (using statements) at the top. 
    /// "using GameDevHQ.FileBase.Gatling_Gun" without the quotes. 
    /// 
    /// For more, visit GameDevHQ.com
    /// 
    /// @authors
    /// Al Heck
    /// Jonathan Weinberger
    /// </summary>

    [RequireComponent(typeof(AudioSource))] //Require Audio Source component
    public class Gatling_Gun : MonoBehaviour, ITower
    {
        [SerializeField]
        private Transform _gunBarrel = null; //Reference to hold the gun barrel
        [SerializeField]
        private GameObject _muzzleFlash = null; //reference to the muzzle flash effect to play when firing
        [SerializeField]
        private ParticleSystem _bulletCasings = null; //reference to the bullet casing effect to play when firing
        [SerializeField]
        private AudioClip _fireSound = null; //Reference to the audio clip

        [SerializeField]
        private int _warFundValue = 0;
        [SerializeField]
        private int _towerID = -1;
        [SerializeField]
        private Transform _rotationPoint = null;
        [SerializeField]
        private int _damagePerSecond = 1;
        private float _attackDelay = -1f;

        private AudioSource _audioSource; //reference to the audio source component
        private bool _startWeaponNoise = true;

        public int WarFundValue { get; set; }
        public int TowerID { get; set; } = 0;
        public MeshRenderer AttackRange { get; set; }
        public GameObject EnemyToTarget { get; set; }
        public bool IsEnemyInRange { get; set; }
        public Transform RotationObj { get; set; }

        private void OnEnable()
        {
            TowerPlacement.onSelectTower += PlaceMode;
        }

        private void OnDisable()
        {
            TowerPlacement.onSelectTower -= PlaceMode;
        }

        void Awake()
        {
            Init();
        }

        // Use this for initialization
        void Start()
        {
            _muzzleFlash.SetActive(false); //setting the initial state of the muzzle flash effect to off
            _audioSource = GetComponent<AudioSource>(); //ssign the Audio Source to the reference variable
            _audioSource.playOnAwake = false; //disabling play on awake
            _audioSource.loop = true; //making sure our sound effect loops
            _audioSource.clip = _fireSound; //assign the clip to play
            
        }

        // Update is called once per frame
        void Update()
        {
            if (IsEnemyInRange == true)
            {
                RotationObj.LookAt(EnemyToTarget.transform, Vector3.up);
                RotateBarrel(); //Call the rotation function responsible for rotating our gun barrel
                _muzzleFlash.SetActive(true); //enable muzzle effect particle effect
                _bulletCasings.Emit(1); //Emit the bullet casing particle effect  

                if (_startWeaponNoise == true) //checking if we need to start the gun sound
                {
                    _audioSource.Play(); //play audio clip attached to audio source
                    _startWeaponNoise = false; //set the start weapon noise value to false to prevent calling it again
                }

                if (Time.time > _attackDelay)
                {
                    _attackDelay = Time.time + 1f;
                    EnemyToTarget.GetComponent<EnemyClass>().Damage(_damagePerSecond);
                }
            }
            else if (IsEnemyInRange == false && _startWeaponNoise == false) 
            {
                _muzzleFlash.SetActive(false); //turn off muzzle flash particle effect
                _audioSource.Stop(); //stop the sound effect from playing
                _startWeaponNoise = true; //set the start weapon noise value to true
            }
        }

        // Method to rotate gun barrel 
        void RotateBarrel() 
        {
            _gunBarrel.transform.Rotate(Vector3.forward * Time.deltaTime * -500.0f); //rotate the gun barrel along the "forward" (z) axis at 500 meters per second
        }

        public void Init()
        {
            WarFundValue = _warFundValue;
            TowerID = _towerID;
            AttackRange = transform.Find("Attack Range").GetComponent<MeshRenderer>();
            RotationObj = _rotationPoint;
        }

        public void PlaceMode(bool inPlaceMode)
        {
            if (inPlaceMode == true)
            {
                AttackRange.enabled = true;
            }
            else
            {
                AttackRange.enabled = false;
            }
        }

        public void AttackEnemy(GameObject enemy)
        {
            if (EnemyToTarget != enemy || EnemyToTarget == null)
            {
                EnemyToTarget = enemy;
            }
            IsEnemyInRange = true;
        }

        public void NoEnemiesInRange()
        {
            EnemyToTarget = null;
            IsEnemyInRange = false;
        }

        IEnumerator DamageEnemy(EnemyClass enemy = null)
        {
            while (true)
            {
                Debug.Log("FIRE!");
                enemy.Damage(1);
                yield return new WaitForSeconds(2f);
            }
        }
    }

}
