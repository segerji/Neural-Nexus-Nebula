namespace NNN.Entities.Events;

public class ObjectDestroyedEvent
{
    public ObjectDestroyedEvent(IEntity destroyedObject)
    {
        DestroyedObject = destroyedObject;
    }

    public IEntity DestroyedObject { get; set; }
}