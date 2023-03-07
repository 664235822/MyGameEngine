namespace Core.ECS.Components;

public class CId:IComponent
{
    public Guid Id { get; }

    public CId()
    {
        Id = Guid.NewGuid();
    }
}