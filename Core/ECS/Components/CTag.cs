namespace Core.ECS.Components;

public class CTag : IComponent
{
    public Guid Id { get; }

    public string Tag { get; set; }

    public CTag()
    {
        Id = Guid.NewGuid();
        Tag = "";
    }

    public CTag(string tag)
    {
        Id = Guid.NewGuid();
        Tag = tag;
    }

    public bool CompareTag(string tag) => Tag.Equals(tag);
}