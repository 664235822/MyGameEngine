namespace Core.ECS.Components;

public class IdComponent:IComponent
{
    public Guid Id { get; }

    public IdComponent(Guid id)
    {
        Id = Guid.NewGuid();
    }
}