using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Global;
using ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations.TypedBuiltConfigurations;

namespace ITech.CrudGenerator.CrudGeneratorCore.Configurations.Operations.BuiltConfigurations;

internal class CqrsOperationWithoutReturnValueGeneratorConfiguration
{
    public bool Generate { get; set; } = true;
    public GlobalCqrsGeneratorConfiguration GlobalConfiguration { get; set; } = null!;
    public CqrsOperationsSharedConfiguration OperationsSharedConfiguration { get; set; } = null!;
    public CqrsOperationType OperationType { get; set; }
    public string OperationGroup { get; set; } = "";
    public string OperationName { get; set; } = "";
    public string Operation { get; set; } = null!;
    public string Handler { get; set; } = null!;
    public MinimalApiEndpointConfiguration Endpoint { get; set; } = null!;

    protected bool Equals(CqrsOperationWithoutReturnValueGeneratorConfiguration other)
    {
        return Generate == other.Generate &&
               GlobalConfiguration.Equals(other.GlobalConfiguration) &&
               OperationsSharedConfiguration.Equals(other.OperationsSharedConfiguration) &&
               OperationType == other.OperationType &&
               OperationGroup == other.OperationGroup &&
               OperationName == other.OperationName &&
               Operation == other.Operation &&
               Handler == other.Handler &&
               Endpoint.Equals(other.Endpoint);
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((CqrsOperationWithoutReturnValueGeneratorConfiguration)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Generate.GetHashCode();
            hashCode = (hashCode * 397) ^ GlobalConfiguration.GetHashCode();
            hashCode = (hashCode * 397) ^ OperationsSharedConfiguration.GetHashCode();
            hashCode = (hashCode * 397) ^ (int)OperationType;
            hashCode = (hashCode * 397) ^ OperationGroup.GetHashCode();
            hashCode = (hashCode * 397) ^ OperationName.GetHashCode();
            hashCode = (hashCode * 397) ^ Operation.GetHashCode();
            hashCode = (hashCode * 397) ^ Handler.GetHashCode();
            hashCode = (hashCode * 397) ^ Endpoint.GetHashCode();
            return hashCode;
        }
    }
}