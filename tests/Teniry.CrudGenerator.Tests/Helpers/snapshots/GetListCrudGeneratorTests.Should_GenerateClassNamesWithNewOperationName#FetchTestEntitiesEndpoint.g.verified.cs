﻿//HintName: FetchTestEntitiesEndpoint.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a crud generator tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using Microsoft.AspNetCore.Mvc;
using Teniry.Cqrs.Queries;
using Teniry.CrudGenerator.Tests.Application.TestEntityFeature.FetchTestEntities;

namespace Teniry.CrudGenerator.Tests.Endpoints.TestEntityEndpoints;
public static partial class FetchTestEntitiesEndpoint
{
    /// <summary>
    ///     Get Test entities
    /// </summary>
    /// <response code="200">Returns Test entity list</response>
    [ProducesResponseType(typeof(TestEntitiesDto), 200)]
    public static async Task<IResult> FetchAsync([AsParameters] FetchTestEntitiesQuery query, IQueryDispatcher queryDispatcher, CancellationToken cancellation)
    {
        var result = await queryDispatcher.DispatchAsync<FetchTestEntitiesQuery, TestEntitiesDto>(query, cancellation);
        return TypedResults.Ok(result);
    }
}