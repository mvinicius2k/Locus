namespace Api.Models
{
    public class PostTag : IEntity<(int tagName, int postId)>
    {
        public int TagId { get; set; }

        public int PostId { get; set; }

        

        public virtual Tag Tag { get; set; }

        public virtual Post Post { get; set; }

       

        public (int tagName, int postId) GetId()
            => (TagId, PostId);

        public void SetId((int tagName, int postId) value)
        {
            PostId = value.postId;
            TagId = value.tagName;
        }
            
    }
}
