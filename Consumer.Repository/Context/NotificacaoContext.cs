using Consumer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Repository.Context;

public class NotificacaoContext : DbContext
{
    public NotificacaoContext(DbContextOptions<NotificacaoContext> options) : base(options) { }

    public DbSet<Template> Templates { get; set; }
}