using System.Collections.ObjectModel;

namespace LiVer;

public class Project
{
    public string name { get; private set; }
    public string dirPath { get; private set; }
    private List<Collection> collections { get; } = new List<Collection>();
    public ReadOnlyCollection<Collection> collectionReadOnly => collections.AsReadOnly();

    private Project(string name, string dirPath)
    {
        this.name = name;
        this.dirPath = dirPath;
    }

    public static Project CreateProject(string alsPath)
    {
        return null; // Stub
    }
    public static Project LoadProject(string lvrPath)
    {
        return null; // Stub
    }

    private void ReadProjectFile()
    {

    }
    public void SaveProjectFile()
    {

    }

    public Collection NewCollection(string projectName)
    {
        return null; //stub
    }
}
