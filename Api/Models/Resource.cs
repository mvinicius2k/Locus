using Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Models
{
    public class Resource : IModel<int>
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public Guid OwnerId { get; set; }
        public DateTime UploadedAt { get; set; }

        public User Owner { get; set; }
        public ICollection<Post> UsedByPosts { get; set; }
       

        public int GetId()
            => Id;

        public void SetId(int value)
            => Id = value;
    }
}
