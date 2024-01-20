using UnityEngine;

public class SpriteFlip : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (rb.velocity.x >= 0.01f)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.velocity.x <= -0.01f)
        {
            spriteRenderer.flipX = true;
        }
    }
}
