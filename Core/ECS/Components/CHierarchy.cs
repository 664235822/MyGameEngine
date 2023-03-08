namespace Core.ECS.Components;

public class CHierarchy : IComponent
{
    public Guid Id { get; }

    public Guid? ParentId { get; internal set; }

    public List<Guid> Children { get; }

    public CHierarchy()
    {
        Id = Guid.NewGuid();
        ParentId = null;
        Children = new List<Guid>();
    }
}