using System.Collections.ObjectModel;
using System.Text.Json;

namespace LiVer;

public class Collection
{
    public string name { get; private set; }
    public string projectDirPath { get; private set; }
    private int maxIndex { get; set; }
    private List<Version> versions { get; } = new List<Version>();
    public ReadOnlyCollection<Version> versionsReadOnly => versions.AsReadOnly();

    // ** CONSTRUCTORS **
    // New collection
    public Collection(string name, string projectPath, string sourceFilePath)
    {
        this.name = name;
        this.projectDirPath = projectPath;
        this.maxIndex = 0;

        FileHelper.CreateDirectory(GetDirectoryPath());

        Version version = new Version(GetDirectoryPath(), sourceFilePath);
        versions.Add(version);
    }
    // From CollectionData
    private Collection(SerializableCollection collectionData)
    {
        name = collectionData.name;
        projectDirPath = collectionData.projectPath;
        maxIndex = collectionData.maxIndex;

        foreach (string sv in collectionData.serializedVersions)
        {
            try
            {
                versions.Add(Version.Deserialize(sv, this));
            }
            catch { }
        }
    }


    // ** VERSION **
    public Version NewVersion(Version source)
    {
        Version version = new Version(maxIndex + 1, GetDirectoryPath(), source);
        versions.Add(version);
        maxIndex++;

        return version;
    }
    public void DeleteVersion(Version version)
    {
        version.DeleteFile();
        versions.Remove(version);
    }
    public Version? FindVersion(int id)
    {
        return versions.Find(v => v.id == id);
        /*foreach (Version v in versions)
        {
            if (v.id == id) return v;
        }
        return null;*/
    }


    // ** FILE **
    private string GetDirectoryPath()
    {
        return Path.Combine(projectDirPath, name);
    }
    public void DeleteDirectory()
    {
        FileHelper.DeleteDirectory(projectDirPath);
    }


    // ** DATA **
    public string Serialize()
    {
        SerializableCollection data = ConvertToSerializable();
        return JsonSerializer.Serialize(data, FileHelper.GetSerializerOptions());
    }
    public static Collection Deserialize(string data)
    {
        SerializableCollection? deserialized = JsonSerializer.Deserialize<SerializableCollection>(data);
        if (deserialized == null) { throw new Exception("Invalid Collection data"); }
        return new Collection(deserialized!);
    }
    private SerializableCollection ConvertToSerializable()
    {
        List<string> serializedVersions = new List<string>();
        foreach (Version v in versions)
        {
            serializedVersions.Add(v.Serialize());
        }
        return new SerializableCollection(name, projectDirPath, maxIndex, serializedVersions.ToArray());
    }
}

public class SerializableCollection
{
    public string name;
    public string projectPath;
    public int maxIndex;
    public string[] serializedVersions;
    
    public SerializableCollection(string name, string projectPath, int maxIndex, string[] serializedVersions)
    {
        this.name = name;
        this.projectPath = projectPath;
        this.maxIndex = maxIndex;
        this.serializedVersions = serializedVersions;
    }
}