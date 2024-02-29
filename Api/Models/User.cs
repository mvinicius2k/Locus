using Microsoft.AspNetCore.Identity;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Models
{
    public class User : IdentityUser<Guid>, IModel<Guid>
    {
        public string PresentationName { get; set; }
        public ICollection<Resource> Resources { get; set; }
        public ICollection<Post> Posts { get; set; }

        public Guid GetId()
            => base.Id;

        public void SetId(Guid value)
            => base.Id = value;
    }
}
