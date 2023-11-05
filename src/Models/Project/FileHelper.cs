using System.Diagnostics;
using System.Text.Json;

namespace LiVer;

public static class FileHelper
{
    public const string projectDirectoryName = "LiVer";
    public const string projectFileExtension = ".lvr";
    public const string abletonLiveSetExtension = ".als";
    public const string waveFileExtension = ".wav";

    public static void CreateDirectory(string path)
    {
        try
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }
        catch (Exception e)
        {
            ShowErrorMessage(e.Message);
        }
    }
    public static void DeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path)) Directory.Delete(path, true);
        }
        catch (Exception e)
        {
            ShowErrorMessage(e.Message);
        }
    }
    public static string ReadFile(string path)
    {
        try
        {
            return File.ReadAllText(path);
        }
        catch (Exception e)
        {
            ShowErrorMessage(e.Message);
            return string.Empty;
        }
    }
    public static void WriteFile(string path, string content)
    {
        try
        {
            File.WriteAllText(path, content);
        }
        catch (Exception e)
        {
            ShowErrorMessage(e.Message);
        }
    }
    public static void DeleteFile (string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception e)
        {
            ShowErrorMessage(e.Message);
        }
    }
    public static void CopyFile(string source, string destination)
    {
        try
        {
            File.Copy(source, destination, true);
        }
        catch (Exception e)
        {
            ShowErrorMessage(e.Message);
        }
    }
    public static void OpenFile(string path)
    {
        try
        {
            Process.Start(path);
        }
        catch (Exception e)
        {
            ShowErrorMessage(e.Message);
        }
        
    }
    public static bool CheckFileExists(string path)
    {
        return File.Exists(path);
    }
    public static JsonSerializerOptions GetSerializerOptions()
    {
        return new JsonSerializerOptions { WriteIndented = true };
    }

    private static void ShowErrorMessage(string message)
    {
        MessageBox.Show(message);
    }
}
