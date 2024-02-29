using Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Models
{
    public class Image : IModel<int>
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string OwnerId { get; set; }
        public DateTime UploadedAt { get; set; }

        public virtual User Owner { get; set; }
        public virtual ICollection<Post> UsedByPosts { get; set; }
        public virtual ICollection<Group> UserByGroups { get; set; }
        
       

        public int GetId()
            => Id;

        public void SetId(int value)
            => Id = value;
    }
}
