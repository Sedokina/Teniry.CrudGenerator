# Customization

Using configuration class you can customize the generated code.

Each operation has own properties that can be customized.

Customization is done by creating a class that inherits from `EntityGeneratorConfiguration<TEntity>` where `TEntity` is
your entity class. Customization itself is done by defining properties of the operation you want to customize in the
constructor of the configuration class.

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        // customize the operations here
    }
}

public class Example {
    public Guid Id { get; set; }
    public string Name { get; set; }
}
``` 

## Create operation

Generates code to create an entity and save it to the database.

Property of `EntityGeneratorConfiguration`: `CreateOperation` of type `EntityGeneratorCreateOperationConfiguration`

| Property Name          | Type   | Default value              | Description                                                                    |
|------------------------|--------|----------------------------|--------------------------------------------------------------------------------|
| `Generate`             | bool   | true                       | defines if the operation should be generated at all or not                     |
| `Operation`            | string | Create                     | name of the operation                                                          |
| `OperationGroup`       | string | Create{EntityName}         | namespace where all related classes for operation handling will be placed in   |
| `CommandName`          | string | Create{EntityName}Command  | name of the CQRS command class                                                 |
| `DtoName`              | string | Created{EntityName}Dto     | name of the DTO class that will be returned as a result of operation execution |
| `HandlerName`          | string | Create{EntityName}Handler  | name of the CQRS command handler class                                         |
| `GenerateEndpoint`     | bool   | true                       | defines if the endpoint should be generated at all or not                      |
| `EndpointClassName`    | string | Create{EntityName}Endpoint | name of the class that will contain the endpoint function                      |
| `EndpointFunctionName` | string | CreateAsync                | name of the function that will be called when the endpoint been hit            |
| `RouteName`            | string | /{EntityName}/create       | name of the route that will be used to call the endpoint                       |

Note:

* `Create` text is taken from the `Operation`, and would be replaced if the other `Operation` value provided
* `{EntityName}` will be replaced with the name of the entity class

Customization example:

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        CreateOperation = new() {
            OperationGroup = "ManagedEntityCreateOperationCustomNs",
            CommandName = "CustomizedNameCreateManagedEntityCommand",
            HandlerName = "CustomizedNameCreateManagedEntityHandler",
            DtoName = "CustomizedNameCreatedManagedEntityDto",
            EndpointClassName = "CustomizedNameCreateManagedEntityEndpoint",
            EndpointFunctionName = "RunCreateAsync",
            RouteName = "/customizedManagedEntityCreate"
        };
    }
}
```

## Get list operation

Generates code to get a list of entities from the database.

Property of `EntityGeneratorConfiguration`: `GetListOperation` of type `EntityGeneratorGetListOperationConfiguration`

| Property Name          | Type   | Default value                 | Description                                                                          |
|------------------------|--------|-------------------------------|--------------------------------------------------------------------------------------|
| `Generate`             | bool   | true                          | defines if the operation should be generated at all or not                           |
| `Operation`            | string | Get                           | name of the operation                                                                |
| `OperationGroup`       | string | Get{PluralEntityName}         | namespace where all related classes for operation handling will be placed in         |
| `QueryName`            | string | Get{PluralEntityName}Query    | name of the CQRS query class                                                         |
| `DtoName`              | string | {PluralEntityName}Dto         | name of the DTO class that will be returned as a result of query's handler execution |
| `ListItemDtoName`      | string | {EntityName}ListItemDto       | name of the DTO class that is a part of `DtoName` and stores entity's properties     |
| `FilterName`           | string | Get{PluralEntityName}Filter   | name of the filter class that will be used to filter the list of entities            |
| `HandlerName`          | string | Get{PluralEntityName}Handler  | name of the CQRS query handler class                                                 |
| `GenerateEndpoint`     | bool   | true                          | defines if the endpoint should be generated at all or not                            |
| `EndpointClassName`    | string | Get{PluralEntityName}Endpoint | name of the class that will contain the endpoint function                            |
| `EndpointFunctionName` | string | GetAsync                      | name of the function that will be called when the endpoint is hit                    |
| `RouteName`            | string | /{EntityName}                 | name of the route that will be used to call the endpoint                             |

Note:

* `Get` text is taken from the `Operation`, and would be replaced if the other `Operation` value provided
* `{PluralEntityName}` will be replaced with the pluralized name of the entity class
* `{EntityName}` will be replaced with the name of the entity class

