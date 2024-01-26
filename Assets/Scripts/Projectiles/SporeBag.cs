using System.Collections;
using UnityEngine;

public class SporeBag : MonoBehaviour
{
    public enum BagState
    {
        Held,
        InFlight,
        Exploding
    }

    [Header("Throw Options")]
    [SerializeField] private float throwSpeed;
    [SerializeField] private float arcRadius;

    [Header("References")]
    [SerializeField] GameObject sporeCloudPrefab;

    internal BagState state = BagState.Held;

    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    // throw positions
    private Vector2 origin;
    private Vector2 destination;
    private Vector2 relativeOrigin;
    private Vector2 relativeDestination;
    private Vector2 midpoint;

    // times
    private float timeThrown;
    private float travelTime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        switch (state)
        {
            case BagState.InFlight:
                if (Vector2.Distance(transform.position, destination) < 0.1f)
                {
                    StartCoroutine(Explode());
                    state = BagState.Exploding;
                    break;
                }

                transform.position = Vector3.Slerp(relativeOrigin, relativeDestination, (Time.time - timeThrown) / travelTime);
                transform.position += (Vector3)midpoint;
                break;
            case BagState.Exploding:
                break;
        }
    }

    private IEnumerator Explode()
    {
        audioSource.Play();
        Instantiate(sporeCloudPrefab, transform.position, Quaternion.identity);

        Color tmp = spriteRenderer.color;
        tmp.a = 0f;
        spriteRenderer.color = tmp;

        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }

    public void Throw(Vector2 throwOrigin, Vector2 throwDestination)
    {
        Debug.DrawLine(transform.position, throwOrigin, Color.green, 5f);

        origin = throwOrigin;
        destination = throwDestination;

        float rotationAmount = (throwDestination - throwOrigin).x > 0 ? -90f : 90f;
        midpoint = (origin + destination) / 2 + arcRadius * (Vector2)(Quaternion.Euler(0f, 0f, rotationAmount) * (destination - origin).normalized);
        relativeOrigin = origin - midpoint;
        relativeDestination = destination - midpoint;

        // visualizing the arc
        Debug.DrawLine(origin, midpoint, Color.blue, 2.5f);
        Debug.DrawLine(midpoint, destination, Color.red, 2.5f);

        // speed = distance / time --> time = distance / speed
        float arcLength = Helpers.ArcLength(midpoint, origin, destination);
        travelTime = arcLength / throwSpeed;
        timeThrown = Time.time;

        state = BagState.InFlight;
    }
}
