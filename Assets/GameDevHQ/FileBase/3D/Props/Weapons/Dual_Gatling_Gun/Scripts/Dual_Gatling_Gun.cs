using GameDevHQ.Enemy.EnemyClassNS;
using GameDevHQ.Interface.IHealth;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Tower.TowerPlacementNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameDevHQ.FileBase.Dual_Gatling_Gun
{
    /// <summary>
    /// This script will allow you to view the presentation of the Turret and use it within your project.
    /// Please feel free to extend this script however you'd like. To access this script from another script
    /// (Script Communication using GetComponent) -- You must include the namespace (using statements) at the top. 
    /// "using GameDevHQ.FileBase.Dual_Gatling_Gun" without the quotes. 
    /// 
    /// For more, visit GameDevHQ.com
    /// 
    /// @authors
    /// Al Heck
    /// Jonathan Weinberger
    /// </summary>

    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))]
    public class Dual_Gatling_Gun : MonoBehaviour, ITower, IHealth
    {
        [SerializeField]
        private Transform[] _gunBarrel = null;
        [SerializeField]
        private GameObject[] _muzzleFlash = null;
        [SerializeField]
        private ParticleSystem[] _bulletCasings = null;
        [SerializeField]
        private AudioClip _fireSound = null;

        [SerializeField]
        private int _warFundValue = 0;
        [SerializeField]
        private int _towerID = -1;
        [SerializeField]
        private Transform _rotationPoint = null;
        [SerializeField]
        private int _damage = 0;
        [SerializeField]
        private float _attackDelay = 1f;

        private AudioSource _audioSource;
        private bool _startWeaponNoise = true;

        public bool IsEnemyInRange { get; set; }
        public int WarFundValue { get; set; }
        public int TowerID { get; set; }
        public int DamageAmount { get; set; }
        public int StartingHealth { get; set; }
        public int Health { get; set; }
        public float AttackDelay { get; set; }
        public GameObject EnemyToTarget { get; set; }
        public MeshRenderer AttackRange { get; set; }
        public Transform RotationObj { get; set; }
        public List<GameObject> EnemiesInRange { get; set; }

        private void OnEnable()
        {
            EnemyClass.onHealthGone += RemoveEnemy;
            TowerPlacement.onSelectTower += PlaceMode;
        }

        private void OnDisable()
        {
            EnemyClass.onHealthGone -= RemoveEnemy;
            TowerPlacement.onSelectTower -= PlaceMode;
        }

        private void Awake()
        {
            Init();
        }

        void Start()
        {
            for (int i = 0; i < _muzzleFlash.Length; i++)
            {
                _muzzleFlash[i].SetActive(true);
            }
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = true;
            _audioSource.clip = _fireSound;
        }

        void Update()
        {
            if (IsEnemyInRange == true)
            {
                RotationObj.LookAt(EnemyToTarget.transform, Vector3.up);
                RotateBarrel();
                for (int i = 0; i < _muzzleFlash.Length; i++)
                {
                    _muzzleFlash[i].SetActive(true);
                    _bulletCasings[i].Emit(1);  
                }

                if (_startWeaponNoise == true)
                {
                    _audioSource.Play();
                    _startWeaponNoise = false;
                }

                if (Time.time > AttackDelay)
                {
                    AttackDelay = Time.time + _attackDelay;
                    EnemyToTarget.GetComponent<EnemyClass>().Damage(DamageAmount);
                }
            }
            else if (IsEnemyInRange == false && _startWeaponNoise == false)
            {
                for (int i = 0; i < _muzzleFlash.Length; i++)
                {
                    _muzzleFlash[i].SetActive(false);
                }
                _audioSource.Stop();
                _startWeaponNoise = true;
            }
        }

        void RotateBarrel() 
        {
            _gunBarrel[0].transform.Rotate(Vector3.forward * Time.deltaTime * -500.0f);
            _gunBarrel[1].transform.Rotate(Vector3.forward * Time.deltaTime * -500.0f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                GameObject enemy = other.gameObject;
                EnemiesInRange.Add(enemy);
                AttackEnemy(EnemiesInRange[0]);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                GameObject enemy = other.gameObject;
                RemoveEnemy(enemy);
            }
        }

        public void Init()
        {
            WarFundValue = _warFundValue;
            TowerID = _towerID;
            AttackRange = transform.Find("Attack Range").GetComponent<MeshRenderer>();
            RotationObj = _rotationPoint;
            DamageAmount = _damage;
            EnemiesInRange = new List<GameObject>();
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

        public void RemoveEnemy(GameObject enemy)
        {
            EnemiesInRange.Remove(enemy);
            if (EnemiesInRange.Count > 0)
            {
                AttackEnemy(EnemiesInRange[0]);
            }
            else
            {
                NoEnemiesInRange();
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

        public void Damage(int amount)
        {
            Health -= amount;

            if (Health < 1)
            {
                Destroyed();
            }
        }

        public void Destroyed()
        {

        }
    }

}
