using System.Collections.ObjectModel;
using System.Text.Json;

namespace LiVer;

public class Collection
{
    public string Name { get; private set; }
    public string ProjectDirPath { get; private set; }
    private int MaxIndex { get; set; }
    private List<Version> _versions { get; } = new List<Version>();
    public ReadOnlyCollection<Version> VersionsReadOnly => _versions.AsReadOnly();

    // ** CONSTRUCTORS **
    // New collection
    public Collection(string name, string projectPath, string sourceFilePath)
    {
        this.Name = name;
        this.ProjectDirPath = projectPath;
        this.MaxIndex = 0;

        FileHelper.CreateDirectory(GetDirectoryPath());

        Version version = new Version(GetDirectoryPath(), sourceFilePath);
        _versions.Add(version);
    }
    // From CollectionData
    private Collection(SerializableCollection collectionData)
    {
        Name = collectionData.name;
        ProjectDirPath = collectionData.projectPath;
        MaxIndex = collectionData.maxIndex;

        foreach (string sv in collectionData.serializedVersions)
        {
            try
            {
                _versions.Add(Version.Deserialize(sv, this));
            }
            catch { }
        }
    }


    // ** VERSION **
    public Version NewVersion(Version source)
    {
        Version version = new Version(MaxIndex + 1, GetDirectoryPath(), source);
        _versions.Add(version);
        MaxIndex++;

        return version;
    }
    public void DeleteVersion(Version version)
    {
        version.DeleteFile();
        _versions.Remove(version);
    }
    public Version? FindVersion(int id)
    {
        return _versions.Find(v => v.Id == id);
        /*foreach (Version v in versions)
        {
            if (v.id == id) return v;
        }
        return null;*/
    }


    // ** FILE **
    private string GetDirectoryPath()
    {
        return Path.Combine(ProjectDirPath, Name);
    }
    public void DeleteDirectory()
    {
        FileHelper.DeleteDirectory(ProjectDirPath);
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
        foreach (Version v in _versions)
        {
            serializedVersions.Add(v.Serialize());
        }
        return new SerializableCollection(Name, ProjectDirPath, MaxIndex, serializedVersions.ToArray());
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