using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public record GroupResponseDTO
    {
        

        public int Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public int ImageId { get; init; }

        public ICollection<UserRolesResponseDTO> UserRoles { get; init; }
    }
}
