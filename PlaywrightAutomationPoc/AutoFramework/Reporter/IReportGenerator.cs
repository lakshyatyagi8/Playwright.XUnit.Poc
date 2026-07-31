// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
namespace PlaywrightAutomationPoc.AutoFramework.Reporter
{
    public interface IReportGenerator
    {
        void InitializeReport(string reportPath);
        void CreateTest(string testName);
        void LogInfo(string message);
        void LogPass(string message);
        void LogFail(string message, string? base64ScreenCapture = null);
        void Flush();
    }
}