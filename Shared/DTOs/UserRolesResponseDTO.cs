using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Models
{
    public record UserRolesResponseDTO
    {
        public string UserId { get; init; }
        public string RoleId { get; init; }
        public int GroupId { get; init; }

        public GroupResponseDTO Group { get; init; }
    }

}
