using System;

namespace ITech.CrudGenerator.CrudGeneratorCore.Schemes.DbContext;

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