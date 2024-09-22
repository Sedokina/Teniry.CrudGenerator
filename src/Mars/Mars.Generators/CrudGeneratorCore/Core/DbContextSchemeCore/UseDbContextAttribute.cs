using System;

namespace Mars.Generators.ApplicationGenerators.Core.DbContextCore;

[AttributeUsage(AttributeTargets.Class)]
public class UseDbContextAttribute(DbContextDbProvider provider) : Attribute
{
    public DbContextDbProvider Provider { get; } = provider;
}

public enum DbContextDbProvider
{
    Mongo,
    Postgres
}