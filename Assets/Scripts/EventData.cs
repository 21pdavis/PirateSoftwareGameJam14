using UnityEngine;

public class EventData
{
    /// <value>
    /// The Vector2 position from which the event fired.
    /// </value>
    public Vector2 Position { get; set; }

    public EventData(Vector2 position)
    {
        Position = position;
    }
}
