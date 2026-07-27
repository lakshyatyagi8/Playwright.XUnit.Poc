// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using BrowserKind = PlaywrightAutomationPoc.AutoFramework.Enum.BrowserType;
using PlaywrightAutomationPoc.AutoFramework.Browser;

namespace PlaywrightAutomationPoc.Tests;

public class BrowserFactoryTests
{
    [Theory]
    [InlineData(BrowserKind.Chromium, "chromium", "chromium")]
    [InlineData(BrowserKind.Chrome, "chromium", "chrome")]
    [InlineData(BrowserKind.Firefox, "firefox", "firefox")]
    [InlineData(BrowserKind.Webkit, "webkit", "")]
    [InlineData(BrowserKind.MsEdge, "chromium", "msedge")]
    public void BrowserFactory_MapsExpectedBrowserAndChannel(BrowserKind browserType, string expectedBrowserName, string expectedChannel)
    {
        var browserName = BrowserFactory.GetBrowserName(browserType);
        var channelName = BrowserFactory.GetChannelName(browserType);

        Assert.Equal(expectedBrowserName, browserName);
        Assert.Equal(expectedChannel, channelName);
    }
}
