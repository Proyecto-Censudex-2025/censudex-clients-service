using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClientsService.src.Helper
{
    public class QueryObject
    {
        /// <summary>
        /// Client's name
        /// </summary>
        public string? Name { get; set; } = string.Empty;
        /// <summary>
        /// Client's lastname
        /// </summary>
        public string? Email { get; set; } = string.Empty;
        /// <summary>
        /// Client's state
        /// </summary>
        public bool? isActive { get; set; }
        /// <summary>
        /// Client's username
        /// </summary>
        public string? Username { get; set; } = string.Empty;
    }
}

        