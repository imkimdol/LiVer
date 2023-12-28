using System.Collections;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace LiVer;

public class Collection
{
    public string Name { get; private set; }
    public string ProjectDirPath { get; private set; }
    private int _maxIndex { get; set; }
    private List<Version> _versions { get; set; } = new List<Version>();
    public ReadOnlyCollection<Version> VersionsReadOnly => _versions.AsReadOnly();

    // ** CONSTRUCTORS **

    // New collection
    public Collection(string name, string projectPath, string sourceFilePath)
    {
        Name = name;
        ProjectDirPath = projectPath;
        _maxIndex = 0;

        FileHelper.CreateDirectory(GetDirectoryPath());

        Version version = new Version(GetDirectoryPath(), Name, sourceFilePath);
        _versions.Add(version);
    }
    // New collection from version
    public Collection(string name, string projectPath, Version prev)
    {
        Name = name;
        ProjectDirPath = projectPath;
        _maxIndex = 0;

        FileHelper.CreateDirectory(GetDirectoryPath());

        Version version = new Version(0, GetDirectoryPath(), Name, prev);
        _versions.Add(version);
    }
    // From CollectionData
    private Collection(SerializableCollection collectionData)
    {
        Name = collectionData.name;
        ProjectDirPath = collectionData.projectPath;
        _maxIndex = collectionData.maxIndex;

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
        Version version = new Version(_maxIndex + 1, GetDirectoryPath(), Name, source);
        _versions.Add(version);
        _maxIndex++;

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
    }


    // ** FILE **
    private string GetDirectoryPath()
    {
        return Path.Combine(ProjectDirPath, Name);
    }
    public void DeleteDirectory()
    {
        FileHelper.DeleteDirectory(GetDirectoryPath());
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
        return new SerializableCollection(Name, ProjectDirPath, _maxIndex, serializedVersions.ToArray());
    }
}

public class SerializableCollection
{
    public string name { get; set; }
    public string projectPath { get; set; }
    public int maxIndex { get; set; }
    public string[] serializedVersions { get; set; }

    public SerializableCollection(string name, string projectPath, int maxIndex, string[] serializedVersions)
    {
        this.name = name;
        this.projectPath = projectPath;
        this.maxIndex = maxIndex;
        this.serializedVersions = serializedVersions;
    }
}