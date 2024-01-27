using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

using static Helpers;

public class Sunflower : MonoBehaviour
{
    public enum SunflowerState
    {
        Patrolling,
        Attacking,
        Chasing
    }

    [Header("Patrolling Options")]
    [SerializeField] private List<Vector2> patrolPoints;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float nextWaypointDistance = 1f;
    [Tooltip("How long the flower will stop for at a patrol point before moving on")]
    [SerializeField] private float stopTime = 2.1f;

    [Header("Attack Options")]
    [SerializeField] private float aggroRadius = 5f;
    [SerializeField] private float attackRadius = 2.5f;

    [Header("References")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private List<GameObject> nectarBullets;

    private Path path;
    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;

    private GameObject player;
    private SunflowerState state;
    private GUIStyle style = new();
    private int currentPathWaypoint = 1;
    private int patrolPointIndex = 0;
    private float reachedPointTime = -1;
    private bool ascending = true;
    private Path lastPath = null;
    private Vector2 movementDirection = Vector2.zero;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        // start pathfinding
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);

        // some styling options for displaying
        style.normal.textColor = Color.black;
        style.fontSize = 20;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }

    private void OnDrawGizmosSelected()
    {
        int colorIndex = 0;
        foreach (Vector2 point in patrolPoints)
        {
            Gizmos.color = AllColors[colorIndex % Helpers.AllColors.Length];
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
            case SunflowerState.Chasing:
                Chase();
                break;
            case SunflowerState.Attacking:
                Attack();
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
        if (!seeker.IsDone() || (Time.time < reachedPointTime + stopTime && reachedPointTime != -1))
            return;

        switch (state)
        {
            case SunflowerState.Patrolling:
                seeker.StartPath(rb.position, patrolPoints[patrolPointIndex], OnPathComplete);
                break;
            case SunflowerState.Chasing:
                seeker.StartPath(rb.position, player.transform.position, OnPathComplete);
                break;
            case SunflowerState.Attacking:
                Attack();
                break;
        }

    }

    private void Patrol()
    {
        if (path == null)
            return;

        // arrived, iterate patrol point, accounting for overflow/underflow
        if (
            currentPathWaypoint >= path.vectorPath.Count
            && patrolPoints.Count >= 2
            && lastPath != path
        )
        {
            lastPath = path;
            patrolPointIndex += ascending ? 1 : -1;
            if (patrolPointIndex < 0)
            {
                patrolPointIndex += 2;
                ascending = true;
            }
            else if (patrolPointIndex >= patrolPoints.Count)
            {
                patrolPointIndex -= 2;
                ascending = false;
            }

            reachedPointTime = Time.time;
        }
        else if (currentPathWaypoint < path.vectorPath.Count)
        {
            // set course for next point
            movementDirection = ((Vector2)path.vectorPath[currentPathWaypoint] - rb.position).normalized;
            UpdateSpriteDirection();
            rb.AddForce(rb.mass * speed * Time.deltaTime * movementDirection, ForceMode2D.Impulse);

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentPathWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentPathWaypoint += 1;
            }
        }
        
        if (rb.velocity.magnitude < 0.25f)
        {
            animator.Play(SunflowerAnimStates.idle);
        }
        else
        {
            animator.Play(SunflowerAnimStates.walk);
        }
    }

    private void Chase()
    {
        ChasePlayerScan();
    }

    private void Attack()
    {
        if (ScanForPlayer() != null)
        {
            StartCoroutine(ShootGun());
        }
        else
        {
            state = SunflowerState.Chasing;
        }
    }

    private void UpdateSpriteDirection()
    {
        if (movementDirection.x > 0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (movementDirection.x < -0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void PatrolPlayerScan()
    {
        Collider2D playerCollider = ScanForPlayer();
        if (playerCollider == null)
        {
            return;
        }

        state = SunflowerState.Chasing;
    }

    private void ChasePlayerScan()
    {
        Collider2D playerCollider = ScanForPlayer();
        if (playerCollider == null)
        {
            state = SunflowerState.Patrolling;
            return;
        }

        // TODO: alert sound, defeat, victory
        state = SunflowerState.Attacking;
    }

    private IEnumerator ShootGun()
    {
        yield return null;
    }

    private Collider2D ScanForPlayer()
    {
        Collider2D playerCollider = Physics2D.OverlapCircleAll(transform.position, aggroRadius)
                                    .Where(collider => collider.CompareTag("Player"))
                                    .FirstOrDefault();
        return playerCollider;
    }
}
