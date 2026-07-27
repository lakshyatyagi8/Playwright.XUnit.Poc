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
        public static async Task SafeClickAsync(this ILocator locator)
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
        public static async Task SetTextValueAndEnterOnLocator(this ILocator locator, string value, ILocator? waitForVisiblePostFillLocator = null)
        {
            await locator.WaitForAsync(new() { State = WaitForSelectorState.Visible });
            await locator.FillAsync(value);
            if (waitForVisiblePostFillLocator != null)
            {
                await waitForVisiblePostFillLocator.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
            }
            await locator.PressAsync("Enter");
        }
    }
}