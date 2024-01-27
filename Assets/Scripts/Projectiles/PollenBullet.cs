using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Helpers;

public class PollenBullet : MonoBehaviour
{
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private float speed;

    internal Vector2 trajectory;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprites[RandInRange(0, sprites.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Time.deltaTime * speed * (Vector3)trajectory;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement movement = collision.GetComponent<PlayerMovement>();
            movement.SlowPlayer();
            Destroy(gameObject);
        }
    }
}
