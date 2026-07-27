// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using System.Text.Json;

namespace PlaywrightAutomationPoc.Config
{
    public interface ITestSettingInitializer
    {        
        TestSetting GetTestSettingByConfigFile(string envName);
        string GetConfigFileByEnvName(string envName);
    }

    public class TestSettingInitializer : ITestSettingInitializer
    {
        private readonly string _sConfilgFileExtension = ".json";
        private readonly string _sConfigFolderPath = "Environment";

        public TestSetting GetTestSettingByConfigFile(string envName)
        {
            string configFilePath = Path.GetFullPath(GetConfigFileByEnvName(envName));
            return JsonSerializer.Deserialize<TestSetting>(File.ReadAllText(configFilePath)) ?? new TestSetting();
        }
        public string GetConfigFileByEnvName(string envName)
        {
            string currentPath = Directory.GetCurrentDirectory();
            return Path.Combine(currentPath, _sConfigFolderPath, string.Concat(envName, _sConfilgFileExtension));
        }
    }
}
