using Shared.Models;

namespace Api.Models
{
    
    public class Post : IModel<int>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }
        public string Summary { get; set; }

        public int FeaturedImageId { get; set; }

        public string OwnerId { get; set; }

        public virtual User Owner { get; set; }


        public virtual Image FeaturedImage { get; set; }

        public virtual ICollection<PostTag> PostTags { get; set; }


        public int GetId()
            => Id;

        public void SetId(int value)
            => Id = value;
    }
}
