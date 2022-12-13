namespace UIKitWebApp;
public static class Extension
{
    public static void DeepCopy(this DirectoryInfo directory, string destinationDir)
    {
        foreach (string dir in Directory.GetDirectories(directory.FullName, "*", SearchOption.AllDirectories))
        {
            string dirToCreate = dir.Replace(directory.FullName, destinationDir);
            Directory.CreateDirectory(dirToCreate);
        }

        foreach (string newPath in Directory.GetFiles(directory.FullName, "*.*", SearchOption.AllDirectories))
        {
            System.IO.File.Copy(newPath, newPath.Replace(directory.FullName, destinationDir), true);
        }
    }

    public static void RepaceFileContentString(this string filePath, string oldString, string newString)
    {
        string csprojString = null;

        using (StreamReader appReader = new StreamReader(filePath))
        {
            csprojString = appReader.ReadToEnd();
        };

        using (StreamWriter appWriter = new StreamWriter(filePath))
        {
            appWriter.WriteLine(csprojString.Replace(oldString, newString));
        };
    }
}
