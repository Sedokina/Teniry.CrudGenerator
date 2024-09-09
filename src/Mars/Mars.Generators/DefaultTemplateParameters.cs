using System;

namespace Mars.Generators
{
    public class DefaultTemplateParameters
    {
        public DefaultTemplateParameters(
            string typeName, 
            string typeNamespace,
            string rootNamespace)
        {
            ClassName = typeName 
                        ?? throw new ArgumentNullException(nameof(typeName));
            Namespace = typeNamespace
                        ?? throw new ArgumentNullException(nameof(typeNamespace));
            PreferredNamespace = rootNamespace 
                                  ?? throw new ArgumentNullException(nameof(rootNamespace));
        }
        public string ClassName { get; set; }
        public string Namespace { get; set; }
        public string PreferredNamespace { get; set; }
    }
}