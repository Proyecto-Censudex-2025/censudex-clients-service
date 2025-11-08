using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClientsService.src.DTOs
{
    public class ClientLoginResponse
    {
        public bool IsValid { get; set; }
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Surename { get; set; }
        public string? Username { get; set; }
        public DateOnly? Birthdate { get; set; }
        public string? Address { get; set; }
        public string? TelephoneNumber { get; set; }

        public string? Role { get; set; }
        public List<Claim> Claims { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }
}