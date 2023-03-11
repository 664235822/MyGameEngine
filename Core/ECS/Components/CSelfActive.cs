using System.Runtime.Serialization;

namespace Core.ECS.Components;

[DataContract]
public class CSelfActive : IComponent
{
    [DataMember] private Guid id;
    public Guid Id => id;

    [DataMember] public bool IsSelfActive { get; set; }

    public CSelfActive()
    {
        id = Guid.NewGuid();
        IsSelfActive = true;
    }

    public CSelfActive(bool isSelfActive)
    {
        id = Guid.NewGuid();
        IsSelfActive = isSelfActive;
    }
}