# Customization

Using configuration class you can customize the generated code.

Each operation has own properties that can be customized.

## Create operation

- **Generate** - defines if the operation should be generated at all or not, by default it is `true`
- **Operation** - name of the operation, by default it is `Create`
- **OperationGroup** - namespace where all related classes for operation handling will be placed in
- **CommandName** - name of the CQRS command class, by default it is `Create{EntityName}Command`
- **DtoName** - name of the DTO class that will be returned as a result of operation execution, by default it is
  `Created{EntityName}Dto`
- **HandlerName** - name of the CQRS command handler class, by default it is `Create{EntityName}Handler`
- **GenerateEndpoint** - defines if the endpoint should be generated at all or not, by default it is `true`
- **EndpointClassName** - name of the class that will contain the endpoint function, by default it is
  `Create{EntityName}Endpoint`
- **EndpointFunctionName** - name of the function that will be called when the endpoint is hit, by default it is
  `CreateAsync`
- **RouteName** - name of the route that will be used to call the endpoint, by default it is `/{EntityName}/create`

Note:

* `Create` text is taken from the `Operation`, and would be replaced if the other `Operation` value provided
* `{EntityName}` will be replaced with the name of the entity class

Customization example:

```csharp
CreateOperation = new() {
  OperationGroup = "ManagedEntityCreateOperationCustomNs",
  CommandName = "CustomizedNameCreateManagedEntityCommand",
  HandlerName = "CustomizedNameCreateManagedEntityHandler",
  DtoName = "CustomizedNameCreatedManagedEntityDto",
  EndpointClassName = "CustomizedNameCreateManagedEntityEndpoint",
  EndpointFunctionName = "RunCreateAsync",
  RouteName = "/customizedManagedEntityCreate"
};
```

## Get list operation

- **Generate** - defines if the operation should be generated at all or not, by default it is `true`
- **Operation** - name of the operation, by default it is `Get`
- **OperationGroup** - namespace where all related classes for operation handling will be placed in
- **QueryName** - name of the CQRS query class, by default it is `Get{PluralEntityName}Query`
- **DtoName** - name of the DTO class that will be returned as a result of operation execution, by default it is
  `{PluralEntityName}Dto`
- **ListItemDtoName** - name of the DTO class that is a part of **DtoName** and stores entity's data, by default it is
  `{EntityName}ListItemDto`
- **FilterName** - name of the filter class that will be used to filter the list of entities, by default it is
  `Get{PluralEntityName}Filter`
- **HandlerName** - name of the CQRS query handler class, by default it is `Get{PluralEntityName}Handler`
- **GenerateEndpoint** - defines if the endpoint should be generated at all or not, by default it is `true`
- **EndpointClassName** - name of the class that will contain the endpoint function, by default it is
  `Get{PluralEntityName}Endpoint`
- **EndpointFunctionName** - name of the function that will be called when the endpoint is hit, by default it is
  `GetAsync`
- **RouteName** - name of the route that will be used to call the endpoint, by default it is `/{EntityName}`

Note:

* `Get` text is taken from the `Operation`, and would be replaced if the other `Operation` value provided
* `{PluralEntityName}` will be replaced with the pluralized name of the entity class

Customization example:

```csharp
GetByIdOperation = new() {
    OperationGroup = "GetReadOnlyModelCustomNamespace",
    QueryName = "GetReadOnlyModelQuery",
    HandlerName = "GetReadOnlyModelHandler",
    DtoName = "ReadOnlyModelCustomDto",
    EndpointClassName = "GetReadOnlyModelCustomizedEndpoint",
    EndpointFunctionName = "RunGetAsync",
    RouteName = "/getCustomizedReadOnlyModelById/{{id_param_name}}"
};
```

## Get by id operation

- **Generate** - defines if the operation should be generated at all or not, by default it is `true`
- **Operation** - name of the operation, by default it is `Get`
- **OperationGroup** - namespace where all related classes for operation handling will be placed in
- **QueryName** - name of the CQRS query class, by default it is `Get{EntityName}Query`
- **DtoName** - name of the DTO class that will be returned as a result of operation execution, by default it is
  `{EntityName}Dto`
