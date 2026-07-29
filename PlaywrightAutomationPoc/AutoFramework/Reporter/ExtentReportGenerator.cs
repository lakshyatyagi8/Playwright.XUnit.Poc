// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace PlaywrightAutomationPoc.AutoFramework.Reporting
{
    public class ExtentReportGenerator : IReportGenerator
    {
        private static ExtentReports? _extent;
        private static readonly AsyncLocal<ExtentTest?> _currentTest = new();

        public void InitializeReport(string reportPath)
        {
            var htmlReporter = new ExtentSparkReporter(reportPath);
            _extent = new ExtentReports();
            _extent.AttachReporter(htmlReporter);

            // Optional: Add system info
            _extent.AddSystemInfo("Environment", "Dev");
            _extent.AddSystemInfo("Framework", "Playwright xUnit .NET");
        }

        public void CreateTest(string testName)
        {
            if (_extent is null)
            {
                throw new InvalidOperationException("Report has not been initialized. Call InitializeReport first.");
            }

            _currentTest.Value = _extent.CreateTest(testName);
        }

        public void LogInfo(string message) => _currentTest.Value?.Info(message);

        public void LogPass(string message) => _currentTest.Value?.Pass(message);

        public void LogFail(string message, string? base64ScreenCapture = null)
        {
            if (string.IsNullOrEmpty(base64ScreenCapture))
            {
                _currentTest.Value?.Fail(message);
            }
            else
            {
                // Attaches a screenshot directly into the HTML report
                _currentTest.Value?.Fail(message, MediaEntityBuilder.CreateScreenCaptureFromBase64String(base64ScreenCapture).Build());
            }
        }

        public void Flush()
        {
            _extent?.Flush();
        }
    }
}