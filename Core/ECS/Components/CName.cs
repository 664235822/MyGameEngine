namespace Core.ECS.Components;

public class CName : IComponent
{
    public Guid Id { get; }

    public string Name { get; set; }

    public CName()
    {
        Id = Guid.NewGuid();
        Name = "Empty";
    }
    
    public CName(string name)
    {
        Id = Guid.NewGuid();
        Name = name;
    }
}