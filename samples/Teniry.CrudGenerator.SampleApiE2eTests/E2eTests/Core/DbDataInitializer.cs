using Teniry.CrudGenerator.SampleApi;
using Microsoft.EntityFrameworkCore;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.CustomGottenEntityGenerator;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.SimpleEntityGenerator;
using Teniry.CrudGenerator.SampleApi.CrudConfigurations.SimpleTypeEntityGenerator;

namespace Teniry.CrudGenerator.SampleApiE2eTests.E2eTests.Core;

public static class DbDataInitializer {
    public static async Task InitializeAsync(SampleMongoDb db) {
        await db.AddRangeAsync(
            new SimpleEntity { Id = Guid.NewGuid(), Name = "First Entity Name" },
            new SimpleEntity { Id = Guid.NewGuid(), Name = "Second Entity Name" }
        );

        await db.AddRangeAsync(
            new SimpleTypeEntity {
                Id = new("44bacea2-1e32-452a-b1f3-28e46924e899"),
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
                NotIdGuid = new("63c4e04c-77d3-4e27-b490-8f6e4fc635bd")
            },
            new SimpleTypeEntity {
                Id = new("36b23b6e-ef84-481f-892a-2fc3ad9c6921"),
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
                NotIdGuid = new("f6c5e2d1-b438-4faf-8521-b775d783f6f3")
            }
        );

        await db.AddRangeAsync(
            new ReadOnlyCustomizedEntity {
                Id = new("27ed3a08-c92e-4c8d-b515-f793eb65cacd"),
                Name = "First Entity Name"
            },
            new ReadOnlyCustomizedEntity { Id = Guid.NewGuid(), Name = "Second Entity Name" }
        );

        // TODO: decide on Transactional behaviour
        db.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;

        await db.SaveChangesAsync();
        db.ChangeTracker.Clear();
    }
}