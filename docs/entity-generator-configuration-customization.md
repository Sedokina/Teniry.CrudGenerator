# Customization

Using configuration class you can customize the generated code.

Each operation has own properties that can be customized.

## Create operation

- **OperationGroup** - namespace where all related classes for operation handling will be placed in
- **OperationName** - name of the operation, by default it is `Create`
- **CommandName** - name of the CQRS command class, by default it is `Create{EntityName}Command`
- **HandlerName** - name of the CQRS command handler class, by default it is `Create{EntityName}Handler`
- **DtoName** - name of the DTO class that will be returned as a result of operation execution, by default it is
  `Created{EntityName}Dto`
- **EndpointClassName** - name of the class that will contain the endpoint function, by default it is
  `Create{EntityName}Endpoint`
- **EndpointFunctionName** - name of the function that will be called when the endpoint is hit, by default it is
  `CreateAsync`
- **RouteName** - name of the route that will be used to call the endpoint, by default it is `/{EntityName}/create`

Note:

* `Create` text is taken from the `OperationName`, and would be replaced if the other `OperationName` value provided
* Where `{EntityName}` is used, it will be replaced with the name of the entity class

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