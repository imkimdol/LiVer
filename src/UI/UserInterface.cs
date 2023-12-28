namespace LiVer;

internal abstract class UserInterface
{
    protected int ScreenWidth;
    protected int ScreenHeight;
    protected bool ConsoleIsTooSmall;

    protected bool IsExiting = false;
    protected ConsoleKey? LastKey;


    #region Public
    public void Run()
    {
        while (!IsExiting) Iterate();
    }
    #endregion
    

    #region Abstract Class
    protected virtual void Iterate()
    {
        GetScreenDimensions();
        PrintDisplay();
        AwaitKeyInput();
        HandleKeyInput();
    }
    protected abstract void HandleKeyInput();
    protected virtual void PrintDisplay()
    {
        Console.Clear();
    }
    protected ConsoleKey ReadKey()
    {
        ConsoleKey key = Console.ReadKey(true).Key;
        if (key == ConsoleKey.Escape) IsExiting = true;
        return key;
    }
    #endregion


    #region Helpers
    private void AwaitKeyInput()
    {
        LastKey = ReadKey();
        if (IsExiting) return;
    }
    private void GetScreenDimensions()
    {
        ScreenWidth = Console.WindowWidth;
        ScreenHeight = Console.WindowHeight - 1;

        if (ScreenWidth % 2 == 0) ScreenWidth--;

        if (ScreenWidth < 40 || ScreenHeight < 10) ConsoleIsTooSmall = true;
    }
    #endregion
}
