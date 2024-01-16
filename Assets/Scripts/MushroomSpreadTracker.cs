using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MushroomSpreadTracker : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] private Tilemap ground;
    [SerializeField] private Tilemap mushrooms;

    [Header("Tiles")]
    [SerializeField] private Tile grass;
    [SerializeField] private Tile mushroom;

    private Grid grid;

    private void Start()
    {
        grid = GetComponent<Grid>();
    }

    public void StartMushroomSpread(EventData data)
    {
        StartCoroutine(FloodFillMushrooms(data.Position));
    }

    private IEnumerator FloodFillMushrooms(Vector2 startPosition)
    {
        List<Vector2> mushroomedPositions = new() { new Vector2((int)startPosition.x, (int)startPosition.y) };

        // TODO: different condition
        while (true)
        {
            List<Vector2> mushroomedPositionsThisIteration = new();

            foreach (Vector2 position in mushroomedPositions)
            {
                List<Vector2> neighboringPositions = new()
                {
                    new Vector2(position.x, position.y + 1), // up
                    new Vector2(position.x + 1, position.y), // right
                    new Vector2(position.x, position.y - 1), // down
                    new Vector2(position.x - 1, position.y), // left
                };

                // TODO: eight-directional code here in case we want to use it
                //List<Vector2> neighboringPositions = new();
                //for (int i = -1; i <= 1; ++i)
                //{
                //    for (int j = -1; j <= 1; ++j)
                //    {
                //        neighboringPositions.Add(new Vector2(position.x + i, position.y + j));
                //    }
                //}

                for (int i = 0; i < neighboringPositions.Count; ++i)
                {
                    Vector3Int castedPos = new((int)neighboringPositions[i].x, (int)neighboringPositions[i].y);

                    // checking conditions in steps here for performance reasons
                    TileBase tileInMushroomMap = mushrooms.GetTile(castedPos);
                    if (tileInMushroomMap != null)
                        continue;

                    TileBase tileInGroundMap = ground.GetTile(castedPos);
                    if (tileInGroundMap == null)
                        continue;

                    if (!tileInGroundMap.name.Equals(grass.name))
                        continue;

                    if (neighboringPositions[i] == startPosition)
                        continue;

                    mushrooms.SetTile(castedPos, mushroom);
                    mushroomedPositionsThisIteration.Add(neighboringPositions[i]);
                }
            }

            mushroomedPositions.AddRange(mushroomedPositionsThisIteration);
            yield return new WaitForSeconds(1);
        }
    }

}
