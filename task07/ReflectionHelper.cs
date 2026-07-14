using System.Reflection;
using System.Text;

namespace task07;

public static class ReflectionHelper
{
    public static string PrintTypeInfo(Type type)
    {
        var sb = new StringBuilder();

        var displayName = type.GetCustomAttribute<DisplayNameAttribute>();
        if (displayName is not null)
            sb.AppendLine($"Класс: {displayName.DisplayName}");

        var version = type.GetCustomAttribute<VersionAttribute>();
        if (version is not null)
            sb.AppendLine($"Версия: {version.Major}.{version.Minor}");

        var methods = type
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(m => !m.IsSpecialName)
            .Select(m => new
            {
                m.Name,
                Attr = m.GetCustomAttribute<DisplayNameAttribute>()
            })
            .Where(x => x.Attr is not null)
            .Select(x => $"Метод {x.Name}: {x.Attr!.DisplayName}");

        var properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(p => new
            {
                p.Name,
                Attr = p.GetCustomAttribute<DisplayNameAttribute>()
            })
            .Where(x => x.Attr is not null)
            .Select(x => $"Свойство {x.Name}: {x.Attr!.DisplayName}");

        methods.ToList().ForEach(line => sb.AppendLine(line));
        properties.ToList().ForEach(line => sb.AppendLine(line));

        return sb.ToString();
    }
}