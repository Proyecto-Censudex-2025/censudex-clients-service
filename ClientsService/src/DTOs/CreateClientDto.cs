using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientsService.src.DTOs
{
    public class CreateClientDto
    {
        /// <summary>
        /// Client's name
        /// </summary>
        public required string Name { get; set; }
        /// <summary>
        /// Client's lastname
        /// </summary>
        public required string Surename { get; set; }
        /// <summary>
        /// Client's email
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// Client's username
        /// </summary>
        public required string Username { get; set; }
        /// <summary>
        /// Client's date of birth
        /// </summary>
        public required DateOnly Birthdate { get; set; }
        /// <summary>
        /// Client's address
        /// </summary>
        public required string Address { get; set; }
        /// <summary>
        /// Client's telephone number
        /// </summary>
        public required string TelephoneNumber { get; set; }
        /// <summary>
        /// Client's password
        /// </summary>
        public required string Password { get; set; }
    }
}