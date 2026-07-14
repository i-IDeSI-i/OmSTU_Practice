using Xunit;
using task05;

public class TestClass
{
    public int PublicField;
    private string _privateField = string.Empty;
    public int Property { get; set; }

    public void Method() { }
}

[Serializable]
public class AttributedClass { }

public class ClassAnalyzerTests
{
    [Fact]
    public void GetPublicMethods_ReturnsCorrectMethods()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var methods = analyzer.GetPublicMethods();

        Assert.Contains("Method", methods);
    }

    [Fact]
    public void GetAllFields_IncludesPrivateFields()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var fields = analyzer.GetAllFields();

        Assert.Contains("_privateField", fields);
    }

    [Fact]
    public void GetProperties_ReturnsCorrectProperties()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var properties = analyzer.GetProperties();

        Assert.Contains("Property", properties);
    }

    [Fact]
    public void HasAttribute_WhenAttributePresent_ReturnsTrue()
    {
        var analyzer = new ClassAnalyzer(typeof(AttributedClass));
        Assert.True(analyzer.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void HasAttribute_WhenAttributeAbsent_ReturnsFalse()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        Assert.False(analyzer.HasAttribute<SerializableAttribute>());
    }

    [Fact]
    public void GetMethodParams_ReturnsParamsAndReturnType()
    {
        var analyzer = new ClassAnalyzer(typeof(TestClass));
        var result = analyzer.GetMethodParams("Method").ToList();

        Assert.Contains("returns: Void", result);
    }
}