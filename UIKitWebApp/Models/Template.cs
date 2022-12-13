namespace UIKitWebApp;

[Serializable]
public class Template {
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Title { get; set; } 
    public Platform? Platform { get; set; }
    public string? Category { get; set; }
    public string? ImageURL { get; set; }
    public List<File>? Dependencies { get; set; }
}
