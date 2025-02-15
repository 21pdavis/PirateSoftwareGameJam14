using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour, IInteractable
{
    [SerializeField] private bool interactionEnabled;

    public bool InteractionEnabled {
        get => interactionEnabled;
        set => interactionEnabled = value;
    }
    public UnityEvent<EventData> InteractedWith { get; set; } = new();

    private void Start()
    {
        InteractedWith.AddListener(GameObject.FindGameObjectWithTag("Grid").GetComponent<MushroomSpreadTracker>().StartMushroomSpread);
    }
}
