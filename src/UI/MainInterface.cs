using LiVer;

namespace LiVer;

internal class MainInterface : UserInterface
{
    public MainInterface() { }

    protected override void PrintDisplay()
    {
        base.PrintDisplay();

        Console.WriteLine("     LiVer    ");
        Console.WriteLine("--------------");
        Console.WriteLine("[N]ew Project ");
        Console.WriteLine("[O]pen Project");
    }

    protected override void HandleKeyInput()
    {
        switch (LastKey)
        {
            case ConsoleKey.N:
                NewProject();
                break;
            case ConsoleKey.O:
                LoadProject();
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
        ProjectInterface projectInterface = new ProjectInterface(project);

        projectInterface.Run();
    }

    private void LoadProject()
    {
        Console.WriteLine("Open - Enter Path of Project File:");
        var projectFilePath = Console.ReadLine();
        if (projectFilePath == null) return;

        var project = Project.LoadProject(projectFilePath);
        ProjectInterface projectInterface = new ProjectInterface(project);

        projectInterface.Run();
    }

}
