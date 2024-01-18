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

    internal BagState state = BagState.Held;

    private float spawnTime;
    private Vector2 throwOrigin;
    private Vector2 throwDestination;
    private Vector2 throwMidpoint;

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
                // TODO: temp
                transform.position = throwDestination;
                break;
            case BagState.Exploding:
                //print("Exploding");
                break;
        }
    }

    public void Throw(Vector2 origin, Vector2 destination)
    {
        print("Throwing");

        throwOrigin = origin;
        throwDestination = destination;
        throwMidpoint = (throwOrigin + throwDestination) / 2;

        state = BagState.InFlight;
    }
}
