using System.Collections.ObjectModel;
using System.Text.Json;

namespace LiVer;

public class Project
{
    public string name { get; set; }
    public string dirPath { get; private set; }
    private List<Collection> collections { get; } = new List<Collection>();
    public ReadOnlyCollection<Collection> collectionReadOnly => collections.AsReadOnly();

    // ** CONSTRUCTORS **
    // New collection
    public Project(string liveSetPath)
    {
        if (!Path.GetExtension(liveSetPath).Equals(FileHelper.abletonLiveSetExtension)) throw new Exception();
        name = Path.GetFileNameWithoutExtension(liveSetPath);

        dirPath = Path.Combine(liveSetPath, FileHelper.projectDirectoryName);
        FileHelper.CreateDirectory(dirPath);
        
        SaveProjectFile();
    }
    // From ProjectData
    private Project(SerializableProject projectData)
    {
        name = projectData.name;
        dirPath = projectData.dirPath;
        foreach (string sc in projectData.serializedCollections)
        {
            try
            {
                collections.Add(Collection.Deserialize(sc));
            }
            catch {}
        }
    }


    // ** COLLECTION **
    public Collection NewCollection(string collectionName, Version source)
    {
        Collection collection = new Collection(collectionName, dirPath, source.GetFilePath());
        collections.Add(collection);
        return collection;
    }
    public void DeleteCollection(Collection collection)
    {
        collection.DeleteDirectory();
        collections.Remove(collection);
    }

    // ** FILE **
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
        return Path.Combine(dirPath, name, FileHelper.projectFileExtension);
    }


    // ** DATA **
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
        foreach (Collection c in collections)
        {
            serializedCollections.Add(c.Serialize());
        }
        return new SerializableProject(name, dirPath, serializedCollections.ToArray());
    }
}

public class SerializableProject
{
    public string name;
    public string dirPath;
    public string[] serializedCollections;

    public SerializableProject(string name, string dirpath, string[] serializedCollections)
    {
        this.name = name;
        this.dirPath = dirpath;
        this.serializedCollections = serializedCollections;
    }
}