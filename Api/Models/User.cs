﻿using Microsoft.AspNetCore.Identity;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Models
{
    public class User : IdentityUser, IEntity<string>, IUserInfos
    {
        public string PresentationName { get; set; }
        public ICollection<Image> Images { get; set; }
        public ICollection<Post> Posts { get; set; }
        
        public string GetId()
            => base.Id;

        public void SetId(string value)
            => base.Id = value;

        
    }

    public interface IUserInfos{
        public string? Email {get; set;}
        public string? PhoneNumber {get; set;}
        public string PresentationName { get; set; }
    }
}
