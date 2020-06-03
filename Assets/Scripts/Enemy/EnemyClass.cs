using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyClass : MonoBehaviour
{
    [SerializeField]
    protected float speed = 1.5f;
    [SerializeField]
    protected int StartingHealth = 1;
    protected int currentHealth;
    [SerializeField]
    protected int currencyValue = 0;

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
        Transform endPoint = SpawnManager.Instance.GetEndPoint();
        currentHealth = StartingHealth;
        agent.speed = speed;
        agent.SetDestination(endPoint.position);
    }

    public void Destroyed()
    {
        //UIManager.Instance.AddCurrency(currencyValue); or something
        SpawnManager.Instance.Despawn();
        gameObject.SetActive(false);
    }

    public void ReachedPathEnd()
    {
        SpawnManager.Instance.Despawn();
        gameObject.SetActive(false);
    }
}
