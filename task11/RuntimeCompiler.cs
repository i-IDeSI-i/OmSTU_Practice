using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace task11;

public static class RuntimeCompiler
{
    public static T CompileAndCreate<T>(string sourceCode, string typeName) where T : class
    {
        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(ICalculator).Assembly.Location),
            MetadataReference.CreateFromFile(
                Assembly.Load("System.Runtime").Location),
        };

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName: "DynamicAssembly_" + Guid.NewGuid().ToString("N"),
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var ms = new MemoryStream();
        EmitResult result = compilation.Emit(ms);

        if (!result.Success)
        {
            var errors = result.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => d.GetMessage());
            throw new InvalidOperationException(
                "Ошибка компиляции:\n" + string.Join("\n", errors));
        }

        ms.Seek(0, SeekOrigin.Begin);
        Assembly assembly = Assembly.Load(ms.ToArray());
        Type type = assembly.GetType(typeName)
            ?? throw new InvalidOperationException($"Тип '{typeName}' не найден в сборке.");

        object instance = Activator.CreateInstance(type)
            ?? throw new InvalidOperationException("Не удалось создать экземпляр.");

        return (T)instance;
    }
}