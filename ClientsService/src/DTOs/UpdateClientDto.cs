using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientsService.src.DTOs
{
    public class UpdateClientDto
    {
        /// <summary>
        /// Client's name
        /// </summary>
        public string? Name { get; set; } = string.Empty;
        /// <summary>
        /// Client's lastname
        /// </summary>
        public string? Surename { get; set; } = string.Empty;
        /// <summary>
        /// Client's email
        /// </summary>
        public string? Email { get; set; } = string.Empty;
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
        /// Client's telephone number
        /// </summary>
        public string? TelephoneNumber { get; set; }
        /// <summary>
        /// Client's registration date
        /// </summary>
        /// <summary>
        /// Client's password
        /// </summary>
        public string? Password { get; set; } = string.Empty;

    }
}