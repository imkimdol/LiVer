using System.Collections.ObjectModel;
using System.Text.Json;

namespace LiVer;

public class Version
{
    public int id { get; set; }
    public string collectionDirPath { get; private set; }
    public bool renderExists { get; private set; } = false;
    public Version? prev { get; private set; }
    public List<Version> next { get; set; } = new List<Version>();
    public ReadOnlyCollection<Version> nextReadOnly => next.AsReadOnly();
    public string changeLog { get; set; } = string.Empty;
    public string commentsForNext { get; set; } = string.Empty;

    
    // ** CONSTRUCTORS **
    // New first version
    public Version(string collectionPath, string sourceFilePath)
    {
        this.id = 0;
        this.collectionDirPath = collectionPath;
        this.prev = null;

        FileHelper.CopyFile(sourceFilePath, GetFilePath());
    }
    // New version
    public Version(int id, string collectionPath, Version prev)
    {
        this.id = id;
        this.collectionDirPath = collectionPath;
        this.prev = prev;

        FileHelper.CopyFile(prev.GetFilePath(), GetFilePath());
    }
    //  From VersionData
    private Version(SerializableVersion versionData, Version? prev, List<Version> next)
    {
        this.id = versionData.id;
        this.collectionDirPath = versionData.collectionPath;
        this.prev = prev;
        this.next = next;
        this.changeLog = versionData.changeLog;
        this.commentsForNext = versionData.commentsForNext;

        if (!CheckFile()) throw new Exception("Version file does not exist");
        CheckRender();
    }


    // ** FILE & RENDER **
    public string GetFilePath()
    {
        return Path.Join(collectionDirPath, id.ToString(), FileHelper.abletonLiveSetExtension);
    }
    private bool CheckFile()
    {
        return FileHelper.CheckFileExists(GetFilePath());
    }
    public void OpenFile()
    {
        FileHelper.OpenFile(GetFilePath());
    }
    public void DeleteFile()
    {
        FileHelper.DeleteFile(GetFilePath());
    }
    public string GetRenderPath()
    {
        return Path.Join(collectionDirPath, id.ToString(), FileHelper.waveFileExtension);
    }
    public bool CheckRender()
    {
        return FileHelper.CheckFileExists(GetRenderPath());
    }
    public void PlayRender()
    {
        FileHelper.OpenFile(GetRenderPath());
    }


    // ** DATA **
    public string Serialize()
    {
        SerializableVersion data = ConvertToSerializable();
        return JsonSerializer.Serialize(data, FileHelper.GetSerializerOptions());
    }
    public static Version Deserialize(string data, Collection collection)
    {
        SerializableVersion? deserialized = JsonSerializer.Deserialize<SerializableVersion>(data);
        if (deserialized == null) { throw new Exception("Invalid Version data"); }

        int? prevId = deserialized!.prevId;
        Version? prev = (prevId != null) ? collection.FindVersion((int) prevId) : null;

        List<Version> versions = new List<Version>();
        foreach (int id in deserialized.nextIds)
        {
            Version? version = collection.FindVersion(id);
            if (version != null) versions.Add(version);
        }

        return new Version(deserialized, prev, versions);
    }
    private SerializableVersion ConvertToSerializable()
    {
        int? prevId = (prev != null) ? prev.id : null;
        int[] nextIds = next.ConvertAll(n => n.id).ToArray();
        return new SerializableVersion(id, collectionDirPath, prevId, nextIds, changeLog, commentsForNext);
    } 
}

public class SerializableVersion
{
    public int id;
    public string collectionPath;
    public int? prevId;
    public int[] nextIds;
    public string changeLog;
    public string commentsForNext;

    public SerializableVersion(int id, string collectionPath, int? prevId, int[] nextIds, string changeLog, string commentsForNext)
    {
        this.id = id;
        this.collectionPath = collectionPath;
        this.prevId = prevId;
        this.nextIds = nextIds;
        this.changeLog = changeLog;
        this.commentsForNext = commentsForNext;
    }
}