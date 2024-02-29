using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace Api.Models;

public class UserRole : IdentityUserRole<string>, IModel<(string userId, string roleId, int groupId)>
{
    public int GroupId { get; set; }

    public virtual Group Group { get; set; }

    public (string userId, string roleId, int groupId) GetId()
        => (UserId, RoleId, GroupId);

    public void SetId((string userId, string roleId, int groupId) value)
    {
        UserId = value.userId;
        RoleId = value.roleId;
        GroupId = value.groupId;
    }
        
}
