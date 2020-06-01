using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyClass : MonoBehaviour
{
    [SerializeField]
    protected float speed = 1.5f;
    [SerializeField]
    protected int StartingHealth = 1;
    [SerializeField]
    protected int currentHealth;
    [SerializeField]
    protected int currencyValue = 0;

    protected bool reused = false;
    protected NavMeshAgent agent;
    protected Transform endPoint;
    protected float distance = Mathf.Infinity;

    public virtual void Init()
    {
        agent = GetComponent<NavMeshAgent>();
        endPoint = GameObject.FindGameObjectWithTag("End Point").transform;

        if (agent == null)
        {
            Debug.LogError("Nav Mesh Agent is NULL");
        }

        if (endPoint == null)
        {
            Debug.LogError("End Point is NULL");
        }
        currentHealth = StartingHealth;
        agent.speed = speed;
        agent.SetDestination(endPoint.position);
    }

    public virtual void Start()
    {
        Init();
    }

    public virtual void Update()
    {
        distance = Vector3.Distance(transform.position, agent.pathEndPosition);

        if (distance < 0.1f)
        {
            ReachedPathEnd();
        }

        if (currentHealth < 1)
        {
            gameObject.SetActive(false);
        }
    }

    public void Respawn()
    {
        currentHealth = StartingHealth;
        agent.speed = speed;
        agent.SetDestination(endPoint.position);
    }

    public void ReachedPathEnd()
    {
        gameObject.SetActive(false);
    }
}
