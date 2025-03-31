# EntityGeneratorConfiguration

EntityGeneratorConfiguration is a class that holds the configuration for the CRUD Generator. It defines which class
should have CRUD operations generated and allows to customize the generated code.

Based on configuration, the generator will generate endpoints and handlers for:

- Create operation
- Get list operation
- Get by id operation
- Update operation
- Delete operation

# Entity

CRUD generator can generate CRUD operations for any provided class.

> [!WARNING]
> The class should have a public constructor with no parameters.

Currently CRUD generator supports mapping only to the classes with default constructor.

# Configuration

To generate CRUD operations for a class, you have to create declare a class that inherits
from `EntityGeneratorConfiguration`.

On build, the generator will look for all classes that inherit from `EntityGeneratorConfiguration` and generate CRUD
operations for the classes from configuration.

# Example: Minimal configuration

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

