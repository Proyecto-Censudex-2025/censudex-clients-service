using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientsService.src.Model
{
    public class Client
    {
        /// <summary>
        /// Client's Id
        /// </summary>
        public required string Id { get; set; }
        /// <summary>
        /// Client's role
        /// </summary>
        public required string Role { get; set; }
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
        /// Client's birth date
        /// </summary>
        public DateOnly BirthDate { get; set; }
        /// <summary>
        /// Client's birth date
        /// </summary>
        public required string Address { get; set; }
        /// <summary>
        /// Client's password
        /// </summary>
        public required string TelephoneNumber { get; set; }
        /// <summary>
        /// Client's password
        /// </summary>
        public required string Password { get; set; }
        /// <summary>
        /// Client's state
        /// </summary>
        public bool isActive { get; set; }
        /// <summary>
        /// Client's dates of deactivation
        /// </summary>
        public List<DateOnly> DeactivationDates { get; set; } = new List<DateOnly>();
    }
}