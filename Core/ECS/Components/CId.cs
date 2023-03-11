using System.Runtime.Serialization;

namespace Core.ECS.Components;

[DataContract]
public class CId : IComponent
{
    [DataMember] private Guid id;
    public Guid Id => id;

    public CId()
    {
        id = Guid.NewGuid();
    }
}