using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using static Helpers;

public class PlayerCombat : MonoBehaviour
{
    [Header("Animation Options")]
    [Tooltip("How many puffs per second the gun fires.")]
    [SerializeField] private float gunRateOfFire = 5f;
    [SerializeField] private Transform grenadeSpawnPoint;
    [Tooltip("The delay between being able to attack again.")]
    [SerializeField] private float grenadeShootDelay = 1f;
    [SerializeField] private Weapon startingWeapon = Weapon.Gun;
    [SerializeField] private float swapDelay = 0.1f;

    [Header("References")]
    [SerializeField] private GameObject sporeGrenadePrefab;
    [SerializeField] private Transform pivot;

    internal bool attacking = false;
    internal bool swapping = false;
    internal Weapon currentWeapon;

    private PlayerAnimation playerAnimation;
    private PlayerMovement playerMovement;

    // Do not need an enum to track this, but I think it will help code readability to have this instead of a bool (at least for me)
    public enum Weapon
    {
        Gun,
        Grenade
    }

    private void Start()
    {
        currentWeapon = startingWeapon;
        playerAnimation = GetComponent<PlayerAnimation>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (!context.started || attacking || swapping)
            return;

        attacking = true;

        if (currentWeapon == Weapon.Gun)
        {
            StartCoroutine(SpawnAndShootGunSporeCloud());
        }
        else
        {
            playerAnimation.ChangeAnimState(
                playerMovement.movementDirection == Vector2.zero ?
                PlayerAnimStates.shootGrenade : PlayerAnimStates.shootWalkGrenade
            );

            // TODO: this might be buggy...
            Invoke(nameof(SetNotAttacking), grenadeShootDelay);
        }
    }

    public void SwapWeapon(InputAction.CallbackContext context)
    {
        // TODO: grey out UI while swapping
        if (!context.started || attacking || swapping)
            return;

        swapping = true;

        if (currentWeapon == Weapon.Gun)
        {
            playerAnimation.ChangeAnimState(
                playerMovement.movementDirection == Vector2.zero ?
                PlayerAnimStates.swapGunToGrenade : PlayerAnimStates.swapGunToGrenadeWalk
            );
        }
        else // Grenade
        {
            playerAnimation.ChangeAnimState(
                playerMovement.movementDirection == Vector2.zero ?
                PlayerAnimStates.swapGrenadeToGun : PlayerAnimStates.swapGrenadeToGunWalk
            );
        }

        // set to not be swapping after the animation finishes
        Invoke(nameof(SetNotSwapping), swapDelay);
    }

    private IEnumerator SpawnAndShootGunSporeCloud()
    {
        while (Input.GetMouseButton(0))
        {
            print("Shooting!");

            playerAnimation.ChangeAnimState(
                playerMovement.movementDirection == Vector2.zero ?
                PlayerAnimStates.shootGun : PlayerAnimStates.shootWalkGun
            );

            yield return new WaitForSeconds(1 / gunRateOfFire);
        }
        attacking = false;
    }

    // called as an animation event
    public void SpawnAndShootGrenade()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        GameObject sporeGrenade = Instantiate(sporeGrenadePrefab, grenadeSpawnPoint.position, Quaternion.identity);
        sporeGrenade.GetComponent<SporeBag>().Throw(grenadeSpawnPoint.position, mouseWorldPosition);
    }

    private void SetNotAttacking()
    {
        attacking = false;
    }

    private void SetNotSwapping()
    {
        swapping = false;
        currentWeapon = currentWeapon == Weapon.Gun ? Weapon.Grenade : Weapon.Gun;
    }
}
