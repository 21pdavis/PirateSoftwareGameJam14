using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Sunflower : MonoBehaviour
{
    public enum SunflowerState
    {
        Patrolling,
        Alerted,
        Chasing
    }

    [Header("Patrolling Options")]
    [SerializeField] private List<Vector2> patrolPoints;
    [SerializeField] private float speed = 200f;
    [SerializeField] private float nextWaypointDistance = 1f;

    // Pathfinding fields
    private Path path;
    private Seeker seeker;
    private Rigidbody2D rb;


    private SunflowerState state;
    // Editor visualization
    private GUIStyle style = new();
    // patrolling
    private int currentPathWaypoint = 1;
    private int patrolPointIndex = 0;
    private bool ascending = true;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        // start pathfinding
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);

        // some styling options for displaying 
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
#if UNITY_EDITOR
            Handles.Label(point, colorIndex.ToString(), style);
#endif
            colorIndex += 1;
        }

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, nextWaypointDistance);
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

    private void OnPathComplete(Path finishedPath)
    {
        if (finishedPath.error)
        {
            Debug.Log($"Pathfinding error: {path.errorLog}");
            return;
        }

        currentPathWaypoint = 1;
        path = finishedPath;
    }

    private void UpdatePath()
    {
        if (!seeker.IsDone())
            return;

        seeker.StartPath(rb.position, patrolPoints[patrolPointIndex], OnPathComplete);
    }

    private void Patrol()
    {
        if (path == null)
            return;

        // arrived, iterate patrol point, accounting for overflow/underflow
        if (
            currentPathWaypoint >= path.vectorPath.Count
            && patrolPoints.Count >= 2
        )
        {
            patrolPointIndex += ascending ? 1 : -1;
            if (patrolPointIndex < 0)
            {
                patrolPointIndex += 2;
            }
            else if (patrolPointIndex >= patrolPoints.Count)
            {
                patrolPointIndex -= 2;
            }
        }
        else if (currentPathWaypoint < path.vectorPath.Count)
        {
            // set course for next point
            Vector2 direction = ((Vector2)path.vectorPath[currentPathWaypoint] - rb.position).normalized;
            rb.AddForce(rb.mass * speed * Time.deltaTime * direction, ForceMode2D.Impulse);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentPathWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentPathWaypoint += 1;
            }
        }
    }
}
