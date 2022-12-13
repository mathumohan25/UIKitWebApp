namespace UIKitWebApp;
public class PackageFactory {
    public IPackage? GetPackage(Platform? platform) {
        IPackage? package = null;

        switch(platform) {
            case Platform.maui: {
                    package = new MauiPackage(); 
                    break;
                }
            default: { return package; }
        }

        return package;
    }

}
