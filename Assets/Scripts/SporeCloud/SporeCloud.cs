using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SporeCloud : MonoBehaviour
{
    public float maxScaleMultiplier = 2.25f;
    public float cloudScaleSpeed = 5f;
    public float lifetime = 5f;
    public Vector3 initialVelocity = Vector3.zero;
    public Vector3 velocityDecay = Vector3.zero;

    [Header("References")]
    [SerializeField] private List<TileBase> grassTiles;
    [SerializeField] private List<TileBase> mushroomTiles;

    private SpriteRenderer spriteRenderer;
    private Tilemap groundMap;
    private Tilemap mushroomMap;
    private Vector3 initialScale;
    private Vector3 maxScale;
    private Vector3 velocity;
    private float startTime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        groundMap = GameObject.Find("Ground").GetComponent<Tilemap>();
        mushroomMap = GameObject.Find("Mushrooms").GetComponent<Tilemap>();
        initialScale = transform.localScale;
        maxScale = maxScaleMultiplier * initialScale;
        startTime = Time.time;
        velocity = initialVelocity;
    }

    private void Update()
    {
        // update cloud position
        transform.position += Time.deltaTime * velocity;
        Vector3 timeScaledDecay = Time.deltaTime * velocityDecay;

        velocity = new Vector3(
            initialVelocity.x > 0 ? Mathf.Max(0, velocity.x - timeScaledDecay.x) : Mathf.Min(0, velocity.x - timeScaledDecay.x),
            initialVelocity.x > 0 ? Mathf.Max(0, velocity.y - timeScaledDecay.y) : Mathf.Min(0, velocity.y - timeScaledDecay.y),
            initialVelocity.x > 0 ? Mathf.Max(0, velocity.z - timeScaledDecay.z) : Mathf.Min(0, velocity.z - timeScaledDecay.z)
        );

        // update cloud scale
        transform.localScale = Vector3.Lerp(transform.localScale, maxScale, cloudScaleSpeed * Time.deltaTime);

        SpreadMushroomsInBounds();

        // reduce alpha in step. Can't directly modify .a property, so modify a temp color first
        Color temp = spriteRenderer.color;
        temp.a = Mathf.Lerp(1f, 0f, (Time.time - startTime) / lifetime);
        spriteRenderer.color = temp;
        if (temp.a == 0f)
        {
            Destroy(gameObject);
        }
    }

    private void SpreadMushroomsInBounds()
    {
        Bounds spriteBounds = spriteRenderer.bounds;
        BoundsInt spriteBoundsTruncated = new BoundsInt(
            position: new Vector3Int((int)spriteRenderer.bounds.min.x + 1, (int)spriteRenderer.bounds.min.y + 1, 0),
            size: new Vector3Int((int)spriteBounds.size.x, (int)spriteBounds.size.y, 1)
        );

        BoundsInt.PositionEnumerator positions = spriteBoundsTruncated.allPositionsWithin;

        do {
            TileBase mushroom = Helpers.RandFromList(mushroomTiles);
            if (!mushroomTiles.Contains(mushroomMap.GetTile(positions.Current)) && grassTiles.Contains(groundMap.GetTile(positions.Current)))
            {
               mushroomMap.SetTile(positions.Current, mushroom);
            }
        } while (positions.MoveNext());
    }
}
