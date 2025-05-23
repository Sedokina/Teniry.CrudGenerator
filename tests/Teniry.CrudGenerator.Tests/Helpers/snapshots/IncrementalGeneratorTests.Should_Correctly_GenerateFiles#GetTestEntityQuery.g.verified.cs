﻿//HintName: GetTestEntityQuery.g.cs
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a crud generator tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using Teniry.Cqrs.Extended.Exceptions;

namespace Teniry.CrudGenerator.Tests.Application.TestEntityFeature.GetTestEntity;
/// <summary>
///     Get Test entity by id
/// </summary>
/// <returns>Returns full entity data of type <see cref = "TestEntityDto"/></returns>
/// <exception cref = "EntityNotFoundException">When Test entity entity does not exist</exception>
public partial class GetTestEntityQuery
{
    public int Id { get; set; }

    public GetTestEntityQuery(int id)
    {
        Id = id;
    }
}