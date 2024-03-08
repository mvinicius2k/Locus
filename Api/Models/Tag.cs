using Shared.Models;

namespace Api.Models
{
    public class Tag : IEntity<string>
    {
        
        public string Name { get; set; }

        public virtual ICollection<PostTag> PostTags { get; set; }

        public string GetId()
            => Name;

        public void SetId(string value)
            => Name = value;
    }
}
