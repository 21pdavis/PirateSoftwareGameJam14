using UnityEngine;

public class SporeBag : MonoBehaviour
{
    public enum BagState
    {
        Held,
        InFlight,
        Exploding
    }

    [SerializeField] float cookTime;
    [SerializeField] float throwSpeed;
    [SerializeField] float arcSize;

    internal BagState state = BagState.Held;

    private float spawnTime;
    private Vector2 throwOrigin;
    private Vector2 throwDestination;
    private Vector2 throwRelOrigin;
    private Vector2 throwRelDestination;
    private Vector2 throwMidpoint;
    private float timeThrown;
    private float travelTime;

    private void Start()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time > spawnTime + cookTime)
        {
            // TODO: spawn spore cloud
            //Destroy(gameObject);
            //print("BAG EXPLODED!");
        }

        switch (state)
        {
            case BagState.InFlight:
                //print("In flight");
                if (Vector2.Distance(transform.position, throwDestination) < 0.1f)
                {
                    state = BagState.Exploding;
                    break;
                }

                transform.position = Vector3.Slerp(throwRelOrigin, throwRelDestination, (Time.time - timeThrown) / travelTime);
                transform.position += (Vector3)throwMidpoint;
                break;
            case BagState.Exploding:
                //print("Exploding");
                break;
        }
    }

    public void Throw(Vector2 origin, Vector2 destination)
    {
        throwOrigin = origin;
        throwDestination = destination;

        throwMidpoint = (throwOrigin + throwDestination) / 2 - arcSize * Vector2.up; // TODO: get more consistent vector for offsetting
        throwRelOrigin = throwOrigin - throwMidpoint;
        throwRelDestination = throwDestination - throwMidpoint;

        float arcLength = Helpers.ArcLength(throwMidpoint, throwOrigin, throwDestination);

        // speed = distance / time --> time = distance / speed
        travelTime = arcLength / throwSpeed;
        print($"travelTime: {travelTime}");
        timeThrown = Time.time;

        state = BagState.InFlight;
    }
}
