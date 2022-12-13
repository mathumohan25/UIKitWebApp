namespace UIKitWebApp {
    public class TemplateParser {

        public List<Template> GetAllTemplates() {

            List<Template> templates = new List<Template>();

            using (XmlReader reader = XmlReader.Create("Templates.xml")) {
                Template template = null;
                while (reader.Read()) {
                    if (reader.IsStartElement()) {
                        //return only when you have START tag  
                        switch (reader.Name.ToString()) {
                            case "Template": {
                                    template = new Template() {
                                        Category = reader.GetAttribute("Category"),
                                        Name = reader.GetAttribute("Name"),
                                        ImageURL = reader.GetAttribute("ImageURL"),
                                        Title = reader.GetAttribute("Title"),
                                        Description = reader.GetAttribute("Description"),
                                        Dependencies = new List<File>()
                                    };
                                    if (Enum.TryParse(reader.GetAttribute("Platform"), out Platform currentPlatform))
                                        template.Platform = currentPlatform;
                                    
                                    break;
                                }
                            case "File": {
                                    File file = new File {
                                        Name = reader.GetAttribute("Name"),
                                        Folder = reader.GetAttribute("Folder")
                                    };
                                    template?.Dependencies?.Add(file);

                                    break;
                                }
                        }
                    } else if (!reader.IsStartElement()) {
                        switch (reader.Name.ToString()) {
                            case "Template": {
                                    if (template != null)
                                        templates.Add(template);
                                    break;
                                }
                        }
                    }
                }
            }
            return templates;
        }
    }
}
