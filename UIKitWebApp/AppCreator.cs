using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text.Json;
using System.Xml;

namespace UIKitWebApp {

    [Serializable]
    public class SelectionModel {

        public string AppName { get; set; }

        public List<string> Templates { get; set; }

        public Platform Platform { get; set; }

        public string StartupPage { get; set; }

    }

    public enum Platform {
        maui = 0
    }

    public static class Extension {
        public static void DeepCopy(this DirectoryInfo directory, string destinationDir) {
            foreach (string dir in Directory.GetDirectories(directory.FullName, "*", SearchOption.AllDirectories)) {
                string dirToCreate = dir.Replace(directory.FullName, destinationDir);
                Directory.CreateDirectory(dirToCreate);
            }

            foreach (string newPath in Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories)) {
                File.Copy(newPath, newPath.Replace(directory.FullName, destinationDir), true);
            }
        }

        public static void RepaceFileContentString(this string filePath, string oldString, string newString) {
            string csprojString = null;

            using (StreamReader appReader = new StreamReader(filePath)) {
                csprojString = appReader.ReadToEnd();
            };

            using (StreamWriter appWriter = new StreamWriter(filePath)) {
                appWriter.WriteLine(csprojString.Replace(oldString, newString));
            };
        }
    }

    public class AppCreator {
        const string RESOURCE_PATH = "Resources";
        const string OUTPUT_PATH = "Output";
        public string AppName { get; set; }
        public AppCreator() {

        }

        private void CreateProject(SelectionModel model) {

            Console.WriteLine("Creating MAUI project...");

            Task.Delay(2000).Wait();

            //Copy maui project template
            if (!Directory.Exists(OUTPUT_PATH))
                Directory.CreateDirectory(OUTPUT_PATH);

            var sourceDirectory = new DirectoryInfo(Path.Combine(RESOURCE_PATH, model.Platform.ToString(), "Template"));
            sourceDirectory.DeepCopy(OUTPUT_PATH);

            UpdateProjectFileNames(model.AppName);

            Console.WriteLine("MAUI project has been created...");
        }



        private void UpdateProjectFileNames(string appName) {

            string sourceFile = Path.Combine(OUTPUT_PATH, "MAUITemplate.csproj");
            string targetFile = Path.Combine(OUTPUT_PATH, appName + ".csproj");

            //Renames CSProject file.
            System.IO.File.Move(sourceFile, targetFile, true);

            sourceFile = Path.Combine(OUTPUT_PATH, "MAUITemplate.sln");
            targetFile = Path.Combine(OUTPUT_PATH, appName + ".sln");

            System.IO.File.Move(sourceFile, targetFile, true);

            foreach (string newPath in Directory.GetFiles(OUTPUT_PATH, "*.*", SearchOption.AllDirectories)) {
                newPath.RepaceFileContentString("MAUITemplate", appName);
            }
        }

        private void CopyDependentFiles(SelectionModel model) {
            Task.Delay(2000).Wait();

            Console.WriteLine("Creating dependent folders and files for this template...");

            string category = null;

            using (XmlReader reader = XmlReader.Create("Templates.xml")) {
                while (reader.Read()) {
                    if (reader.IsStartElement()) {
                        //return only when you have START tag  
                        switch (reader.Name.ToString()) {
                            case "Template": {
                                    category = reader.GetAttribute("Category");
                                    var templateName = reader.GetAttribute("Name");
                                    if (!model.Templates.Contains(templateName))
                                        reader.Skip();
                                    break;
                                }
                            case "File": {
                                    var fileName = reader.GetAttribute("Name");
                                    var subFolder = reader.GetAttribute("Folder");

                                    var fileLookupPath = Path.Combine(RESOURCE_PATH, model.Platform.ToString(), subFolder, category, fileName);
                                    var destinationPath = Path.Combine(OUTPUT_PATH, subFolder, fileName);

                                    if (!string.IsNullOrEmpty(fileLookupPath)) {
                                        var destinationDirectory = Path.GetDirectoryName(destinationPath);
                                        if (!Directory.Exists(destinationDirectory))
                                            Directory.CreateDirectory(destinationDirectory);

                                        File.Copy(fileLookupPath, destinationPath, true);

                                        destinationPath.RepaceFileContentString("MAUITemplate", model.AppName);

                                    }

                                    break;
                                }
                        }
                    }
                }

            }

            Task.Delay(2000).Wait();

            Console.WriteLine("Files are placed properly.");
        }

        private void SetStartupPage(string startupPage) {
            Task.Delay(2000).Wait();

            Console.WriteLine("Creating Startup of the current project.");

            //Set startup page

            string filePath = Path.Combine(OUTPUT_PATH, "App.xaml.cs");

            filePath.RepaceFileContentString("AppShell", startupPage);

            Task.Delay(2000).Wait();

            Console.WriteLine("Runnable project is ready...");
        }

        public void Start() {

            // Input JSON
            SelectionModel model = new SelectionModel() {
                AppName = "MAUIApp2",
                Platform = Platform.maui,
                Templates = new List<string>() {
                "LoginPage",
            },
                StartupPage = "LoginPage"

            };

            string json = JsonSerializer.Serialize(model);
           

            CreateProject(model);

            CopyDependentFiles(model);

            SetStartupPage(model.StartupPage);

        }

        public void Start(string json) {
            if (string.IsNullOrWhiteSpace(json)) return;

            if(Directory.Exists(OUTPUT_PATH))
                Directory.Delete(OUTPUT_PATH, true);

            try {
                SelectionModel model = JsonSerializer.Deserialize<SelectionModel>(json);
                AppName = model.AppName;
                CreateProject(model);

                CopyDependentFiles(model);

                SetStartupPage(model.StartupPage);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }

        }

        public Stream Compress(string appName) {
            string zipFileName = appName + ".zip";

            if (File.Exists(zipFileName))
                File.Delete(zipFileName);

            ZipFile.CreateFromDirectory(OUTPUT_PATH, appName+".zip");

            return new FileStream(appName + ".zip", FileMode.Open, FileAccess.Read);
        }

    }
}
