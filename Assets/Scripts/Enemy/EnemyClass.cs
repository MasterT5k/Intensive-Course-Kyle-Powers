using GameDevHQ.Interface.IHealth;
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
        protected GameObject _explosionPrefab = null;
        [SerializeField]
        protected float _deathInactiveDelay = 5f;
        [SerializeField]
        protected Transform _target = null;
        [SerializeField]
        protected Renderer[] _renders = null;

        public static event Action<int> onDestroyed;
        public static event Action<GameObject> onHealthGone;
        public static event Action onDisabled;
        public static event Func<Transform> onGetEndPoint;

        protected NavMeshAgent _agent;
        protected Animator _anim;
        protected Transform _endPoint;
        protected Collider _collider;

        public int StartingHealth { get; set; }
        public int Health { get; set; }

        public virtual void Init()
        {
            _agent = GetComponent<NavMeshAgent>();
            _anim = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
            _renders = GetComponentsInChildren<Renderer>();
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
            Activate();
        }

        public virtual void OnDisable()
        {
            onDisabled?.Invoke();
        }

        public virtual void Activate()
        {
            if (_agent.isStopped == true)
            {
                _agent.isStopped = false;
            }

            if (_collider.enabled == false)
            {
                _collider.enabled = true;
            }

            for (int i = 0; i < _renders.Length; i++)
            {
                Renderer render = _renders[i];
                if (render.enabled == false)
                {
                    render.enabled = true;
                }
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

        public void Destroyed()
        {
            onDestroyed?.Invoke(_currencyValue);
            onHealthGone?.Invoke(this.gameObject);
            _agent.isStopped = true;
            _explosionPrefab.SetActive(true);
            _anim.SetTrigger("Destroyed");
            _collider.enabled = false;
            StartCoroutine(InactiveCoroutine(_deathInactiveDelay));
        }

        public void ReachedPathEnd()
        {
            gameObject.SetActive(false);
        }

        public void Damage(int amount)
        {
            Health -= amount;

            if (Health < 1)
            {
                Destroyed();
            }
        }

        public Transform GetTarget()
        {
            return _target;
        }

        protected IEnumerator InactiveCoroutine(float inactiveDelay)
        {
            yield return new WaitForSeconds(inactiveDelay);
            _explosionPrefab.SetActive(false);

            for (int i = 0; i < _renders.Length; i++)
            {
                Renderer render = _renders[i];
                if (render.enabled == true)
                {
                    render.enabled = false;
                }
            }

            _anim.SetTrigger("Reset");
            yield return new WaitForSeconds(2f);
            gameObject.SetActive(false);
        }
    }
}

