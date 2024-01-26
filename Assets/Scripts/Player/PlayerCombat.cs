using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using static Helpers;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Options")]
    [SerializeField] private Transform grenadeSpawnPoint;
    [Tooltip("The delay between being able to attack again.")]
    [SerializeField] private float grenadeShootDelay = 1f;
    [SerializeField] private Weapon startingWeapon = Weapon.Gun;
    [SerializeField] private float swapDelay = 0.1f;
    [SerializeField] private float gunCloudSpeed = 2.5f;
    [SerializeField] private float gunCloudDecay = 0.5f;

    [Header("References")]
    [SerializeField] private GameObject sporeGrenadePrefab;
    [SerializeField] private GameObject sporeCloudPrefab;
    [SerializeField] private Transform gunShootPoint;
    [SerializeField] private Transform pivot;
    [SerializeField] private AudioSource grenadeShootSource;
    [SerializeField] private AudioSource gunShootSource;

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pivot.position, 2f);
    }

    public void Fire(InputAction.CallbackContext context)
    {
        if (!context.started || attacking || swapping)
            return;

        attacking = true;

        if (currentWeapon == Weapon.Gun)
        {
            StartCoroutine(AttackWithGun());
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

    private IEnumerator AttackWithGun()
    {
        playerAnimation.ChangeAnimState(
            playerMovement.movementDirection == Vector2.zero ?
            PlayerAnimStates.shootGun : PlayerAnimStates.shootWalkGun
        );

        while (Input.GetMouseButton(0))
        {
            float animStartTime = Time.time;
            while (Time.time <= animStartTime + playerAnimation.animator.GetCurrentAnimatorStateInfo(0).length)
            {
                // spore clouds actually spawned using animation events
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForEndOfFrame();
        }
        
        attacking = false;
    }

    public void ShootSporeCloud()
    {
        gunShootSource.pitch = 1;
        gunShootSource.pitch += RandInRange(0, 20) / 20.0f;
        gunShootSource.Play();

        GameObject cloudObj = Instantiate(sporeCloudPrefab, gunShootPoint.position, Quaternion.identity);
        SporeCloud cloud = cloudObj.GetComponent<SporeCloud>();

        //Vector3 shootDirection = (cloud.transform.position - pivot.position).normalized;
        cloud.maxScaleMultiplier = 1.5f;
        cloud.cloudScaleSpeed = 1f;
        cloud.initialVelocity = gunCloudSpeed * new Vector3(transform.localScale.x, RandInRange(1, 5) / 10.0f, 0f).normalized;
        cloud.velocityDecay = -gunCloudDecay * new Vector3(transform.localScale.x, RandInRange(1, 5) / 10.0f, 0f).normalized;
    }

    // called as an animation event
    public void SpawnAndShootGrenade()
    {
        grenadeShootSource.pitch = 1;
        float addition = RandInRange(0, 20) / 20.0f;
        grenadeShootSource.pitch += addition;
        grenadeShootSource.Play();

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
