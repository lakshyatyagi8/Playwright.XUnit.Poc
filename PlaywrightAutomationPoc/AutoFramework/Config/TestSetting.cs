using System;
using System.Text.Json.Serialization;

namespace PlaywrightAutomationPoc.Config
{
    public interface ITestSetting
    {
        string BaseUrl { get; set; }
        int DefaultTimeout { get; set; }
        bool Headless { get; set; }
        string BrowserType { get; set; }
        bool Devtools { get; set; }
        int SlowMo { get; set; }
        Viewport Viewport { get; set; }
        string Trace { get; set; }
        string Video { get; set; }
        string Screenshot { get; set; }
    }
    
    public class TestSetting : ITestSetting
    {
        [JsonPropertyName("baseURL")]
        public string BaseUrl { get; set; } = "https://maps.google.com";

        [JsonPropertyName("defaultTimeout")]
        public int DefaultTimeout { get; set; } = 10000;

        [JsonPropertyName("headless")]
        public bool Headless { get; set; } = false;

        [JsonPropertyName("browserType")]
        public string BrowserType { get; set; } = "chrome";

        [JsonPropertyName("devtools")]
        public bool Devtools { get; set; } = true;

        [JsonPropertyName("slowMo")]
        public int SlowMo { get; set; } = 20000;

        [JsonPropertyName("viewport")]
        public Viewport Viewport { get; set; } = new Viewport();

        [JsonPropertyName("trace")]
        public string Trace { get; set; } = "on-first-retry";
    
        [JsonPropertyName("video")]
        public string Video { get; set; } = "retain-on-failure";
    
        [JsonPropertyName("screenshot")]
        public string Screenshot { get; set; } = "only-on-failure";

    }
    public class Viewport
    {
        [JsonPropertyName("width")]
        public int Width { get; set; } = 1920;
        [JsonPropertyName("height")]
        public int Height { get; set; } = 1080;
    }
}