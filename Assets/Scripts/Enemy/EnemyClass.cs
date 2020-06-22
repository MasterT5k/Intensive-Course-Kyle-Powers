using GameDevHQ.FileBase.Gatling_Gun;
using GameDevHQ.Interface.IHealth;
using GameDevHQ.Interface.ITowerNS;
using System;
using System.Collections;
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
        protected int _currencyValue = 0;
        [SerializeField]
        protected int _damage = 1;
        [SerializeField]
        private float _attackDelay = 1f;
        [SerializeField]
        protected GameObject _explosionPrefab = null;
        [SerializeField]
        protected float _deathInactiveDelay = 5f;
        [SerializeField]
        protected Transform _hitTarget = null;

        public static event Action<int> onDestroyed;
        public static event Action<GameObject> onHealthGone;
        public static event Action onDisabled;
        public static event Func<Transform> onGetEndPoint;

        protected bool _towerInRange = false;
        protected float _canFire = -1f;
        protected NavMeshAgent _agent;
        protected Animator _anim;
        protected Transform _endPoint;
        protected Collider _collider;
        protected GameObject _targetedTower;

        public int StartingHealth { get; set; }
        public int Health { get; set; }

        public virtual void Init()
        {
            _agent = GetComponent<NavMeshAgent>();
            _anim = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
            StartingHealth = _startingHealth;

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
            Gatling_Gun.onDestroyed += TowerDestroyed;
            Activate();
        }

        public virtual void OnDisable()
        {
            Gatling_Gun.onDestroyed -= TowerDestroyed;
            onDisabled?.Invoke();
        }

        public virtual void Update()
        {
            if (_towerInRange == true)
            {
                if (Time.time > _canFire)
                {
                    _canFire = Time.time + _attackDelay;
                    _targetedTower.GetComponent<IHealth>().Damage(_damage);
                }
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
        }

        public virtual void Destroyed()
        {
            onDestroyed?.Invoke(_currencyValue);
            onHealthGone?.Invoke(this.gameObject);
            _agent.enabled = false;
            _explosionPrefab.SetActive(true);
            _anim.SetTrigger("Destroyed");
            _collider.enabled = false;
            StartCoroutine(InactiveCoroutine(_deathInactiveDelay));
        }

        public virtual void ReachedPathEnd()
        {
            gameObject.SetActive(false);
        }

        public virtual void Damage(int amount)
        {
            Health -= amount;

            if (Health < 1)
            {
                Destroyed();
            }
        }

        public virtual void AttackTower()
        {

        }

        public Transform GetHitTarget()
        {
            return _hitTarget;
        }

        public void TowerDestroyed(GameObject tower)
        {
            if (_targetedTower == tower)
            {
                _targetedTower = null;
                _towerInRange = false;
                Debug.Log("Tower Destroyed.");
            }
        }

        protected IEnumerator InactiveCoroutine(float inactiveDelay)
        {
            yield return new WaitForSeconds(inactiveDelay);
            _anim.Rebind();
            _explosionPrefab.SetActive(false);
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Attack Range"))
            {
                IHealth tower = other.GetComponentInParent<IHealth>();

                if (tower != null)
                {
                    _targetedTower = other.transform.parent.gameObject;
                    _towerInRange = true;
                }
            }
        }
    }
}

