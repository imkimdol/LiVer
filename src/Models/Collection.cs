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

    #region Constructors
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
        Name = collectionData.Name;
        ProjectDirPath = collectionData.ProjectPath;
        _maxIndex = collectionData.MaxIndex;

        foreach (string sv in collectionData.SerializedVersions)
        {
            try
            {
                _versions.Add(Version.Deserialize(sv, this));
            }
            catch { }
        }
    }
    #endregion

    #region Version
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
    #endregion

    #region File
    private string GetDirectoryPath()
    {
        return Path.Combine(ProjectDirPath, Name);
    }
    public void DeleteDirectory()
    {
        FileHelper.DeleteDirectory(GetDirectoryPath());
    }
    #endregion

    #region Data
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
    #endregion
}

public class SerializableCollection
{
    public string Name { get; set; }
    public string ProjectPath { get; set; }
    public int MaxIndex { get; set; }
    public string[] SerializedVersions { get; set; }

    public SerializableCollection(string name, string projectPath, int maxIndex, string[] serializedVersions)
    {
        this.Name = name;
        this.ProjectPath = projectPath;
        this.MaxIndex = maxIndex;
        this.SerializedVersions = serializedVersions;
    }
}