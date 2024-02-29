using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public record TagResponseDTO
    {
        public const int NameMaxLength = 32;

        public string Name { get; init; }

        public ICollection<PostResponseDTO> Posts { get; init; }

    }

}
