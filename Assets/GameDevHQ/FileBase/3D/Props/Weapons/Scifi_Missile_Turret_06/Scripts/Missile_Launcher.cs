using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevHQ.FileBase.Missile_Launcher.Missile;
using GameDevHQ.Interface.ITowerNS;
using GameDevHQ.Tower.TowerPlacementNS;
using GameDevHQ.Enemy.EnemyClassNS;

/*
 *@author GameDevHQ 
 * For support, visit gamedevhq.com
 */

namespace GameDevHQ.FileBase.Missile_Launcher
{
    [RequireComponent(typeof(Rigidbody))]
    public class Missile_Launcher : MonoBehaviour, ITower
    {
        public enum MissileType
        {
            Normal,
            Homing
        }

        [SerializeField]
        private GameObject _missilePrefab = null;
        [SerializeField]
        private MissileType _missileType = MissileType.Homing;
        [SerializeField]
        private GameObject[] _misslePositions = null;
        [SerializeField]
        private float _fireDelay = 0f;
        [SerializeField]
        private float _launchSpeed = 0f;
        [SerializeField]
        private float _power = 0f;
        [SerializeField]
        private float _fuseDelay = 0f;
        [SerializeField]
        private float _reloadTime = 0f;
        [SerializeField]
        private float _destroyTime = 10.0f;
        private bool _launched;
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

        public bool IsEnemyInRange { get; set; }
        public int WarFundValue { get; set; }
        public int TowerID { get; set; }
        public int Damage { get; set; }
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
        }

        IEnumerator FireRocketsRoutine()
        {
            for (int i = 0; i < _misslePositions.Length; i++)
            {
                if (_target == null)
                {
                    break;
                }

                GameObject rocket = Instantiate(_missilePrefab);

                rocket.transform.parent = _misslePositions[i].transform;
                rocket.transform.localPosition = Vector3.zero;
                rocket.transform.localEulerAngles = new Vector3(-90, 0, 0);
                rocket.transform.parent = null;

                rocket.GetComponent<Missile.Missile>().AssignMissleRules(_missileType, _target, _launchSpeed, _power, _fuseDelay, _destroyTime, Damage);

                _misslePositions[i].SetActive(false);

                yield return new WaitForSeconds(AttackDelay);
            }

            for (int i = 0; i < _misslePositions.Length; i++)
            {
                yield return new WaitForSeconds(_reloadTime);
                _misslePositions[i].SetActive(true);
            }

            _launched = false;
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
            Damage = _damage;
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
            _target = EnemyToTarget.GetComponent<EnemyClass>().GetTarget();
        }

        public void NoEnemiesInRange()
        {
            EnemyToTarget = null;
            IsEnemyInRange = false;
            _target = null;
        }
    }
}

