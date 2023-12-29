using LiVer;
using System.IO;
using System.Text.Json;

namespace LiVer;

internal class MainInterface : UserInterface
{
    private List<string> _recentProjectPaths;
    private string _liverAppDataPath = @"%AppData%\LiVer";
    private string _recentsFilePath;

    public MainInterface()
    {
        _recentProjectPaths = new List<string>();
        _liverAppDataPath = Environment.ExpandEnvironmentVariables(_liverAppDataPath);
        _recentsFilePath = Path.Combine(_liverAppDataPath, "LiVer.json");

        if (!FileHelper.CheckFileExists(_recentsFilePath))
        {
            List<string> emptyList = new List<string>();
            FileHelper.CreateDirectory(_liverAppDataPath);
            SerializeAndSave(emptyList);
        }
        else
        {
            string json = FileHelper.ReadFile(_recentsFilePath);
            List<string>? data = JsonSerializer.Deserialize<List<string>>(json);
            if (data != null) _recentProjectPaths = data;
        }
    }
    

    protected override void PrintDisplay()
    {
        base.PrintDisplay();

        Console.WriteLine("     LiVer    ");
        Console.WriteLine("--------------");
        Console.WriteLine("[N]ew Project ");
        Console.WriteLine("[O]pen Project");
        Console.WriteLine("");
        Console.WriteLine("    Recents   ");
        Console.WriteLine("--------------");

        for (int i=0; i<_recentProjectPaths.Count; i++)
        {
            string str = (i+1).ToString() + ". ";
            str += _recentProjectPaths[i];
            Console.WriteLine(str);
        }
    }

    protected override void HandleKeyInput()
    {
        switch (LastKey)
        {
            case ConsoleKey.N:
                NewProject();
                break;
            case ConsoleKey.O:
                LoadProjectPrompt();
                break;
            case ConsoleKey.D1:
                if (_recentProjectPaths.Count > 0) LoadProject(_recentProjectPaths[0]);
                break;
            case ConsoleKey.D2:
                if (_recentProjectPaths.Count > 1) LoadProject(_recentProjectPaths[1]);
                break;
            case ConsoleKey.D3:
                if (_recentProjectPaths.Count > 2) LoadProject(_recentProjectPaths[2]);
                break;
            case ConsoleKey.D4:
                if (_recentProjectPaths.Count > 3) LoadProject(_recentProjectPaths[3]);
                break;
            case ConsoleKey.D5:
                if (_recentProjectPaths.Count > 4) LoadProject(_recentProjectPaths[4]);
                break;
            default:
                break;
        }
    }

    private void NewProject()
    {
        Console.WriteLine("New - Enter Path of Live Set:");
        var liveSetPath = Console.ReadLine();
        if (liveSetPath == null) return;

        var project = new Project(liveSetPath);
        AddToRecentPaths(project.GetProjectFilePath());

        ProjectInterface projectInterface = new ProjectInterface(project);
        projectInterface.Run();
    }

    private void LoadProject(string path)
    {
        AddToRecentPaths(path);

        var project = Project.LoadProject(path);
        ProjectInterface projectInterface = new ProjectInterface(project);

        projectInterface.Run();
    }

    private void LoadProjectPrompt()
    {
        Console.WriteLine("Open - Enter Path of Project File:");
        var projectFilePath = Console.ReadLine();
        if (projectFilePath == null) return;

        LoadProject(projectFilePath);
    }

    private void SerializeAndSave(List<string> data)
    {
        string json = JsonSerializer.Serialize(data, FileHelper.GetSerializerOptions());
        FileHelper.WriteFile(_recentsFilePath, json);
    }

    private void AddToRecentPaths(string path)
    {
        if (!_recentProjectPaths.Contains(path))
        {
            _recentProjectPaths.Add(path);
            if (_recentProjectPaths.Count > 5) _recentProjectPaths.RemoveAt(0);
        }

        SerializeAndSave(_recentProjectPaths);
    }
}
