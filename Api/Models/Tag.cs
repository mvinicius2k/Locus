using Shared.Models;

namespace Api.Models
{
    public class Tag : IEntity<int>
    {
        public int Id {get; set;}
        public string Name { get; set; }

        public virtual ICollection<PostTag> PostTags { get; set; }

        public int GetId()
            => Id;

        public void SetId(int value)
            => Id = value;
    }
}
