namespace UIKitWebApp;

[Serializable]
public class SelectionModel
{
    public string? AppName { get; set; }

    public List<string>? Templates { get; set; }

    public Platform? Platform { get; set; }

    public string? StartupPage { get; set; }

    public PackageType PackageType { get; set; } 

}
