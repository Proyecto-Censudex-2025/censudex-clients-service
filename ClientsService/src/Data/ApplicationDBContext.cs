using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientsService.src.Model;
using Microsoft.EntityFrameworkCore;

namespace ClientsService.src.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        { }

        public DbSet<Client> clients { get; set; }
    }
}