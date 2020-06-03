using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyClass : MonoBehaviour
{
    [SerializeField]
    protected float speed = 1.5f;
    [SerializeField]
    protected int StartingHealth = 1;
    protected int currentHealth;
    [SerializeField]
    protected int currencyValue = 0;

    //public delegate void OnDestroyed(int currencyValue);
    //public static event OnDestroyed onDestroyed;
    public static Action<int> OnDestroyed;

    //public delegate void OnDisabled();
    //public static event OnDisabled onDisabled;
    public static Action OnDisabled;

    //public delegate Transform OnGetEndPoint();
    //public static event OnGetEndPoint onGetEndPoint;
    public static Func<Transform> OnGetEndPoint;

    protected NavMeshAgent agent;
    protected Transform endPoint;

    public virtual void Init()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
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

    public virtual void Update()
    {

    }

    public virtual void Activate()
    {
        //Transform endPoint = SpawnManager.Instance.GetEndPoint();
        Transform endPoint = OnGetEndPoint?.Invoke();
        if (endPoint == null)
        {
            Debug.LogError("End Point was not retrieved.");
            endPoint.position = Vector3.zero;
        }
        currentHealth = StartingHealth;
        agent.speed = speed;
        agent.SetDestination(endPoint.position);
    }

    public void Destroyed()
    {
        //UIManager.Instance.AddCurrency(currencyValue); or something
        //SpawnManager.Instance.Despawn();
        OnDestroyed?.Invoke(currencyValue);
        OnDisabled?.Invoke();
        gameObject.SetActive(false);
    }

    public void ReachedPathEnd()
    {
        //SpawnManager.Instance.Despawn();
        OnDisabled?.Invoke();
        gameObject.SetActive(false);
    }
}
