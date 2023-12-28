using System.Collections.ObjectModel;
using System.Text.Json;

namespace LiVer;

public class Project
{
    public string Name { get; set; }
    public string DirectoryPath { get; private set; }
    private List<Collection> _collections { get; } = new List<Collection>();
    public ReadOnlyCollection<Collection> CollectionsReadOnly => _collections.AsReadOnly();

    #region Constructors
    // New collection
    public Project(string liveSetPath)
    {
        if (!Path.GetExtension(liveSetPath).Equals(FileHelper.AbletonLiveSetExtension)) throw new Exception();
        Name = Path.GetFileNameWithoutExtension(liveSetPath);

        string liveSetDirectory = Path.GetDirectoryName(liveSetPath);
        DirectoryPath = Path.Combine(liveSetDirectory, FileHelper.ProjectDirectoryName);
        FileHelper.CreateDirectory(DirectoryPath);

        NewCollection(liveSetPath);

        SaveProjectFile();
    }
    // From ProjectData
    private Project(SerializableProject projectData)
    {
        Name = projectData.name;
        DirectoryPath = projectData.dirPath;
        foreach (string sc in projectData.serializedCollections)
        {
            try
            {
                _collections.Add(Collection.Deserialize(sc));
            }
            catch { }
        }
    }
    #endregion

    #region Collection
    public Collection NewCollection(string sourcePath)
    {
        Collection collection = new Collection("Untitled", DirectoryPath, sourcePath);
        _collections.Add(collection);
        return collection;
    }
    public Collection NewCollection(string collectionName, Version source)
    {
        Collection collection = new Collection(collectionName, DirectoryPath, source);
        _collections.Add(collection);
        return collection;
    }
    public void DeleteCollection(Collection collection)
    {
        collection.DeleteDirectory();
        _collections.Remove(collection);
    }
    #endregion

    #region File
    public static Project LoadProject(string projectFilePath)
    {
        string serializedData = FileHelper.ReadFile(projectFilePath);
        return Deserialize(serializedData);
    }
    public void SaveProjectFile()
    {
        FileHelper.WriteFile(GetProjectFilePath(), Serialize());
    }
    private string GetProjectFilePath()
    {
        return Path.Combine(DirectoryPath, Name + FileHelper.ProjectFileExtension);
    }
    #endregion

    #region Data
    private string Serialize()
    {
        SerializableProject data = ConvertToSerializable();
        return JsonSerializer.Serialize(data, FileHelper.GetSerializerOptions());
    }
    private static Project Deserialize(string data)
    {
        SerializableProject? deserialized = JsonSerializer.Deserialize<SerializableProject>(data);
        if (deserialized == null) { throw new Exception("Invalid Project data"); }
        return new Project(deserialized!);
    }
    private SerializableProject ConvertToSerializable()
    {
        List<string> serializedCollections = new List<string>();
        foreach (Collection c in _collections)
        {
            serializedCollections.Add(c.Serialize());
        }
        return new SerializableProject(Name, DirectoryPath, serializedCollections.ToArray());
    }
    #endregion
}

public class SerializableProject
{
    public string name { get; set; }
    public string dirPath { get; set; }
    public string[] serializedCollections { get; set; }

    public SerializableProject(string name, string dirpath, string[] serializedCollections)
    {
        this.name = name;
        dirPath = dirpath;
        this.serializedCollections = serializedCollections;
    }
}