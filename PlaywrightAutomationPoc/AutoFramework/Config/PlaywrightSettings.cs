// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
namespace PlaywrightAutomationPoc.AutoFramework.Config
{
public class PlaywrightSettings
    {
        public string BrowserType { get; set; } = "chromium";
        public string Channel { get; set; } = "";
        public bool Headless { get; set; } = true;
        public int SlowMo { get; set; } = 0;
        public bool Devtools { get; set; } = false;
        public float DefaultTimeout { get; set; } = 30000;
        public int ViewportWidth { get; set; } = 1920;
        public int ViewportHeight { get; set; } = 1080;
        public string Tracing { get; set; } = "off";
        public string Video { get; set; } = "off";
        public string Screenshot { get; set; } = "off";
    }
}