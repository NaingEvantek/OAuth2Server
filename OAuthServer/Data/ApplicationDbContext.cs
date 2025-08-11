// Data/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;

namespace AuthServerNew.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<OpenIddictEntityFrameworkCoreApplication> Applications { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreAuthorization> Authorizations { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreScope> Scopes { get; set; }
        public DbSet<OpenIddictEntityFrameworkCoreToken> Tokens { get; set; }
    }
}
