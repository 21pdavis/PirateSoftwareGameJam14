using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Options")]
    [SerializeField] private float movementSpeed;

    [SerializeField] private float interactRange;

    private Rigidbody2D rb;
    private Vector2 movementDirection = Vector2.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + movementSpeed * Time.deltaTime * movementDirection);
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            movementDirection = context.ReadValue<Vector2>();
        }
        else if (context.canceled)
        {
            movementDirection = Vector2.zero;
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        Collider2D interactable = Physics2D.OverlapCircleAll((Vector2)transform.position, interactRange).FirstOrDefault();
        if (interactable != null)
        {
            interactable.GetComponent<IInteractable>().InteractedWith.Invoke(new EventData(interactable.transform.position));
        }
    }
}
