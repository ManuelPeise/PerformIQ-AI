namespace Web.Mobile.Services;

public sealed class AppDialogService : IAppDialogService
{
    public async Task ShowMessageAsync(string title, string message, string cancel = "OK")
    {
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            var page = Application.Current?.Windows.FirstOrDefault()?.Page;
            if (page is not null)
            {
                await page.DisplayAlertAsync(title, message, cancel);
            }
        });
    }
}
