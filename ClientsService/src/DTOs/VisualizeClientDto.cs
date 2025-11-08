using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientsService.src.DTOs
{
    public class VisualizeClientDto
    {
        /// <summary>
        /// Client's Id
        /// </summary>
        public required string Id { get; set; }
         /// <summary>
        /// Client's fullname (name + surename)
        /// </summary>
        public required string Fullname { get; set; }
        /// <summary>
        /// Client's email
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// Client's username
        /// </summary>
        public required string Username { get; set; }
        /// <summary>
        /// Client's state
        /// </summary>
        public bool isActive { get; set; }
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
        /// Client's registration date
        /// </summary>
        public DateOnly RegistrationDate { get; set; }
    }
}