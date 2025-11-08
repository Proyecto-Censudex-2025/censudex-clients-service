using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientsService.src.DTOs
{
    public class LoginDto
    {
        /// <summary>
        /// Client's email
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Client's username
        /// </summary>
        public string? Username { get; set; }
        /// <summary>
        /// Client's passsword
        /// </summary>
        public required string Password { get; set; }
    }
}
