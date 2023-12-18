using System.Collections.ObjectModel;
using System.Text.Json;

namespace LiVer;

public class Version
{
    public int Id { get; set; }
    public string CollectionDirPath { get; private set; }
    public bool RenderExists { get; private set; } = false;
    public Version? Prev { get; private set; }
    public List<Version> Next { get; set; } = new List<Version>();
    public ReadOnlyCollection<Version> NextReadOnly => Next.AsReadOnly();
    public string ChangeLog { get; set; } = string.Empty;
    public string CommendsForNext { get; set; } = string.Empty;

    
    // ** CONSTRUCTORS **
    // New first version
    public Version(string collectionPath, string sourceFilePath)
    {
        this.Id = 0;
        this.CollectionDirPath = collectionPath;
        this.Prev = null;

        FileHelper.CopyFile(sourceFilePath, GetFilePath());
    }
    // New version
    public Version(int id, string collectionPath, Version prev)
    {
        this.Id = id;
        this.CollectionDirPath = collectionPath;
        this.Prev = prev;

        FileHelper.CopyFile(prev.GetFilePath(), GetFilePath());
    }
    //  From VersionData
    private Version(SerializableVersion versionData, Version? prev, List<Version> next)
    {
        this.Id = versionData.id;
        this.CollectionDirPath = versionData.collectionPath;
        this.Prev = prev;
        this.Next = next;
        this.ChangeLog = versionData.changeLog;
        this.CommendsForNext = versionData.commentsForNext;

        if (!CheckFile()) throw new Exception("Version file does not exist");
        CheckRender();
    }


    // ** FILE & RENDER **
    public string GetFilePath()
    {
        return Path.Join(CollectionDirPath, Id.ToString(), FileHelper.AbletonLiveSetExtension);
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
        return Path.Join(CollectionDirPath, Id.ToString(), FileHelper.WaveFileExtension);
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
        int? prevId = (Prev != null) ? Prev.Id : null;
        int[] nextIds = Next.ConvertAll(n => n.Id).ToArray();
        return new SerializableVersion(Id, CollectionDirPath, prevId, nextIds, ChangeLog, CommendsForNext);
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