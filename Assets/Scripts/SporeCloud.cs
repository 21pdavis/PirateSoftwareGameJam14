using UnityEngine;
using UnityEngine.Tilemaps;

public class SporeCloud : MonoBehaviour
{
    [SerializeField] float lifetime;

    [Header("References")]
    //[SerializeField] Tilemap mushroomMap;
    [SerializeField] Tile mushroomTile;

    private SpriteRenderer spriteRenderer;
    private Tilemap mushroomMap;
    private Vector3 initialScale;
    private float startTime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // TODO: maybe need better solution but this works
        mushroomMap = GameObject.Find("Mushrooms").GetComponent<Tilemap>();
        initialScale = transform.localScale;
        startTime = Time.time;
    }

    private void Update()
    {
        transform.localScale += Time.deltaTime * 1.01f * initialScale;
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
            if (mushroomMap.GetTile(positions.Current) != mushroomTile)
            {
               mushroomMap.SetTile(positions.Current, mushroomTile);
            }
        } while (positions.MoveNext());
    }
}
