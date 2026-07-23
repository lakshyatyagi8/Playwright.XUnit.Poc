using Xunit.Sdk;
using System.Reflection;

namespace PlaywrightAutomationPoc.AutoFramework.Browser
{
    public class WithTestNameAttribute : BeforeAfterTestAttribute
    {
        public static string CurrentTestName = string.Empty;
        public static string CurrentClassName = string.Empty;

        public override void Before(MethodInfo methodInfo)
        {
            CurrentTestName = methodInfo.Name;
            CurrentClassName = methodInfo.DeclaringType!.Name;
        }

        public override void After(MethodInfo methodInfo)
        {
        }
    }
}