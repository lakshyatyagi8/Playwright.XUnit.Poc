using Microsoft.Playwright;

namespace PlaywrightAutomationPoc.AutoFramework.Extensions
{
    public static class LocatorExtension
    {
        /// <summary>
        /// Waits for the locator to be visible and enabled before clicking.
        /// </summary>
        /// <param name="locator"></param>
        /// <returns></returns>
        public static async Task WaitAndClickAsync(this ILocator locator)
        {
            await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000  });
            await locator.ClickAsync();
        }

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