- **HandlerName** - name of the CQRS query handler class, by default it is `Get{EntityName}Handler`
- **GenerateEndpoint** - defines if the endpoint should be generated at all or not, by default it is `true`
- **EndpointClassName** - name of the class that will contain the endpoint function, by default it is
  `Get{EntityName}Endpoint`
- **EndpointFunctionName** - name of the function that will be called when the endpoint is hit, by default it is
  `GetAsync`
- **RouteName** - name of the route that will be used to call the endpoint, by default it is `/{EntityName}/{EntityId}`

Note:

* `Get` text is taken from the `Operation`, and would be replaced if the other `Operation` value provided
* `{EntityName}` will be replaced with the name of the entity class

## Update operation

- **Generate** - defines if the operation should be generated at all or not, by default it is `true`
- **Operation** - name of the operation, by default it is `Update`
- **OperationGroup** - namespace where all related classes for operation handling will be placed in
- **CommandName** - name of the CQRS command class, by default it is `Update{EntityName}Command`
- **HandlerName** - name of the CQRS command handler class, by default it is `Update{EntityName}Handler`
- **GenerateEndpoint** - defines if the endpoint should be generated at all or not, by default it is `true`
- **EndpointClassName** - name of the class that will contain the endpoint function, by default it is
  `Update{EntityName}Endpoint`
- **EndpointFunctionName** - name of the function that will be called when the endpoint is hit, by default it is
  `UpdateAsync`
- **RouteName** - name of the route that will be used to call the endpoint, by default it is
  `/{EntityName}/{EntityId}/update`
- **ViewModelName** - name of the view model class that is accepted in the body of the endpoint and is used to update
  the entity, by default it is `Update{EntityName}Vm`

* `Update` text is taken from the `Operation`, and would be replaced if the other `Operation` value provided
* `{EntityName}` will be replaced with the name of the entity class

Customization example:

```csharp
UpdateOperation = new() {
    OperationGroup = "ManagedEntityUpdateOperationCustomNs",
    CommandName = "CustomizedNameUpdateManagedEntityCommand",
    HandlerName = "CustomizedNameUpdateManagedEntityHandler",
    ViewModelName = "CustomizedNameUpdateManagedEntityViewModel",
    EndpointClassName = "CustomizedNameUpdateManagedEntityEndpoint",
    EndpointFunctionName = "RunUpdateAsync",
    RouteName = "/customizedManagedEntityUpdate/{{id_param_name}}"
};
```

## Delete operation
- **Generate** - defines if the operation should be generated at all or not, by default it is `true`
- **Operation** - name of the operation, by default it is `Delete`
- **OperationGroup** - namespace where all related classes for operation handling will be placed in
- **CommandName** - name of the CQRS command class, by default it is `Delete{EntityName}Command`
- **HandlerName** - name of the CQRS command handler class, by default it is `Delete{EntityName}Handler`
- **GenerateEndpoint** - defines if the endpoint should be generated at all or not, by default it is `true`
- **EndpointClassName** - name of the class that will contain the endpoint function, by default it is
  `Delete{EntityName}Endpoint`
- **EndpointFunctionName** - name of the function that will be called when the endpoint is hit, by default it is
  `DeleteAsync`
- **RouteName** - name of the route that will be used to call the endpoint, by default it is
  `/{EntityName}/{EntityId}/delete`

* `Delete` text is taken from the `Operation`, and would be replaced if the other `Operation` value provided
* `{EntityName}` will be replaced with the name of the entity class

Customization example:

```csharp
DeleteOperation = new() {
    OperationGroup = "ManagedEntityDeleteOperationCustomNs",
    CommandName = "CustomizedNameDeleteManagedEntityCommand",
    HandlerName = "CustomizedNameDeleteManagedEntityHandler",
    EndpointClassName = "CustomizedNameDeleteManagedEntityEndpoint",
    EndpointFunctionName = "RunDeleteAsync",
    RouteName = "/customizedManagedEntityDelete/{{entity_name}}/{{id_param_name}}"
};
```