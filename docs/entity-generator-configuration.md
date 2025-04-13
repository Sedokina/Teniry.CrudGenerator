# EntityGeneratorConfiguration

EntityGeneratorConfiguration is a class that holds the configuration for the CRUD Generator. It defines which class
should have CRUD operations generated and allows to customize the generated code.

Based on configuration, the generator will generate endpoints and handlers for:

- Create operation
- Get list operation
- Get by id operation
- Update operation
- Delete operation

## Entity

CRUD generator can generate CRUD operations for any provided class.

> [!WARNING]
> The class should have a public constructor with no parameters.

Currently CRUD generator supports mapping only to the classes with default constructor.

## Configuration

To generate CRUD operations for a class, you have to create declare a class that inherits
from `EntityGeneratorConfiguration`.

On build, the generator will look for all classes that inherit from `EntityGeneratorConfiguration` and generate CRUD
operations for the classes from configuration.

## Example: Minimal configuration

### Create a class

```csharp
public class Todo {
    public Guid Id { get; set; }
    public string Description { get; set; }
    public bool IsDone { get; set; }
}
```

### Create a configuration

```csharp
public class TodoConfiguration : EntityGeneratorConfiguration<Todo> { }
```

With this configuration, the CRUD generator will generate all CRUD operations for the `Todo` class.

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

### Simple customization example:

Customize `Operation` so that all generated classes would have `Save` as a name of the operation instead of
`Create`.

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        CreateOperation = new() {
            Operation = "Save"
        };
    }
}
```

### Full customization example:

Customize all properties of the operation so that all generated classes would have custom names.

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

Note: When you override values of all properties you do not need to override `Operation` property, because it is only
used to generate names of the classes defined by other properties. But it wouldn't be a mistake to override it as well,
because it just will be ignored.

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

### Simple customization example:

Customize `Operation` so that all generated classes would have `All` as a name of the operation instead of
`Get`.

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        GetListOperation = new() {
            Operation = "All"
        };
    }
}
```

### Full customization example:

Customize all properties of the operation so that all generated classes would have custom names.

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

Note: When you override values of all properties you do not need to override `Operation` property, because it is only
used to generate names of the classes defined by other properties. But it wouldn't be a mistake to override it as well,
because it just will be ignored.

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

### Simple customization example:

Customize `Operation` so that all generated classes would have `GetById` as a name of the operation instead of
`Get`.

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        GetByIdOperation = new() {
            Operation = "GetById"
        };
    }
}
```

### Full customization example:

Customize all properties of the operation so that all generated classes would have custom names.

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

Note: When you override values of all properties you do not need to override `Operation` property, because it is only
used to generate names of the classes defined by other properties. But it wouldn't be a mistake to override it as well,
because it just will be ignored.

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

### Simple customization example:

Customize `Operation` so that all generated classes would have `Change` as a name of the operation instead of
`Update`.

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        UpdateOperation = new() {
            Operation = "Change"
        };
    }
}
```

### Full customization example:

Customize all properties of the operation so that all generated classes would have custom names.

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

Note: When you override values of all properties you do not need to override `Operation` property, because it is only
used to generate names of the classes defined by other properties. But it wouldn't be a mistake to override it as well,
because it just will be ignored.

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

### Simple customization example:

Customize `Operation` so that all generated classes would have `Remove` as a name of the operation instead of
`Delete`.

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        DeleteOperation = new() {
            Operation = "Remove"
        };
    }
}
```

### Full customization example:

Customize all properties of the operation so that all generated classes would have custom names.

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

Note: When you override values of all properties you do not need to override `Operation` property, because it is only
used to generate names of the classes defined by other properties. But it wouldn't be a mistake to override it as well,
because it just will be ignored.

## Entity level customization

You can customize the generated code for the whole entity and not for particular operation.

Property of `EntityGeneratorConfiguration`

| Property Name | Type                       | Default value                                      | Description                                                                     |
|---------------|----------------------------|----------------------------------------------------|---------------------------------------------------------------------------------|
| `Title`       | string                     | Entity name (words separated by spaces)            | used in descriptions of the generated api                                       |
| `PluralTitle` | string                     | Pluralized entity name (words separated by spaces) | used in descriptions of the generated api when pluralized entity names are used |
| `DefaultSort` | EntityGeneratorDefaultSort | None                                               | default sort applied to result when querying `Get list operation`               |

# Configuration's keywords

Keyword - is a special string that will be replaced with the value while generating code.

Available keys for use in configuration of **each operation**:

| Keyword                  | Replaced with                                             |
|--------------------------|-----------------------------------------------------------|
| `{{entity_name}}`        | name of the entity class                                  |
| `{{entity_name_plural}}` | pluralized name of the entity class                       |
| `{{operation_name}}`     | the `Operation` property of the operation's configuration |
| `{{id_param_name}}`      | id properties of the entity                               |

*be sure to use this params with double curly braces in your configurations*

## Keywords usage example:

```csharp
public class ExampleGeneratorConfiguration : EntityGeneratorConfiguration<Example> {
    public ExampleGeneratorConfiguration() {
        DeleteOperation = new() {
            RouteName = "/myRoute/{{entity_name}}/{{id_param_name}}"
        };
    }
}
````