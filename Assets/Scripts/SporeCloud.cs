using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SporeCloud : MonoBehaviour
{
    [SerializeField] float maxScaleMultiplier;
    [SerializeField] float cloudScaleSpeed;
    [SerializeField] float lifetime;

    [Header("References")]
    [SerializeField] List<TileBase> mushroomTiles;

    private SpriteRenderer spriteRenderer;
    private Tilemap mushroomMap;
    private Vector3 initialScale;
    private Vector3 maxScale;
    private float startTime;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // TODO: maybe need better solution but this works
        mushroomMap = GameObject.Find("Mushrooms").GetComponent<Tilemap>();
        initialScale = transform.localScale;
        maxScale = maxScaleMultiplier * initialScale;
        startTime = Time.time;
    }

    private void Update()
    {
        //Vector3 nextScale = Time.deltaTime * (1 + cloudScaleSpeed) * initialScale;
        //if (nextScale.magnitude < maxScale.magnitude)
        //{
        //    transform.localScale = nextScale;
        //}

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

    //void DrawBounds(Bounds b, float delay = 0)
    //{
    //    // bottom
    //    var p1 = new Vector3(b.min.x, b.min.y, b.min.z);
    //    var p2 = new Vector3(b.max.x, b.min.y, b.min.z);
    //    var p3 = new Vector3(b.max.x, b.min.y, b.max.z);
    //    var p4 = new Vector3(b.min.x, b.min.y, b.max.z);

    //    Debug.DrawLine(p1, p2, Color.blue, delay);
    //    Debug.DrawLine(p2, p3, Color.red, delay);
    //    Debug.DrawLine(p3, p4, Color.yellow, delay);
    //    Debug.DrawLine(p4, p1, Color.magenta, delay);

    //    // top
    //    var p5 = new Vector3(b.min.x, b.max.y, b.min.z);
    //    var p6 = new Vector3(b.max.x, b.max.y, b.min.z);
    //    var p7 = new Vector3(b.max.x, b.max.y, b.max.z);
    //    var p8 = new Vector3(b.min.x, b.max.y, b.max.z);

    //    Debug.DrawLine(p5, p6, Color.blue, delay);
    //    Debug.DrawLine(p6, p7, Color.red, delay);
    //    Debug.DrawLine(p7, p8, Color.yellow, delay);
    //    Debug.DrawLine(p8, p5, Color.magenta, delay);

    //    // sides
    //    Debug.DrawLine(p1, p5, Color.white, delay);
    //    Debug.DrawLine(p2, p6, Color.gray, delay);
    //    Debug.DrawLine(p3, p7, Color.green, delay);
    //    Debug.DrawLine(p4, p8, Color.cyan, delay);
    //}

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
            if (!mushroomTiles.Contains(mushroomMap.GetTile(positions.Current)))
            {
               mushroomMap.SetTile(positions.Current, mushroom);
            }
        } while (positions.MoveNext());
    }
}
