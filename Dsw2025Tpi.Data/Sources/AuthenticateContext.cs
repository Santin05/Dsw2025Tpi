using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dsw2025Tpi.Data.Sources;
public class AuthenticateContext : IdentityDbContext<IdentityUser>
{
    public AuthenticateContext(DbContextOptions<AuthenticateContext> options) : base(options) { }
}

