using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowBot : MonoBehaviour
{
    [SerializeField] private LayerMask collisionLayers;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float moveTime = 0.3f;
    [SerializeField] private AudioClip baldiHitClip;
    [SerializeField] private AudioClip baldiDoorSmash;
    [SerializeField] private int level;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform target;

    void Start()
    {
        agent.speed = 0f;
        StartCoroutine(BaldiFollow());
    }

    void Update()
    {
        Vector3 lookDir = (target.position - transform.position).normalized;
        lookDir.y = 0f;

        transform.rotation = Quaternion.LookRotation(lookDir);

        agent.SetDestination(target.position);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(agent.steeringTarget, Vector3.one);
    }

    private IEnumerator BaldiFollow()
    {
        while (true)
        {
            Vector3 nextPos = transform.position + (agent.steeringTarget - transform.position).normalized * speed;
            float sqrDistance = (agent.steeringTarget - transform.position).sqrMagnitude;
            if (sqrDistance <= speed * speed && sqrDistance > 16f) nextPos = agent.steeringTarget;

            nextPos.y = transform.position.y;

            float elapsed = 0f;
            while (elapsed < moveTime)
            {
                transform.position = Vector3.Lerp(transform.position, nextPos, elapsed / moveTime);
                elapsed += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(1f / level);
            AudioManager.Instance.PlayOnce(baldiHitClip, transform.position);
        }
    }
}
