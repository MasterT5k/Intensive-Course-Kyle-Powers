using GameDevHQ.Enemy.EnemyClassNS;
using GameDevHQ.Interface.IHealthNS;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Tower.TowerPlacementNS;
using System;
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
        private int _startingHealth = 1;
        [SerializeField]
        private int _damage = 0;
        [SerializeField]
        private float _attackDelay = 1f;
        [SerializeField]
        private GameObject _healthBar = null;
        [SerializeField]
        private Transform _rotationPoint = null;

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
        public IHealth TargetHealth { get; set; }
        public MeshRenderer AttackRange { get; set; }
        public Transform RotationObj { get; set; }
        public List<GameObject> EnemiesInRange { get; set; }
        public bool Damaged { get; set; } = false;
        public GameObject HealthBar => _healthBar;
        public GameObject MainCamera => Camera.main.gameObject;
        public MeshRenderer HealthRender => _healthBar.GetComponent<MeshRenderer>();
        public MaterialPropertyBlock MatBlock { get; set; }

        public static event Action<GameObject> onDestroyed;

        private void OnEnable()
        {
            EnemyClass.onHealthGone += RemoveEnemy;
            TowerPlacement.onSelectTower += PlaceMode;
            Damaged = false;
            HealthRender.enabled = false;
            Health = StartingHealth;
            AttackRange.enabled = false;
            EnemiesInRange.Clear();
            NoEnemiesInRange();
        }

        private void OnDisable()
        {
            EnemyClass.onHealthGone -= RemoveEnemy;
            TowerPlacement.onSelectTower -= PlaceMode;
            onDestroyed?.Invoke(this.gameObject);
        }

        private void Awake()
        {
            Init();
        }

        void Start()
        {
            for (int i = 0; i < _muzzleFlash.Length; i++)
            {
                _muzzleFlash[i].SetActive(false);
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
                    TargetHealth.Damage(DamageAmount);
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

            if (Damaged == true)
            {
                AlignToCamera();
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
            StartingHealth = _startingHealth;
            HealthRender.enabled = false;
            MatBlock = new MaterialPropertyBlock();
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
                TargetHealth = EnemyToTarget.GetComponent<IHealth>();
            }
            IsEnemyInRange = true;
        }

        public void NoEnemiesInRange()
        {
            EnemyToTarget = null;
            TargetHealth = null;
            IsEnemyInRange = false;
        }

        public void Damage(int amount)
        {
            if (Damaged == false)
            {
                HealthRender.enabled = true;
                Damaged = true;
            }

            Health -= amount;
            float healthPrecent = Health / (float)StartingHealth;
            HealthRender.GetPropertyBlock(MatBlock);
            MatBlock.SetFloat("_amount", healthPrecent);
            HealthRender.SetPropertyBlock(MatBlock);

            if (Health < 1)
            {
                Destroyed();
            }
        }

        public void Destroyed()
        {
            //Debug.Log("Tower " + this.name + " destroyed.");
            gameObject.SetActive(false);
        }

        public void AlignToCamera()
        {
            if (MainCamera != null)
            {
                var camXform = MainCamera.transform;
                var forward = HealthBar.transform.position - camXform.position;
                forward.Normalize();
                var up = Vector3.Cross(forward, camXform.right);
                HealthBar.transform.rotation = Quaternion.LookRotation(forward, up);
            }
        }
    }

}
