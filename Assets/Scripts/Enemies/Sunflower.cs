using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

public class Sunflower : MonoBehaviour
{
    public enum SunflowerState
    {
        Patrolling,
        Alerted,
        Chasing
    }

    [SerializeField] private List<Vector2> patrolPoints;

    private NavMeshAgent navMeshAgent;

    private SunflowerState state;
    // Editor visualization
    private GUIStyle style = new();
    // patrolling
    private int patrolPointIndex;
    private bool ascending;

    private void Start()
    {
        ascending = true;
        navMeshAgent = GetComponent<NavMeshAgent>();

        style.normal.textColor = Color.black;
        style.fontSize = 20;
    }

    private void OnDrawGizmosSelected()
    {
        int colorIndex = 0;
        foreach (Vector2 point in patrolPoints)
        {
            Gizmos.color = Helpers.AllColors[colorIndex % Helpers.AllColors.Length];
            Gizmos.DrawWireSphere(point, 0.5f);
            Handles.Label(point, colorIndex.ToString(), style);
            colorIndex += 1;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case SunflowerState.Patrolling:
                Patrol();
                break;
            case SunflowerState.Alerted:
                break;
            case SunflowerState.Chasing:
                break;
        }
    }

    private void Patrol()
    {
        if (Vector2.Distance(transform.position, patrolPoints[patrolPointIndex]) > 0.1f)
        {
            navMeshAgent.SetDestination(patrolPoints[patrolPointIndex]);
        }
        else
        {
            int nextPatrolPointIndex = patrolPointIndex + 1;

            if (patrolPointIndex > patrolPoints.Count)
            {
                // TODO: Left off here
            }
            else
            {

            }


        }
    }
}
