using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Tower.TowerPlacementNS;
using GameDevHQ.Enemy.EnemyClassNS;
using GameDevHQ.Interface.IHealth;

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
        private int _towerID = -1;
        [SerializeField]
        private int _damage = 0;
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

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && _launched == false) //check for space key and if we launched the rockets
            {
                _launched = true; //set the launch bool to true
                StartCoroutine(FireRocketsRoutine()); //start a coroutine that fires the rockets. 
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

                rocketLeft.GetComponent<GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle.Missile>().AssignMissleRules(_missileType, _target, _launchSpeed, _power, _fuseDelay, _destroyTime, DamageAmount);
                rocketRight.GetComponent<GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle.Missile>().AssignMissleRules(_missileType, _target, _launchSpeed, _power, _fuseDelay, _destroyTime, DamageAmount);

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
            _target = EnemyToTarget.GetComponent<EnemyClass>().GetHitTarget();
        }

        public void NoEnemiesInRange()
        {
            EnemyToTarget = null;
            IsEnemyInRange = false;
            _target = null;
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

