using Consumer.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Consumer.Repository.Context;

public class EmailContext : DbContext
{
    public EmailContext(DbContextOptions<EmailContext> options) : base(options) { }

    public DbSet<EmailTemplate> EmailTemplates { get; set; }
}