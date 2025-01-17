using System;

namespace Teniry.CrudGenerator.Abstractions.DbContext;

[AttributeUsage(AttributeTargets.Class)]
public class UseDbContextAttribute(DbContextDbProvider provider) : Attribute {
    public DbContextDbProvider Provider { get; } = provider;
}