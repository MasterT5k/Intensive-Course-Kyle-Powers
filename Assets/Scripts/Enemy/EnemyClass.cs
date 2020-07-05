using GameDevHQ.FileBase.Gatling_Gun;
using GameDevHQ.Interface.IHealthNS;
using GameDevHQ.Interface.ITowerNS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevHQ.Enemy.EnemyClassNS
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Collider))]
    public abstract class EnemyClass : MonoBehaviour, IHealth
    {
        [SerializeField]
        protected float _speed = 1.5f;
        [SerializeField]
        protected int _startingHealth = 1;
        [SerializeField]
        protected int _livesCost = 1;
        [SerializeField]
        protected int _currencyValue = 0;
        [SerializeField]
        protected int _damage = 1;
        [SerializeField]
        private float _attackDelay = 1f;
        [SerializeField]
        private GameObject _healthBar = null;
        [SerializeField]
        protected GameObject _explosionPrefab = null;
        [SerializeField]
        protected float _deathInactiveDelay = 5f;
        [SerializeField]
        protected Transform _hitTarget = null;
        [SerializeField]
        protected Transform _rotationObj = null;

        public static event Action<int> onDestroyed;
        public static event Action<int> onReachedEnd;
        public static event Action<GameObject> onHealthGone;
        public static event Action onDisabled;
        public static event Func<Transform> onGetEndPoint;

        protected bool _towerInRange = false;
        protected bool _isAlive = true;
        protected float _canFire = -1f;
        protected NavMeshAgent _agent;
        protected Animator _anim;
        protected Transform _endPoint;
        protected Collider _collider;
        protected List<GameObject> _towersInRange = new List<GameObject>();
        protected GameObject _targetedTower;
        protected IHealth _towerHealth;
        protected Quaternion _startRotation;

        public int StartingHealth { get; set; }
        public int Health { get; set; }
        public bool Damaged { get; set; } = false;
        public GameObject HealthBar => _healthBar;
        public GameObject MainCamera => Camera.main.gameObject;
        public MeshRenderer HealthRender => _healthBar.GetComponent<MeshRenderer>();
        public MaterialPropertyBlock MatBlock { get; set; }

        public virtual void Init()
        {
            _agent = GetComponent<NavMeshAgent>();
            _anim = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
            StartingHealth = _startingHealth;
            MatBlock = new MaterialPropertyBlock();
            _startRotation = _rotationObj.localRotation;
            HealthRender.enabled = false;

            if (_agent == null)
            {
                Debug.LogError("Nav Mesh Agent is NULL");
            }

            if (_anim == null)
            {
                Debug.LogError("Animator is NULL");
            }

            if (_collider == null)
            {
                Debug.LogError("Collider is NULL");
            }
        }

        public virtual void Awake()
        {
            Init();
        }

        public virtual void OnEnable()
        {
            Gatling_Gun.onDestroyed += RemoveTower;
            Activate();
        }

        public virtual void OnDisable()
        {
            Gatling_Gun.onDestroyed -= RemoveTower;
            onDisabled?.Invoke();
        }

        public virtual void Update()
        {
            if (Damaged == true)
            {
                AlignToCamera();
            }

            if (_towerInRange == true)
            {
                _rotationObj.LookAt(_targetedTower.transform, Vector3.up);

                if (Time.time > _canFire)
                {
                    _anim.SetTrigger("Shoot");
                    _canFire = Time.time + _attackDelay;
                    _towerHealth.Damage(_damage);
                }
            }
            else if (_towerInRange == false && _isAlive == true && _rotationObj.localRotation != _startRotation)
            {
                _rotationObj.localRotation = Quaternion.Lerp(_rotationObj.localRotation, _startRotation, 5f * Time.deltaTime);
            }
        }

        public virtual void Activate()
        {
            if (_agent.enabled == false)
            {
                _agent.enabled = true;
            }

            if (_collider.enabled == false)
            {
                _collider.enabled = true;
            }

            if (_endPoint == null)
            {
                _endPoint = onGetEndPoint?.Invoke();
                Debug.Log("End Point was retrieved.");
                Health = StartingHealth;
                _agent.speed = _speed;
                _agent.SetDestination(_endPoint.position);
            }
            else
            {
                Health = StartingHealth;
                _agent.speed = _speed;
                _agent.SetDestination(_endPoint.position);
            }
            _isAlive = true;
            Damaged = false;
            _rotationObj.localRotation = _startRotation;
            _towersInRange.Clear();
            NoTowersInRange();
        }

        public virtual void Destroyed()
        {
            onDestroyed?.Invoke(_currencyValue);
            onHealthGone?.Invoke(this.gameObject);
            _agent.enabled = false;
            _explosionPrefab.SetActive(true);
            HealthRender.enabled = false;
            _anim.SetTrigger("Destroyed");
            _anim.SetBool("Target", false);
            _isAlive = false;
            NoTowersInRange();
            _collider.enabled = false;
            StartCoroutine(InactiveCoroutine(_deathInactiveDelay));
        }

        public virtual void ReachedPathEnd()
        {
            onReachedEnd?.Invoke(_livesCost);
            gameObject.SetActive(false);
        }

        public virtual void Damage(int amount)
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

        private void AttackTower(GameObject tower)
        {
            if (_targetedTower != tower || _targetedTower == null)
            {
                _targetedTower = tower;
                _towerHealth = _targetedTower.GetComponent<IHealth>();
            }
            _towerInRange = true;
            _anim.SetBool("Target", true);
        }

        public Transform GetHitTarget()
        {
            return _hitTarget;
        }

        private void RemoveTower(GameObject tower)
        {
            _towersInRange.Remove(tower);
            if (_towersInRange.Count > 0)
            {
                AttackTower(_towersInRange[0]);
            }
            else
            {
                NoTowersInRange();
            }
        }

        private void NoTowersInRange()
        {
            _targetedTower = null;
            _towerHealth = null;
            _towerInRange = false;
            _anim.ResetTrigger("Shoot");
            _anim.SetBool("Target", false);
            
        }

        protected IEnumerator InactiveCoroutine(float inactiveDelay)
        {
            yield return new WaitForSeconds(inactiveDelay);
            _anim.SetTrigger("Fade");
            _explosionPrefab.SetActive(false);
            yield return new WaitForSeconds(2f);
            _anim.Rebind();
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Attack Range"))
            {
                GameObject tower = other.transform.parent.gameObject;
                _towersInRange.Add(tower);
                AttackTower(_towersInRange[0]);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Attack Range"))
            {
                GameObject tower = other.transform.parent.gameObject;
                RemoveTower(tower);
            }
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

