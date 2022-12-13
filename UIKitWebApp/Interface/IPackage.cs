namespace UIKitWebApp;

public interface IPackage
{
    Task<Stream> GetBundleAsync(SelectionModel model);
    Task<Stream> GetStandalonePackageAsync(SelectionModel model);
}


