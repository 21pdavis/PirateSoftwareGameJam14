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
    [SerializeField] private float rateOfFire = 3f;
    [SerializeField] private float timeBetweenBursts = 3f;

    [Header("References")]
    [SerializeField] private Transform shootPoint;
    [SerializeField] private GameObject nectarBullet;

    private GameObject canvas;

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
    private bool shooting = false;
    private Path lastPath = null;
    private Vector2 movementDirection = Vector2.zero;
    private float startedShootWait;
    private string currentAnim;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        canvas = GameObject.Find("Canvas");

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

        if (path != null && !shooting && currentPathWaypoint < path.vectorPath.Count)
        {
            // set course for next point
            movementDirection = ((Vector2)path.vectorPath[currentPathWaypoint] - rb.position).normalized;
            rb.AddForce(rb.mass * speed * Time.deltaTime * movementDirection, ForceMode2D.Impulse);

            UpdateSpriteDirection();

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentPathWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentPathWaypoint += 1;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Time.timeScale = 0f;
            canvas.GetComponent<UIElementManager>().HUD.SetActive(false);
            canvas.GetComponent<UIElementManager>().deathMenu.SetActive(true);
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
        if (!seeker.IsDone() || (state == SunflowerState.Patrolling && (Time.time < reachedPointTime + stopTime || reachedPointTime != -1)))
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
        
        if (rb.velocity.magnitude < 0.25f && !shooting)
        {
            ChangeAnimState(SunflowerAnimStates.idle);
        }
        else
        {
            ChangeAnimState(SunflowerAnimStates.walk);
        }

        PatrolPlayerScan();
    }

    private void Chase()
    {
        if (!shooting)
        {
            ChangeAnimState(SunflowerAnimStates.walk);
        }
        ChasePlayerScan();
    }

    private void Attack()
    {
        if (!shooting && Time.time > startedShootWait + timeBetweenBursts && ScanForPlayer(attackRadius) != null)
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
        Collider2D playerCollider = ScanForPlayer(aggroRadius);
        if (playerCollider == null)
        {
            return;
        }

        state = SunflowerState.Chasing;
    }

    private void ChasePlayerScan()
    {
        Collider2D playerCollider = ScanForPlayer(aggroRadius);
        if (playerCollider == null)
        {
            print("back to patrolling");
            state = SunflowerState.Patrolling;
            return;
        }

        if (Time.time > startedShootWait + timeBetweenBursts && ScanForPlayer(aggroRadius))
        {
            state = SunflowerState.Attacking;
        }
    }

    private IEnumerator ShootGun()
    {
        shooting = true;

        ChangeAnimState(SunflowerAnimStates.shoot);
        float animStartTime = Time.time;
        while (Time.time < animStartTime + 1.5f * animator.GetCurrentAnimatorStateInfo(0).length)
        {
            GameObject bullet = Instantiate(nectarBullet, shootPoint.position, Quaternion.identity);
            Vector3 direction = (player.GetComponent<PlayerMovement>().pivot.position - bullet.transform.position).normalized;
            bullet.transform.right = direction;
            bullet.GetComponent<PollenBullet>().trajectory = direction;
            yield return new WaitForSeconds(1 / rateOfFire);
        }

        shooting = false;
        startedShootWait = Time.time;
    }

    private Collider2D ScanForPlayer(float radius)
    {
        Collider2D playerCollider = Physics2D.OverlapCircleAll(transform.position, radius)
                                    .Where(collider => collider.CompareTag("Player"))
                                    .FirstOrDefault();
        return playerCollider;
    }

    private void ChangeAnimState(string animName)
    {
        if (currentAnim == animName)
            return;

        //Debug.Log($"Changing to {animName}");
        animator.Play(animName);
        currentAnim = animName;
    }
}
