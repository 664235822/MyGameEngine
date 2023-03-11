using System.Runtime.Serialization;

namespace Core.ECS.Components;

[DataContract]
public class CHierarchy : IComponent
{
    [DataMember] private Guid id;
    [DataMember] private Guid? parentId;
    [DataMember] private List<Guid> children;

    public Guid Id => id;

    public Guid? ParentId => parentId;

    public List<Guid> Children => children;

    public CHierarchy()
    {
        id = Guid.NewGuid();
        parentId = null;
        children = new List<Guid>();
    }
}