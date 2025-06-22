using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Consumer.Repository.Context;

public class EmailContextFactory : IDesignTimeDbContextFactory<EmailContext>
{
    public EmailContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EmailContext>();
        optionsBuilder.UseNpgsql(
            "Host=168.231.96.82;Port=19432;Database=email-db;Username=admin;Password=admin",
            b => b.MigrationsAssembly("Consumer.Email"));

        return new EmailContext(optionsBuilder.Options);
    }
}
