using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Options")]
    [SerializeField] private float movementSpeed;

    [SerializeField] private float interactRange;

    //[Header("Tilemaps")]
    //[SerializeField]
    //private Tilemap ground;

    //[SerializeField]
    //private Tilemap mushrooms;

    private Vector2 movementDirection = Vector2.zero;

    private void Update()
    {
        transform.position += movementSpeed * Time.deltaTime * (Vector3)movementDirection;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(transform.position, interactRange);
    //}

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