Customization example:

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        GetListOperation = new() {
            OperationGroup = "GetReadOnlyModelsListCustomNamespace",
            QueryName = "GetReadOnlyModelsQuery",
            HandlerName = "GetReadOnlyModelsHandler",
            DtoName = "ReadOnlyModelsListCustomDto",
            EndpointClassName = "ReadOnlyModelsListCustomizedEndpoint",
            EndpointFunctionName = "RunGetListAsync",
            RouteName = "/getAllCustomizedReadOnlyEntities"
        };
    }
}
```

## Get by id operation

Generates code to get an entity by id from the database.

Property of `EntityGeneratorConfiguration`: `GetByIdOperation` of type `EntityGeneratorGetByIdOperationConfiguration`

| Property Name          | Type   | Default value            | Description                                                                          |
|------------------------|--------|--------------------------|--------------------------------------------------------------------------------------|
| `Generate`             | bool   | true                     | defines if the operation should be generated at all or not                           |
| `Operation`            | string | Get                      | name of the operation                                                                |
| `OperationGroup`       | string | Get{EntityName}          | namespace where all related classes for operation handling will be placed in         |
| `QueryName`            | string | Get{EntityName}Query     | name of the CQRS query class                                                         |
| `DtoName`              | string | {EntityName}Dto          | name of the DTO class that will be returned as a result of query's handler execution |
| `HandlerName`          | string | Get{EntityName}Handler   | name of the CQRS query handler class                                                 |
| `GenerateEndpoint`     | bool   | true                     | defines if the endpoint should be generated at all or not                            |
| `EndpointClassName`    | string | Get{EntityName}Endpoint  | name of the class that will contain the endpoint function                            |
| `EndpointFunctionName` | string | GetAsync                 | name of the function that will be called when the endpoint is hit                    |
| `RouteName`            | string | /{EntityName}/{EntityId} | name of the route that will be used to call the endpoint                             |

Note:

* `Get` text is taken from the `Operation`, and would be replaced if the other `Operation` value provided
* `{EntityName}` will be replaced with the name of the entity class

Customization example:

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        GetByIdOperation = new() {
            OperationGroup = "GetReadOnlyModelCustomNamespace",
            QueryName = "GetReadOnlyModelQuery",
            HandlerName = "GetReadOnlyModelHandler",
            DtoName = "ReadOnlyModelCustomDto",
            EndpointClassName = "GetReadOnlyModelCustomizedEndpoint",
            EndpointFunctionName = "RunGetAsync",
            RouteName = "/getCustomizedReadOnlyModelById/{{id_param_name}}"
        };
    }
}

```

## Update operation

Generates code to update an entity in the database.

Property of `EntityGeneratorConfiguration`: `UpdateOperation` of type `EntityGeneratorUpdateOperationConfiguration`

| Property Name          | Type   | Default value                   | Description                                                                                                |
|------------------------|--------|---------------------------------|------------------------------------------------------------------------------------------------------------|
| `Generate`             | bool   | true                            | defines if the operation should be generated at all or not                                                 |
| `Operation`            | string | Update                          | name of the operation                                                                                      |
| `OperationGroup`       | string | Update{EntityName}              | namespace where all related classes for operation handling will be placed in                               |
| `CommandName`          | string | Update{EntityName}Command       | name of the CQRS command class                                                                             |
| `HandlerName`          | string | Update{EntityName}Handler       | name of the CQRS command handler class                                                                     |
| `GenerateEndpoint`     | bool   | true                            | defines if the endpoint should be generated at all or not                                                  |
| `EndpointClassName`    | string | Update{EntityName}Endpoint      | name of the class that will contain the endpoint function                                                  |
| `EndpointFunctionName` | string | UpdateAsync                     | name of the function that will be called when the endpoint is hit                                          |
| `RouteName`            | string | /{EntityName}/{EntityId}/update | name of the route that will be used to call the endpoint                                                   |
| `ViewModelName`        | string | Update{EntityName}Vm            | name of the view model class that is accepted in the body of the endpoint and is used to update the entity |

Note:

* `Update` text is taken from the `Operation`, and would be replaced if the other `Operation` value provided
* `{EntityName}` will be replaced with the name of the entity class

Customization example:

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        UpdateOperation = new() {
            OperationGroup = "ManagedEntityUpdateOperationCustomNs",
            CommandName = "CustomizedNameUpdateManagedEntityCommand",
            HandlerName = "CustomizedNameUpdateManagedEntityHandler",
            ViewModelName = "CustomizedNameUpdateManagedEntityViewModel",
            EndpointClassName = "CustomizedNameUpdateManagedEntityEndpoint",
            EndpointFunctionName = "RunUpdateAsync",
            RouteName = "/customizedManagedEntityUpdate/{{id_param_name}}"
        };
    }
}
```

## Delete operation

Generates code to delete an entity from the database.

Property of `EntityGeneratorConfiguration`: `DeleteOperation` of type `EntityGeneratorDeleteOperationConfiguration`

| Property Name          | Type   | Default value                   | Description                                                                  |
|------------------------|--------|---------------------------------|------------------------------------------------------------------------------|
| `Generate`             | bool   | true                            | defines if the operation should be generated at all or not                   |
| `Operation`            | string | Delete                          | name of the operation                                                        |
| `OperationGroup`       | string | Delete{EntityName}              | namespace where all related classes for operation handling will be placed in |
| `CommandName`          | string | Delete{EntityName}Command       | name of the CQRS command class                                               |
| `HandlerName`          | string | Delete{EntityName}Handler       | name of the CQRS command handler class                                       |
| `GenerateEndpoint`     | bool   | true                            | defines if the endpoint should be generated at all or not                    |
| `EndpointClassName`    | string | Delete{EntityName}Endpoint      | name of the class that will contain the endpoint function                    |
| `EndpointFunctionName` | string | DeleteAsync                     | name of the function that will be called when the endpoint is hit            |
| `RouteName`            | string | /{EntityName}/{EntityId}/delete | name of the route that will be used to call the endpoint                     |

Note:

* `Delete` text is taken from the `Operation`, and would be replaced if the other `Operation` value provided
* `{EntityName}` will be replaced with the name of the entity class

Customization example:

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        DeleteOperation = new() {
            OperationGroup = "ManagedEntityDeleteOperationCustomNs",
            CommandName = "CustomizedNameDeleteManagedEntityCommand",
            HandlerName = "CustomizedNameDeleteManagedEntityHandler",
            EndpointClassName = "CustomizedNameDeleteManagedEntityEndpoint",
            EndpointFunctionName = "RunDeleteAsync",
            RouteName = "/customizedManagedEntityDelete/{{entity_name}}/{{id_param_name}}"
        };
    }
}
```