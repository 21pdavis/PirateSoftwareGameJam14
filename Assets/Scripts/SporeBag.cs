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
    [SerializeField] float cookTime;
    [SerializeField] float throwSpeed;
    [SerializeField] float arcSize;

    [Header("References")]
    [SerializeField] GameObject sporeCloudPrefab;

    internal BagState state = BagState.Held;

    private SpriteRenderer spriteRenderer;

    // throw positions
    private Vector2 origin;
    private Vector2 destination;
    private Vector2 relativeOrigin;
    private Vector2 relativeDestination;
    private Vector2 midpoint;

    // times
    private float spawnTime;
    private float timeThrown;
    private float travelTime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > spawnTime + cookTime)
        {
            // TODO: spawn spore cloud
        }

        switch (state)
        {
            case BagState.InFlight:
                //print("In flight");
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

    // TODO: rework to explode on a timer no matter what (maybe)
    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.5f);
        GameObject sporeCloud = Instantiate(sporeCloudPrefab, transform.position, Quaternion.identity);

        while (sporeCloud != null)
            yield return new WaitForEndOfFrame();

        float fadeTime = 2f;
        float startFadeTime = Time.time;
        while ((Time.time - startFadeTime) < fadeTime)
        {
            Color tmp = spriteRenderer.color;
            tmp.a = Mathf.Lerp(1f, 0f, (Time.time - startFadeTime) /  fadeTime);
            spriteRenderer.color = tmp;
            yield return new WaitForEndOfFrame();
        }
    }

    public void Throw(Vector2 origin, Vector2 destination)
    {
        this.origin = origin;
        this.destination = destination;

        midpoint = (this.origin + this.destination) / 2 - arcSize * Vector2.up; // TODO: get more consistent vector for offsetting
        relativeOrigin = this.origin - midpoint;
        relativeDestination = this.destination - midpoint;

        float arcLength = Helpers.ArcLength(midpoint, this.origin, this.destination);

        // speed = distance / time --> time = distance / speed
        travelTime = arcLength / throwSpeed;
        print($"travelTime: {travelTime}");
        timeThrown = Time.time;

        state = BagState.InFlight;
    }
}
