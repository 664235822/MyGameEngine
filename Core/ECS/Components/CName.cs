using System.Runtime.Serialization;

namespace Core.ECS.Components;

[DataContract]
public class CName : IComponent
{
    [DataMember] private Guid id;
    public Guid Id => id;

    [DataMember] public string Name { get; set; }

    public CName()
    {
        id = Guid.NewGuid();
        Name = "Empty";
    }

    public CName(string name)
    {
        id = Guid.NewGuid();
        Name = name;
    }
}