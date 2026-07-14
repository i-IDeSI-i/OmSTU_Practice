using System.Reflection;
using System.Text;

if (args.Length == 0)
{
    Console.WriteLine("Использование: MetadataViewer <путь-к-библиотеке.dll>");
    return;
}

string dllPath = args[0];

if (!File.Exists(dllPath))
{
    Console.WriteLine($"Файл не найден: {dllPath}");
    return;
}

Assembly assembly = Assembly.LoadFrom(dllPath);
Console.WriteLine(DescribeAssembly(assembly));


static string DescribeAssembly(Assembly assembly)
{
    var sb = new StringBuilder();
    sb.AppendLine($"Сборка: {assembly.GetName().Name}");
    sb.AppendLine(new string('=', 50));

    var types = assembly.GetExportedTypes();
    types.ToList().ForEach(t => sb.AppendLine(DescribeType(t)));

    return sb.ToString();
}

static string DescribeType(Type type)
{
    var sb = new StringBuilder();
    sb.AppendLine($"\nКласс: {type.FullName}");

    var attributes = type.GetCustomAttributes(false)
        .Select(a => a.GetType().Name);
    sb.AppendLine($"  Атрибуты: {FormatList(attributes)}");

    sb.AppendLine("  Конструкторы:");
    type.GetConstructors()
        .Select(DescribeConstructor)
        .ToList()
        .ForEach(line => sb.AppendLine($"    {line}"));

    sb.AppendLine("  Методы:");
    type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
        .Where(m => !m.IsSpecialName)
        .Select(DescribeMethod)
        .ToList()
        .ForEach(line => sb.AppendLine($"    {line}"));

    return sb.ToString();
}

static string DescribeConstructor(ConstructorInfo ctor)
    => $".ctor({FormatParameters(ctor.GetParameters())})";

static string DescribeMethod(MethodInfo method)
    => $"{method.ReturnType.Name} {method.Name}({FormatParameters(method.GetParameters())})";

static string FormatParameters(ParameterInfo[] parameters)
    => string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));

static string FormatList(IEnumerable<string> items)
{
    var list = items.ToList();
    return list.Count == 0 ? "(нет)" : string.Join(", ", list);
}