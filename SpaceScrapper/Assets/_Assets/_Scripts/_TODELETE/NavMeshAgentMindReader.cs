using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAgentMindReader : MonoBehaviour
{
    NavMeshAgent agent;

    private void Awake()
    {
        TryGetComponent(out agent);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || agent == null) return;

        Gizmos.color = agent.hasPath ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position, agent.radius * 1.1f);
    }
}
