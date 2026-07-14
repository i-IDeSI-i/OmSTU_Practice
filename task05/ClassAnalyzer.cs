using System.Reflection;

namespace task05;

public class ClassAnalyzer
{
    private readonly Type _type;

    public ClassAnalyzer(Type type) => _type = type;

    public IEnumerable<string> GetPublicMethods()
        => _type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName)
            .Select(m => m.Name);

    public IEnumerable<string> GetMethodParams(string methodname)
    {
        var method = _type.GetMethod(methodname);
        if (method is null)
            return Enumerable.Empty<string>();

        var parameters = method.GetParameters().Select(p => p.Name ?? string.Empty);
        return parameters.Append($"returns: {method.ReturnType.Name}");
    }

    public IEnumerable<string> GetAllFields()
        => _type
            .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Select(f => f.Name);

    // Имена свойств
    public IEnumerable<string> GetProperties()
        => _type
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Select(p => p.Name);

    public bool HasAttribute<T>() where T : Attribute
        => _type.GetCustomAttribute<T>() is not null;
}