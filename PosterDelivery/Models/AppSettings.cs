namespace PosterDelivery.Models;

public class AppSettings
{
    public int SessionTimeout { get; set; }
    public string AzureStorageAccountName { get; set; }
    public string AzureStorageKey { get; set; }
    public string AzureStorageContainer { get; set; }
    public string AzureProductImageStorageContainer { get; set; }
}

