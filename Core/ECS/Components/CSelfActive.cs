namespace Core.ECS.Components;

public class CSelfActive : IComponent
{
    public Guid Id { get; }
    
    public bool IsSelfActive { get; set; }

    public CSelfActive()
    {
        Id = Guid.NewGuid();
        IsSelfActive = true;
    }
    
    public CSelfActive(bool isSelfActive)
    {
        Id = Guid.NewGuid();
        IsSelfActive = isSelfActive;
    }
}