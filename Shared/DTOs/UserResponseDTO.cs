using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public record UserResponseDTO
    {
        public string Id { get; init; }
        public string Email { get; init; }
        public string PresentationName { get; init; }
        public string PhoneNumber {get; init;}

        public ICollection<ResourceResponseDTO> Resources { get; init; }
        public ICollection<PostResponseDTO> Posts { get; init; }
        
        
    }

    public record UserRequestDTO(string Email, string PresentationName, string PhoneNumber, string RawPassword);
    public record UserLoginDTO(string Email, string Password, bool RememberMe);
    public record JwtTokenResponseDTO(string token, DateTime expiration);
        
}
