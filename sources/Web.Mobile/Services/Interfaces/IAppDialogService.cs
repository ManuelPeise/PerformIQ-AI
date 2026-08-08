namespace Web.Mobile.Services;

public interface IAppDialogService
{
    Task ShowMessageAsync(string title, string message, string cancel = "OK");
}
