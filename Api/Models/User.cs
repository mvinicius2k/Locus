using Microsoft.AspNetCore.Identity;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Models
{
    public class User : IdentityUser, IModel<string>
    {
        public string PresentationName { get; set; }
        public ICollection<Image> Resources { get; set; }
        public ICollection<Post> Posts { get; set; }

        public string GetId()
            => base.Id;

        public void SetId(string value)
            => base.Id = value;
    }
}
