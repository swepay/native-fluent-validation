using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Native.FluentValidation.SourceGenerator;

[Generator]
public sealed class PropertyNameGenerator : IIncrementalGenerator
{
    private const string AttributeSource = """
namespace Native.FluentValidation.SourceGeneration;

[System.AttributeUsage(System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
internal sealed class GeneratePropertyNamesAttribute : System.Attribute
{
}
""";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
            ctx.AddSource("GeneratePropertyNamesAttribute.g.cs", SourceText.From(AttributeSource, Encoding.UTF8)));

        var typeProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Native.FluentValidation.SourceGeneration.GeneratePropertyNamesAttribute",
                static (node, _) => node is ClassDeclarationSyntax,
                static (ctx, _) => ctx.TargetSymbol as INamedTypeSymbol)
            .Where(static type => type is not null);

        var collected = context.CompilationProvider.Combine(typeProvider.Collect());

        context.RegisterSourceOutput(collected, (spc, source) =>
        {
            var (compilation, types) = source;
            foreach (var type in types)
            {
                if (type is null)
                {
                    continue;
                }

                var properties = type.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(p => !p.IsStatic && p.GetMethod is not null && p.DeclaredAccessibility == Accessibility.Public)
                    .ToArray();

                if (properties.Length == 0)
                {
                    continue;
                }

                var namespaceName = type.ContainingNamespace.IsGlobalNamespace
                    ? ""
                    : $"namespace {type.ContainingNamespace.ToDisplayString()};";

                var propertyNamesClass = $"{type.Name}PropertyNames";
                var propertiesClass = $"{type.Name}Properties";
                var builder = new StringBuilder();

                if (!string.IsNullOrWhiteSpace(namespaceName))
                {
                    builder.AppendLine(namespaceName);
                    builder.AppendLine();
                }

                builder.AppendLine($"public static class {propertyNamesClass}");
                builder.AppendLine("{");

                foreach (var property in properties)
                {
                    var identifier = EscapeIdentifier(property.Name);
                    builder.AppendLine($"    public const string {identifier} = \"{property.Name}\";");
                }

                builder.AppendLine("}");

                builder.AppendLine();
                builder.AppendLine("public static class " + propertiesClass);
                builder.AppendLine("{");

                foreach (var property in properties)
                {
                    var identifier = EscapeIdentifier(property.Name);
                    var propertyType = property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
                    var modelType = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

                    builder.AppendLine(
                        $"    public static Native.FluentValidation.Core.PropertySelector<{modelType}, {propertyType}> {identifier} {{ get; }} =" +
                        $" new Native.FluentValidation.Core.PropertySelector<{modelType}, {propertyType}>(x => x.{property.Name}, {propertyNamesClass}.{identifier});");
                }

                builder.AppendLine("}");

                spc.AddSource($"{type.Name}.PropertyNames.g.cs", SourceText.From(builder.ToString(), Encoding.UTF8));
            }
        });
    }

    private static string EscapeIdentifier(string name)
    {
        if (SyntaxFacts.GetKeywordKind(name) != SyntaxKind.None ||
            SyntaxFacts.GetContextualKeywordKind(name) != SyntaxKind.None)
        {
            return "@" + name;
        }

        return name;
    }
}
