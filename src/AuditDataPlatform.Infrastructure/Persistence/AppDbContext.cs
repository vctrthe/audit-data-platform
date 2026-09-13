using AuditDataPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuditDataPlatform.Infrastructure.Persistence;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<RuleSet> RuleSets => Set<RuleSet>();
    public DbSet<Rule> Rules => Set<Rule>();
    public DbSet<ImportJob> ImportJobs => Set<ImportJob>();
    public DbSet<ImportRow> ImportRows => Set<ImportRow>();
    public DbSet<Finding> Findings => Set<Finding>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}