using System.Reflection;

namespace UIKitWebApp;
public class MauiPackage : IPackage {
    public async Task<Stream> GetBundleAsync(SelectionModel model) {

        return await Task.Run(async () => {
            if (Directory.Exists(PathCons.OUTPUT))
                Directory.Delete(PathCons.OUTPUT, true);

            try {
                if (model == null) return null;

                CreateProject(model);

                await CopyDependentFiles(model);

                await SetStartupPage(model.StartupPage);

                return Compress(model.AppName);

            } catch (Exception ex) {
                return null;
            }
        });
    }

    public async Task<Stream> GetStandalonePackageAsync(SelectionModel model) {

        return await Task.Run(async () => {
            if (Directory.Exists(PathCons.OUTPUT))
                Directory.Delete(PathCons.OUTPUT, true);

            TemplateParser templateParser = new TemplateParser();

            await CopyDependentFiles(model);

            return Compress("Package");
        });

        throw new NotImplementedException();
    }
    private void CreateProject(SelectionModel model) {

        //Copy maui project template
        if (!Directory.Exists(PathCons.OUTPUT))
            Directory.CreateDirectory(PathCons.OUTPUT);

        var sourceDirectory = new DirectoryInfo(Path.Combine(PathCons.RESOURCES, model?.Platform.ToString(), "Template"));
        sourceDirectory.DeepCopy(PathCons.OUTPUT);

        if (model?.AppName == null) return;
        UpdateProjectFileNames(model?.AppName);
    }



    private void UpdateProjectFileNames(string appName) {

        string sourceFile = Path.Combine(PathCons.OUTPUT, "MAUITemplate.csproj");
        string targetFile = Path.Combine(PathCons.OUTPUT, appName + ".csproj");

        //Renames CSProject file.
        System.IO.File.Move(sourceFile, targetFile, true);

        sourceFile = Path.Combine(PathCons.OUTPUT, "MAUITemplate.sln");
        targetFile = Path.Combine(PathCons.OUTPUT, appName + ".sln");

        System.IO.File.Move(sourceFile, targetFile, true);

        foreach (string newPath in Directory.GetFiles(PathCons.OUTPUT, "*.*", SearchOption.AllDirectories)) {
            newPath.RepaceFileContentString("MAUITemplate", appName);
        }
    }

    private void CopyDependentFiles(Template template, string appName = null, bool isReplaceNamespaceName = false) {

        TemplateParser templateParser = new TemplateParser();

        var templates = templateParser.GetAllTemplates();

        var selectedTemplate = templates.FirstOrDefault(x => x.Name == template.Name && x.Platform == template.Platform);

        foreach (File file in selectedTemplate.Dependencies) {

            var fileLookupPath = Path.Combine(PathCons.RESOURCES, selectedTemplate.Platform.ToString(), file.Folder, template.Category, file.Name);

            var destinationPath = Path.Combine(PathCons.OUTPUT, file.Folder, file.Name);

            if (!string.IsNullOrEmpty(fileLookupPath)) {
                var destinationDirectory = Path.GetDirectoryName(destinationPath);
                if (!Directory.Exists(destinationDirectory))
                    Directory.CreateDirectory(destinationDirectory);

                System.IO.File.Copy(fileLookupPath, destinationPath, true);
                if (isReplaceNamespaceName && !string.IsNullOrWhiteSpace(appName))
                    destinationPath.RepaceFileContentString("MAUITemplate", appName);
            }
        }

    }

    private async Task CopyDependentFiles(SelectionModel model) {
        await Task.Delay(2000);

        TemplateParser templateParser = new TemplateParser();

        var templates = templateParser.GetAllTemplates();

        var selectedTemplates = templates.Where(x => model?.Templates?.Contains(x.Name) == true && x.Platform == model.Platform).ToList();

        foreach (var template in selectedTemplates) {
            if (model.PackageType == PackageType.Bundle)
                CopyDependentFiles(template, model.AppName, true);
            else CopyDependentFiles(template);
        }
    }

    //private async Task CopyDependentFiles(SelectionModel model) {
    //    await Task.Delay(2000);

    //    Console.WriteLine("Creating dependent folders and files for this template...");

    //    string category = null;

    //    using (XmlReader reader = XmlReader.Create(PathCons.TEMPLATEFILE)) {
    //        while (reader.Read()) {
    //            if (reader.IsStartElement()) {
    //                //return only when you have START tag  
    //                switch (reader.Name.ToString()) {
    //                    case ElementCons.Template: {
    //                            category = reader.GetAttribute(AttributeCons.Category);
    //                            var templateName = reader.GetAttribute(AttributeCons.Name);
    //                            if (!model.Templates.Contains(templateName))
    //                                reader.Skip();
    //                            break;
    //                        }
    //                    case ElementCons.File: {
    //                            var fileName = reader.GetAttribute(AttributeCons.Name);
    //                            var subFolder = reader.GetAttribute(AttributeCons.Folder);

    //                            var fileLookupPath = Path.Combine(PathCons.RESOURCES, model.Platform.ToString(), subFolder, category, fileName);
    //                            var destinationPath = Path.Combine(PathCons.OUTPUT, subFolder, fileName);

    //                            if (!string.IsNullOrEmpty(fileLookupPath)) {
    //                                var destinationDirectory = Path.GetDirectoryName(destinationPath);
    //                                if (!Directory.Exists(destinationDirectory))
    //                                    Directory.CreateDirectory(destinationDirectory);

    //                                System.IO.File.Copy(fileLookupPath, destinationPath, true);

    //                                destinationPath.RepaceFileContentString("MAUITemplate", model.AppName);

    //                            }

    //                            break;
    //                        }
    //                }
    //            }
    //        }

    //    }

    //    Console.WriteLine("Files are placed properly.");
    //}

    private async Task SetStartupPage(string startupPage) {
        await Task.Delay(2000);

        Console.WriteLine("Creating Startup of the current project.");

        //Set startup page

        string filePath = Path.Combine(PathCons.OUTPUT, "App.xaml.cs");

        filePath.RepaceFileContentString("AppShell", startupPage);

        Console.WriteLine("Runnable project is ready...");
    }


    private Stream Compress(string appName) {
        string zipFileName = appName + ".zip";

        if (System.IO.File.Exists(zipFileName))
            System.IO.File.Delete(zipFileName);

        ZipFile.CreateFromDirectory(PathCons.OUTPUT, zipFileName);

        return new FileStream(zipFileName, FileMode.Open, FileAccess.Read);
    }
}
