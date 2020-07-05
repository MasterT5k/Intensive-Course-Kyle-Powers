using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Tower.TowerPlacementNS;
using GameDevHQ.Enemy.EnemyClassNS;
using GameDevHQ.Interface.IHealthNS;
using System;

namespace GameDevHQ.FileBase.Missle_Launcher_Dual_Turret
{
    [RequireComponent(typeof(Rigidbody))]
    public class Missle_Launcher : MonoBehaviour, ITower, IHealth
    {
        public enum MissileType
        {
            Normal,
            Homing
        }

        [SerializeField]
        private GameObject _missilePrefab = null; //holds the missle gameobject to clone
        [SerializeField]
        private MissileType _missileType = MissileType.Homing;
        [SerializeField]
        private GameObject[] _misslePositionsLeft = null; //array to hold the rocket positions on the turret
        [SerializeField]
        private GameObject[] _misslePositionsRight = null; //array to hold the rocket positions on the turret
        [SerializeField]
        private float _fireDelay = 0f; //fire delay between rockets
        [SerializeField]
        private float _launchSpeed = 0f; //initial launch speed of the rocket
        [SerializeField]
        private float _power = 0f; //power to apply to the force of the rocket
        [SerializeField]
        private float _fuseDelay = 0f; //fuse delay before the rocket launches
        [SerializeField]
        private float _reloadTime = 0f; //time in between reloading the rockets
        [SerializeField]
        private float _destroyTime = 10.0f; //how long till the rockets get cleaned up
        [SerializeField]
        private Transform _target = null;

        [SerializeField]
        private int _warFundValue = 0;
        [SerializeField]
        private int _towerID = 3;
        [SerializeField]
        private int _startingHealth = 1;
        [SerializeField]
        private int _damage = 0;
        [SerializeField]
        private GameObject _healthBar = null;
        [SerializeField]
        private Transform _rotationPoint = null;

        private bool _launched; //bool to check if we launched the rockets

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

        private void Update()
        {
            if (IsEnemyInRange == true)
            {
                RotationObj.LookAt(EnemyToTarget.transform, Vector3.up);

                if (_launched == false)
                {
                    _launched = true;
                    StartCoroutine(FireRocketsRoutine());
                }
            }

            if (Damaged == true)
            {
                AlignToCamera();
            }
        }

        IEnumerator FireRocketsRoutine()
        {
            for (int i = 0; i < _misslePositionsLeft.Length; i++) //for loop to iterate through each missle position
            {
                if (_target == null)
                {
                    break;
                }

                GameObject rocketLeft = Instantiate(_missilePrefab); //instantiate a rocket
                GameObject rocketRight = Instantiate(_missilePrefab); //instantiate a rocket

                rocketLeft.transform.parent = _misslePositionsLeft[i].transform; //set the rockets parent to the missle launch position 
                rocketRight.transform.parent = _misslePositionsRight[i].transform; //set the rockets parent to the missle launch position 

                rocketLeft.transform.localPosition = Vector3.zero; //set the rocket position values to zero
                rocketRight.transform.localPosition = Vector3.zero; //set the rocket position values to zero

                rocketLeft.transform.localEulerAngles = new Vector3(0, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
                rocketRight.transform.localEulerAngles = new Vector3(0, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction

                rocketLeft.transform.parent = null; //set the rocket parent to null
                rocketRight.transform.parent = null; //set the rocket parent to null

                rocketLeft.GetComponent<GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle.Missle>().AssignMissleRules(_missileType, _target, _launchSpeed, _power, _fuseDelay, _destroyTime, DamageAmount);
                rocketRight.GetComponent<GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle.Missle>().AssignMissleRules(_missileType, _target, _launchSpeed, _power, _fuseDelay, _destroyTime, DamageAmount);

                _misslePositionsLeft[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired
                _misslePositionsRight[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired

                yield return new WaitForSeconds(AttackDelay); //wait for the firedelay
            }

            for (int i = 0; i < _misslePositionsLeft.Length; i++) //itterate through missle positions
            {
                yield return new WaitForSeconds(_reloadTime); //wait for reload time
                _misslePositionsLeft[i].SetActive(true); //enable fake rocket to show ready to fire
                _misslePositionsRight[i].SetActive(true); //enable fake rocket to show ready to fire
            }

            _launched = false; //set launch bool to false
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
            AttackDelay = _fireDelay;
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
                _target = EnemyToTarget.GetComponent<EnemyClass>().GetHitTarget();
                TargetHealth = EnemyToTarget.GetComponent<IHealth>();
            }
            IsEnemyInRange = true;
        }

        public void NoEnemiesInRange()
        {
            EnemyToTarget = null;
            _target = null;
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
            Debug.Log("Tower " + this.name + " destroyed.");
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

