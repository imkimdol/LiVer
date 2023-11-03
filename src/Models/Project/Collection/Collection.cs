using System.Collections.ObjectModel;

namespace LiVer;

public class Collection
{
    public string name { get; private set; }
    private List<Version> versions { get; } = new List<Version>();
    public ReadOnlyCollection<Version> versionsReadOnly => versions.AsReadOnly();

    // New project
    public Collection(string liverDirPath, string name, string originalFilePath)
    {

    }
    // New collection from version
    public Collection(string liverDirPath, string name, Version sourceVersion)
    {

    }
    // Load collection
    public Collection(string collectionDirPath)
    {

    }

    public Version NewVersion(string versionName)
    {
        return null; //stub
    }
}
