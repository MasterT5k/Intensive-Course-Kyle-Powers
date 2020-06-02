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
    protected float distance = Mathf.Infinity;

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

    public virtual void Update()
    {
        distance = Vector3.Distance(transform.position, endPoint.position);

        if (distance < 2f)
        {
            ReachedPathEnd();
        }

        if (currentHealth < 1)
        {
            Destroyed();
        }
    }

    public virtual void Activate()
    {
        endPoint = GameObject.FindGameObjectWithTag("End Point").transform;

        if (endPoint == null)
        {
            Debug.LogError("End Point is NULL");
        }
        currentHealth = StartingHealth;
        agent.speed = speed;
        agent.SetDestination(endPoint.position);
    }

    public void Destroyed()
    {
        //UIManager.Instance.AddCurrency(currencyValue); or something
        distance = Mathf.Infinity;
        SpawnManager.Instance.Despawn();
        gameObject.SetActive(false);
    }

    public void ReachedPathEnd()
    {
        distance = Mathf.Infinity;
        SpawnManager.Instance.Despawn();
        gameObject.SetActive(false);
    }
}
