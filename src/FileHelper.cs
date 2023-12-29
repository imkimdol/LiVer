using System.Diagnostics;
using System.Text.Json;

namespace LiVer;

public static class FileHelper
{
    public const string ProjectDirectoryName = "LiVer";
    public const string ProjectFileExtension = ".lvr";
    public const string AbletonLiveSetExtension = ".als";
    public const string WaveFileExtension = ".wav";

    public static void CreateDirectory(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }
    public static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path)) Directory.Delete(path, true);
    }
    public static string ReadFile(string path)
    {
        return File.ReadAllText(path);
    }
    public static void WriteFile(string path, string content)
    {
        File.WriteAllText(path, content);
    }
    public static void DeleteFile(string path)
    {
        File.Delete(path);
    }
    public static void CopyFile(string source, string destination)
    {
        File.Copy(source, destination, true);
    }
    public static void OpenFile(string path)
    {
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = path;
        psi.UseShellExecute = true;
        Process.Start(psi);
    }
    public static bool CheckFileExists(string path)
    {
        return File.Exists(path);
    }
    public static JsonSerializerOptions GetSerializerOptions()
    {
        return new JsonSerializerOptions { WriteIndented = true };
    }
}
