using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

using static Helpers;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player Options")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float interactRange;

    [Header("References")]
    [SerializeField] private Transform pivot;

    internal Vector2 movementDirection = Vector2.zero;
    
    private Rigidbody2D rb;
    private PlayerCombat playerCombat;
    private PlayerAnimation playerAnimation;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCombat = GetComponent<PlayerCombat>();
        playerAnimation = GetComponent<PlayerAnimation>();
    }

    private void Update()
    {
        if (!playerCombat.attacking && !playerCombat.swapping)
        {
            if (movementDirection == Vector2.zero)
            {
                playerAnimation.ChangeAnimState(playerCombat.currentWeapon == PlayerCombat.Weapon.Gun ? PlayerAnimStates.idleGun : PlayerAnimStates.idleGrenade);
            }
            else
            {
                string animName = playerCombat.currentWeapon == PlayerCombat.Weapon.Gun ? PlayerAnimStates.walkGun : PlayerAnimStates.walkGrenade;

                if (playerAnimation.currentAnim.Contains("swap") || playerAnimation.currentAnim.Contains("shoot"))
                {
                    playerAnimation.CrossfadeAnimState(animName, 0.25f);
                }
                else
                {
                    playerAnimation.ChangeAnimState(animName);
                }
            }
        }

        UpdateSpriteDirection();
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

        Collider2D interactable = Physics2D.OverlapCircleAll((Vector2)pivot.position, interactRange).FirstOrDefault();
        if (interactable != null)
        {
            interactable.GetComponent<IInteractable>().InteractedWith.Invoke(new EventData(interactable.transform.position));
        }
    }

    private void UpdateSpriteDirection()
    {
        if (movementDirection.x > 0.01f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (movementDirection.x < -0.01f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}
