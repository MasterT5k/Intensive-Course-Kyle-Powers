using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace GameDevHQ.Enemy.EnemyClassNS
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public abstract class EnemyClass : MonoBehaviour
    {
        [SerializeField]
        protected float _speed = 1.5f;
        [SerializeField]
        protected int _startingHealth = 1;
        [SerializeField]
        protected int _currentHealth;
        [SerializeField]
        protected int _currencyValue = 0;
        [SerializeField]
        protected GameObject _explosionPrefab = null;
        [SerializeField]
        protected float _deathInactiveDelay = 5f;

        public static event Action<int> onDestroyed;
        public static event Action<GameObject> onHealthGone;
        public static event Action onDisabled;
        public static event Func<Transform> onGetEndPoint;

        protected NavMeshAgent _agent;
        protected Animator _anim;
        protected Transform _endPoint;

        public virtual void Init()
        {
            _agent = GetComponent<NavMeshAgent>();
            _anim = GetComponent<Animator>();

            if (_agent == null)
            {
                Debug.LogError("Nav Mesh Agent is NULL");
            }

            if (_anim == null)
            {
                Debug.LogError("Animator in NULL");
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

            if (_endPoint == null)
            {
                _endPoint = onGetEndPoint?.Invoke();
                Debug.Log("End Point was retrieved.");
                _currentHealth = _startingHealth;
                _agent.speed = _speed;
                _agent.SetDestination(_endPoint.position);
            }
            else
            {
                _currentHealth = _startingHealth;
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
            StartCoroutine(InactiveCoroutine(_deathInactiveDelay));
        }

        public void ReachedPathEnd()
        {
            gameObject.SetActive(false);
        }

        public void Damage(int amount)
        {
            _currentHealth -= amount;

            if (_currentHealth < 1)
            {
                Destroyed();
            }
        }

        protected IEnumerator InactiveCoroutine(float inactiveDelay)
        {
            yield return new WaitForSeconds(inactiveDelay);
            _explosionPrefab.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}

