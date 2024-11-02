using ITech.CrudGenerator.TestApi;
using ITech.CrudGenerator.TestApi.Generators.SimpleEntityGenerator;
using ITech.CrudGenerator.TestApi.Generators.SimpleTypeEntityGenerator;
using Microsoft.EntityFrameworkCore;

namespace ITech.CrudGenerator.Tests.Endpoints.Core;

public static class DbDataInitializer
{
    public static async Task InitializeAsync(TestMongoDb db)
    {
        await db.AddRangeAsync([
            new SimpleEntity { Id = Guid.NewGuid(), Name = "First Entity Name" },
            new SimpleEntity { Id = Guid.NewGuid(), Name = "Second Entity Name" }
        ]);

        await db.AddRangeAsync([
            new SimpleTypeEntity
            {
                Id = Guid.NewGuid(),
                Name = "First Entity Name",
                Code = 'a',
                IsActive = false,
                RegistrationDate = DateTime.Today.AddDays(-1),
                LastSignInDate = DateTimeOffset.UtcNow.AddDays(-1),
                ByteRating = 1,
                ShortRating = -83,
                IntRating = -19876718,
                LongRating = -971652637891,
                SByteRating = -4,
                UShortRating = 83,
                UIntRating = 19876718,
                ULongRating = 971652637891,
                FloatRating = 18.13f,
                DoubleRating = 91873.862378,
                DecimalRating = 867.97716829m,
                NotIdGuid = new Guid("63c4e04c-77d3-4e27-b490-8f6e4fc635bd"),
            },
            new SimpleTypeEntity
            {
                Id = Guid.NewGuid(),
                Name = "Second Entity Name",
                Code = 'b',
                IsActive = true,
                RegistrationDate = DateTime.Today.AddDays(1),
                LastSignInDate = DateTimeOffset.UtcNow.AddDays(1),
                ByteRating = 2,
                ShortRating = -85,
                IntRating = -20876718,
                LongRating = -983652637891,
                SByteRating = -7,
                UShortRating = 100,
                UIntRating = 20876718,
                ULongRating = 999652637891,
                FloatRating = 20.13f,
                DoubleRating = 99873.862378,
                DecimalRating = 967.97716829m,
                NotIdGuid = new Guid("f6c5e2d1-b438-4faf-8521-b775d783f6f3"),
            }
        ]);
        
        // TODO: decide on Transactional behaviour
        db.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
    }
}