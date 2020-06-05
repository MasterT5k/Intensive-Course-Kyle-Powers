using System;
using UnityEditor.MemoryProfiler;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyClass : MonoBehaviour
{
    [SerializeField]
    protected float _speed = 1.5f;
    [SerializeField]
    protected int _startingHealth = 1;
    protected int _currentHealth;
    [SerializeField]
    protected int _currencyValue = 0;

    public static event Action<int> OnDestroyed;
    public static event Action OnDisabled;
    public static event Func<Transform> OnGetEndPoint;

    protected NavMeshAgent _agent;
    protected Transform _endPoint;

    public virtual void Init()
    {
        _agent = GetComponent<NavMeshAgent>();

        if (_agent == null)
        {
            Debug.LogError("Nav Mesh Agent is NULL");
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
        OnDisabled?.Invoke();
    }

    public virtual void Activate()
    {
        if (_endPoint == null)
        {
            _endPoint = OnGetEndPoint?.Invoke();
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
        OnDestroyed?.Invoke(_currencyValue);        
        gameObject.SetActive(false);
    }

    public void ReachedPathEnd()
    {
        gameObject.SetActive(false);
    }
}
