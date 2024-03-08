namespace Api.Models;

public class Group : IEntity<int>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int ImageId { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; }
    public virtual Image Image { get; set; }


    public int GetId()
        => Id;

    public void SetId(int value)
        => Id = value;
}
