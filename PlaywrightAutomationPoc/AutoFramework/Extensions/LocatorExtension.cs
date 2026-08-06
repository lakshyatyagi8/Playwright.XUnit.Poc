// Copyright lakshyatyagi8@gmail.com. All Rights Reserved.
using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.AutoFramework.Extensions
{
    public static class LocatorExtension
    {
        /// <summary>
        /// Sets text value on a given locator after waiting for it to be ready.
        /// </summary>
        /// <param name="locator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task SetTextValueAndEnter(this ILocator locator, string value)
        {
            await locator.ClickAsync();
            await locator.FillAsync(value);
            await locator.Page.Keyboard.PressAsync("Enter");
        }
    }
}