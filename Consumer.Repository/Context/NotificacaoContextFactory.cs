using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Consumer.Repository.Context;

public class NotificacaoContextFactory : IDesignTimeDbContextFactory<NotificacaoContext>
{
    public NotificacaoContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<NotificacaoContext>();
        optionsBuilder.UseNpgsql(
            "Host=168.231.96.82;Port=19432;Database=notificacao-db;Username=admin;Password=admin",
            b => b.MigrationsAssembly("Consumer.Repository"));

        return new NotificacaoContext(optionsBuilder.Options);
    }
}
