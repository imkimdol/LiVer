﻿using System.Collections;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace LiVer;

public class Version
{
    public int Id { get; set; }
    public string CollectionDirPath { get; private set; }
    public string CollectionName { get; private set; }
    public bool RenderExists { get; private set; } = false;
    public Version? Prev { get; private set; }
    public List<Version> Next { get; set; } = new List<Version>();
    public ReadOnlyCollection<Version> NextReadOnly => Next.AsReadOnly();
    public string ChangeLog { get; set; } = string.Empty;
    public string CommentsForNext { get; set; } = string.Empty;

    #region Constructors
    // New first version
    public Version(string collectionPath, string collectionName, string sourceFilePath)
    {
        Id = 0;
        CollectionDirPath = collectionPath;
        CollectionName = collectionName;
        Prev = null;

        FileHelper.CopyFile(sourceFilePath, GetFilePath());
    }
    // New version
    public Version(int id, string collectionPath, string collectionName, Version prev)
    {
        Id = id;
        CollectionDirPath = collectionPath;
        CollectionName = collectionName;
        Prev = prev;

        prev.Next.Add(this);

        FileHelper.CopyFile(prev.GetFilePath(), GetFilePath());
    }
    //  From VersionData
    private Version(SerializableVersion versionData, Version? prev, List<Version> next)
    {
        Id = versionData.Id;
        CollectionDirPath = versionData.CollectionPath;
        CollectionName = versionData.CollectionName;
        Prev = prev;
        Next = next;
        ChangeLog = versionData.ChangeLog;
        CommentsForNext = versionData.CommentsForNext;

        if (!CheckFile()) throw new Exception("Version file does not exist");
        CheckRender();
    }
    #endregion

    #region File & Render
    public string GetFilePath()
    {
        return Path.Join(CollectionDirPath, Id.ToString() + FileHelper.AbletonLiveSetExtension);
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
        return Path.Join(CollectionDirPath, Id.ToString() + FileHelper.WaveFileExtension);
    }
    public bool CheckRender()
    {
        return FileHelper.CheckFileExists(GetRenderPath());
    }
    public void PlayRender()
    {
        FileHelper.OpenFile(GetRenderPath());
    }
    #endregion

    #region Data
    public string Serialize()
    {
        SerializableVersion data = ConvertToSerializable();
        return JsonSerializer.Serialize(data, FileHelper.GetSerializerOptions());
    }
    public static Version Deserialize(string data, Collection collection)
    {
        SerializableVersion? deserialized = JsonSerializer.Deserialize<SerializableVersion>(data);
        if (deserialized == null) { throw new Exception("Invalid Version data"); }

        int? prevId = deserialized!.PrevId;
        Version? prev = prevId != null ? collection.FindVersion((int)prevId) : null;

        List<Version> versions = new List<Version>();
        foreach (int id in deserialized.NextIds)
        {
            Version? version = collection.FindVersion(id);
            if (version != null) versions.Add(version);
        }

        return new Version(deserialized, prev, versions);
    }
    private SerializableVersion ConvertToSerializable()
    {
        int? prevId = Prev != null ? Prev.Id : null;
        int[] nextIds = Next.ConvertAll(n => n.Id).ToArray();
        return new SerializableVersion(Id, CollectionDirPath, CollectionName, prevId, nextIds, ChangeLog, CommentsForNext);
    }
    #endregion
}

public class SerializableVersion
{
    public int Id { get; set; }
    public string CollectionPath { get; set; }
    public string CollectionName { get; set; }
    public int? PrevId { get; set; }
    public int[] NextIds { get; set; }
    public string ChangeLog { get; set; }
    public string CommentsForNext { get; set; }

    public SerializableVersion(int id, string collectionPath, string collectionName, int? prevId, int[] nextIds, string changeLog, string commentsForNext)
    {
        this.Id = id;
        this.CollectionPath = collectionPath;
        this.CollectionName = collectionName;
        this.PrevId = prevId;
        this.NextIds = nextIds;
        this.ChangeLog = changeLog;
        this.CommentsForNext = commentsForNext;
    }
}