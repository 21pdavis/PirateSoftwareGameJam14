using UnityEngine.Events;

public interface IInteractable
{
    bool InteractionEnabled { get; set; }
    UnityEvent<EventData> InteractedWith { get; set; }
}
