using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models;

namespace Api.Models;

public class UserRoles : IdentityUserRole<Guid>, IModel<(Guid userId, Guid roleId, int groupId)>
{
    public int GroupId { get; set; }

    public virtual Group Group { get; set; }

    public (Guid userId, Guid roleId, int groupId) GetId()
        => (UserId, RoleId, GroupId);

    public void SetId((Guid userId, Guid roleId, int groupId) value)
    {
        UserId = value.userId;
        RoleId = value.roleId;
        GroupId = value.groupId;
    }
        
}
