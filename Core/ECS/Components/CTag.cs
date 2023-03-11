using System.Runtime.Serialization;

namespace Core.ECS.Components;

[DataContract]
public class CTag : IComponent
{
    [DataMember] private Guid id;

    public Guid Id => id;

    [DataMember] public string Tag { get; set; }

    public CTag()
    {
        id = Guid.NewGuid();
        Tag = "";
    }

    public CTag(string tag)
    {
        id = Guid.NewGuid();
        Tag = tag;
    }

    public bool CompareTag(string tag) => Tag.Equals(tag);
}