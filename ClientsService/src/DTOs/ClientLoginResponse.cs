using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ClientsService.src.DTOs
{
    public class ClientLoginResponse
    {
        /// <summary>
        /// Client's state
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// Client's id
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Client's email
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Client's name
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Client's surename
        /// </summary>
        public string? Surename { get; set; }
        /// <summary>
        /// Client's username
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// Client's date of birth
        /// </summary>
        public DateOnly? Birthdate { get; set; }
        /// <summary>
        /// Client's address
        /// </summary>
        public string? Address { get; set; }
        /// <summary>
        /// Client's telephone numeber
        /// </summary>
        public string? TelephoneNumber { get; set; }
        /// <summary>
        /// Client's role (Admin, User)
        /// </summary>
        public string? Role { get; set; }
        /// <summary>
        /// Client's list of claims
        /// </summary>
        public List<Claim> Claims { get; set; } = new();
        /// <summary>
        /// error in response
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}