using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject SporeBagPrefab;

    private GameObject heldSporeBag = null;
    private Vector2 mouseToWorldGlobal = Vector2.zero;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, mouseToWorldGlobal);
    }

    public void Equip(InputAction.CallbackContext context)
    {
        if (!context.started || heldSporeBag != null)
            return;

        print("Equipped");

        heldSporeBag = Instantiate(SporeBagPrefab, transform.position, Quaternion.identity, transform);
    }

    // TODO: make more sophisticated based on equipped gadget, but just doing grenade throw for now
    public void Fire(InputAction.CallbackContext context)
    {
        if (!context.started || heldSporeBag == null)
            return;

        print("Fired");

        Vector2 mousePos = Input.mousePosition;
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePos);

        heldSporeBag.transform.parent = null;
        heldSporeBag.GetComponent<SporeBag>().Throw(transform.position, mouseWorldPosition);
        heldSporeBag = null;
    }
}
