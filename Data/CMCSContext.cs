using Microsoft.EntityFrameworkCore;
using PROG6212_New_POE.Models;
using System.Collections.Generic;

namespace PROG6212_New_POE.Data
{
    public class CMCSContext : DbContext
    {
        public CMCSContext(DbContextOptions<CMCSContext> options) : base(options) { }

        public DbSet<Claim> Claims { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
