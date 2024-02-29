namespace Api.Models
{
    public class PostTag : IModel<(string tagName, int postId)>
    {
        public string TagName { get; set; }

        public int PostId { get; set; }

        

        public virtual Tag Tag { get; set; }

        public virtual Post Post { get; set; }

       

        public (string tagName, int postId) GetId()
            => (TagName, PostId);

        public void SetId((string tagName, int postId) value)
        {
            PostId = value.postId;
            TagName = value.tagName;
        }
            
    }
}
