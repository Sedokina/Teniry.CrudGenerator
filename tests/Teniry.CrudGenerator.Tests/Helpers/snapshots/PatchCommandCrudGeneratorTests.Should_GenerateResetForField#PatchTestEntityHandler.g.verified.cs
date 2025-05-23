﻿//HintName: PatchTestEntityHandler.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a crud generator tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using Teniry.Cqrs.Commands;
using Teniry.Cqrs.Extended.Exceptions;
using Teniry.Cqrs.Extended.Types.PatchOperationType;
using Teniry.CrudGenerator.Tests;
using Teniry.CrudGenerator.Tests;
using Mapster;

namespace Teniry.CrudGenerator.Tests.Application.TestEntityFeature.PatchTestEntity;
public partial class PatchTestEntityHandler : ICommandHandler<PatchTestEntityCommand>
{
    private readonly TestDb _db;
    public PatchTestEntityHandler(TestDb db)
    {
        _db = db;
    }

    /// <inheritdoc/>
    public async Task HandleAsync(PatchTestEntityCommand command, CancellationToken cancellation)
    {
        var entity = await _db.FindAsync<TestEntity>(new object[] { command.Id }, cancellation);
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TestEntity));
        }

        PatchOp.Handle(command.ResetableName, nameof(command.ResetableName), x =>
        {
            entity.ResetableName = x;
        }, x =>
        {
            entity.ResetableName = default;
        });
        await _db.SaveChangesAsync(cancellation);
    }
}