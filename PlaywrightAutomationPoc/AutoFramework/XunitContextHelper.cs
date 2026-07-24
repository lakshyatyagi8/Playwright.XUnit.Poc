using System.Reflection;
using Xunit.Abstractions;

namespace PlaywrightAutomationPoc.AutoFramework;

public static class XunitContextHelper
{
    public static (string ClassName, string TestName) GetTestContext(ITestOutputHelper output)
    {
        if (output == null) throw new ArgumentNullException(nameof(output));

        // Use reflection to extract the internal test field from xUnit's output helper
        var type = output.GetType();
        var testField = type.GetField("test", BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (testField?.GetValue(output) is not ITest test)
        {
            throw new InvalidOperationException("Could not extract test context from ITestOutputHelper.");
        }

        var testName = test.TestCase.TestMethod.Method.Name;
        var className = test.TestCase.TestMethod.TestClass.Class.Name.Split('.').Last();

        return (className, testName);
    }
}