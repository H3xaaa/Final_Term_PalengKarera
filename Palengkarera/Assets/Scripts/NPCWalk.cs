using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCWalk : MonoBehaviour
{
    [Header("Wait Delay (seconds)")]
    public float waitTimeMin = 3f;   
    public float waitTimeMax = 3f;    

    [Header("Initial Stagger (seconds)")]
    public float randomStartDelayMax = 1f;

    private bool isWaiting = false;
    private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 6f;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(Random.Range(0f, randomStartDelayMax));
        PickNewDestination();
    }

    void Update()
    {
        if (!isWaiting
            && !agent.pathPending
            && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(ChangeDestinationAfterDelay());
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isWaiting && collision.collider is BoxCollider)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeDestinationAfterDelay());
        }
    }

    IEnumerator ChangeDestinationAfterDelay()
    {
        isWaiting = true;
        agent.isStopped = true;
        // fixed 3 s delay
        yield return new WaitForSeconds(Random.Range(waitTimeMin, waitTimeMax));
        agent.isStopped = false;
        PickNewDestination();
        isWaiting = false;
    }

    void PickNewDestination()
    {
        Vector3 randomPoint = transform.position + new Vector3(
            Random.Range(-10f, 10f),
            0,
            Random.Range(-10f, 10f)
        );

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 10f, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}