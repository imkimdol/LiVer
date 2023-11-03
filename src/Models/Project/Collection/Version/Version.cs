namespace LiVer;

public class Version
{
    public int id { get; set; }
    public string collectionPath { get; private set; }
    public bool renderExists { get; private set; } = false;
    public Version? prev { get; private set; }
    public Version? next { get; set; }
    public string changeLog { get; set; } = string.Empty;
    public string commentsForNext { get; set; } = string.Empty;

    // New first version
    Version(string collectionPath)
    {
        this.id = 0;
        this.collectionPath = collectionPath;
        this.prev = null;
        this.next = null;
    }
    // New version
    Version(string collectionPath, int id, Version prev)
    {
        this.id = id;
        this.collectionPath = collectionPath;
        this.prev = prev;
        this.next = null;
    }
    // Load version from file
    Version(string collectionPath, int id, Version prev, Version next, string changeLog, string commentsForNext)
    {
        this.id = id;
        this.collectionPath = collectionPath;
        this.prev = prev;
        this.next = next;
        this.changeLog = changeLog;
        this.commentsForNext = commentsForNext;

        if (!CheckFile()) throw new Exception("Version file does not exist");
        CheckRender();
    }

    public bool CheckFile()
    {
        return false; //stub
    }
    public void OpenFile()
    {

    }
    public void PlayRender()
    {

    }
    public bool CheckRender()
    {
        return false; //stub
    }


}
