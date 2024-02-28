using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace TreeTest.Data;

public class TreeContext: DbContext
{
    public DbSet<Node> Nodes { get; set; }

    public TreeContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Database=treedb;Username=postgres;Password=123");
}