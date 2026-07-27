using PlaywrightAutomationPoc.AutoFramework.Browser;

namespace PlaywrightAutomationPoc.Tests;

public class BrowserFactoryTests
{
    [Theory]
    [InlineData(BrowserType.Chromium, "chromium", "chromium")]
    [InlineData(BrowserType.Chrome, "chromium", "chrome")]
    [InlineData(BrowserType.Firefox, "firefox", "firefox")]
    [InlineData(BrowserType.Webkit, "webkit", "")]
    [InlineData(BrowserType.MsEdge, "chromium", "msedge")]
    public void BrowserFactory_MapsExpectedBrowserAndChannel(BrowserType browserType, string expectedBrowserName, string expectedChannel)
    {
        var browserName = BrowserFactory.GetBrowserName(browserType);
        var channelName = BrowserFactory.GetChannelName(browserType);

        Assert.Equal(expectedBrowserName, browserName);
        Assert.Equal(expectedChannel, channelName);
    }
}
