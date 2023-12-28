using System;
using System.Collections;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Transactions;
using static System.Net.Mime.MediaTypeNames;

namespace LiVer;

internal class ProjectInterface : UserInterface
{
    private Project _project;
    private int _collectionIndex;
    private int _versionIndex;
    private int _collectionsSectionIndex;

    private const int _collectionsSectionWidth = 18;
    private const int _collectionsSectionNameMaxWidth = _collectionsSectionWidth - 6;
    private const int _versionInfoSectionSetTextHeight = 9;


    public ProjectInterface(Project project)
    {
        _project = project;
        _collectionIndex = 0;
        _versionIndex = 0;
        _collectionsSectionIndex = 0;
    }

    protected override void PrintDisplay()
    {
        base.PrintDisplay();

        if (ConsoleIsTooSmall)
        {
            Console.WriteLine("Console is too small!");
            return;
        }

        PrintTopBar();
        PrintMiddleSection();
        PrintBottomBar();
        PrintInstructions();
    }
    protected override void HandleKeyInput()
    {
        switch (LastKey)
        {
            case ConsoleKey.N:
                HandleInputNew();
                break;
            case ConsoleKey.D:
                HandleInputDelete();
                break;
            case ConsoleKey.O:
                HandleInputOpen();
                break;
            case ConsoleKey.L:
                HandleInputChangelog();
                break;
            case ConsoleKey.C:
                HandleInputCommentsForNext();
                break;
            case ConsoleKey.UpArrow:
                DecreaseVersionIndex();
                break;
            case ConsoleKey.DownArrow:
                IncreaseVersionIndex();
                break;
            default:
                break;
        }
    }


    private void HandleInputNew()
    {
        Console.WriteLine("[C]ollection, [V]ersion");
        ConsoleKey key = Console.ReadKey(true).Key;

        string name;
        switch (key)
        {
            case ConsoleKey.C:
                Console.WriteLine("Enter collection name:");
                name = Console.ReadLine();

                _project.NewCollection(name, GetVersion());

                break;
            case ConsoleKey.V:
                GetCollection().NewVersion(GetVersion());

                break;
            default:
                break;
        }

        _project.SaveProjectFile();
    }
    private void HandleInputDelete()
    {
        Console.WriteLine("[C]ollection, [V]ersion");
        ConsoleKey key = Console.ReadKey(true).Key;

        string name;
        switch (key)
        {
            case ConsoleKey.C:
                if (_project.CollectionsReadOnly.Count == 1) return;

                _project.DeleteCollection(GetCollection());
                DecreaseCollectionIndex();
                _versionIndex = 0;

                break;
            case ConsoleKey.V:
                if (GetCollection().VersionsReadOnly.Count == 1) return;

                GetCollection().DeleteVersion(GetVersion());
                DecreaseVersionIndex();

                break;
            default:
                break;
        }

        _project.SaveProjectFile();
    }
    private void HandleInputOpen()
    {
        GetVersion().OpenFile();
    }
    private void HandleInputChangelog()
    {
        Console.WriteLine("Enter new changelog:");
        var changelog = Console.ReadLine();
        if (changelog == null) return;

        GetVersion().ChangeLog = changelog;
    }
    private void HandleInputCommentsForNext()
    {
        Console.WriteLine("Enter new comments:");
        var comments = Console.ReadLine();
        if (comments == null) return;

        GetVersion().CommentsForNext = comments;
    }


    private void IncreaseVersionIndex()
    {
        _versionIndex += 1;
        if (_versionIndex >= GetCollection().VersionsReadOnly.Count)
        {
            _versionIndex = 0;
            IncreaseCollectionIndex();
        }
    }
    private void IncreaseCollectionIndex()
    {
        _collectionIndex += 1;
        if (_collectionIndex >= _project.CollectionsReadOnly.Count) _collectionIndex = 0;
    }
    private void DecreaseVersionIndex()
    {
        _versionIndex -= 1;
        if (_versionIndex <= -1)
        {
            DecreaseCollectionIndex();
            _versionIndex = GetCollection().VersionsReadOnly.Count - 1;
        }
    }
    private void DecreaseCollectionIndex()
    {
        _collectionIndex -= 1;
        if (_collectionIndex <= -1) _collectionIndex = _project.CollectionsReadOnly.Count - 1;
    }

    private Collection GetCollection()
    {
        return _project.CollectionsReadOnly[_collectionIndex];
    }
    private Version GetVersion()
    {
        return _project.CollectionsReadOnly[_collectionIndex].VersionsReadOnly[_versionIndex];
    }


    private void PrintTopBar()
    {
        StringBuilder line = new StringBuilder();
        int starLength = ScreenWidth / 2 - 3;

        line.Append(BuildStars(starLength));
        line.Append(" LiVer ");
        line.Append(BuildStars(starLength));

        Console.WriteLine(line.ToString());
    }

    private void PrintBottomBar()
    {
        Console.WriteLine(BuildStars(ScreenWidth));
    }
    private void PrintInstructions()
    {
        Console.WriteLine("[N]ew, [D]elete, [O]pen Version, Change[l]og, [C]omments for Next");
    }

    private string BuildStars(int length)
    {
        StringBuilder stars = new StringBuilder();

        for (int i = 0; i < length; i++)
        {
            stars.Append('*');
        }

        return stars.ToString();
    }

    private void PrintMiddleSection()
    {
        var collections = BuildCollectionsSection();
        var versionInfo = BuildVersionInfoSection();

        for (int i = 0; i < collections.Count; i++)
        {
            Console.WriteLine(collections[i] + versionInfo[i]);
        }
    }

    // TODO MAKE THIS LESS CLUTTERED
    private List<string> BuildCollectionsSection()
    {
        var section = new List<string>();
        var collections = _project.CollectionsReadOnly;

        foreach (Collection collection in collections)
        {
            var top = BuildCollectionsSectionTop(collection.Name);
            section.Add(top);

            int count = collection.VersionsReadOnly.Count;
            for (int i=0; i<count; i++)
            {
                var version = collection.VersionsReadOnly[i];
                var line = new StringBuilder();

                line.Append("│ ");
                if (collection == GetCollection() && i == _versionIndex)
                {
                    line.Append(">");
                }
                else
                {
                    line.Append(" ");
                }
                line.Append(" Version ");

                var idString = version.Id.ToString();
                var idMaxLength = 4;

                if (idString.Length > idMaxLength)
                {
                    line.Append(idString.Substring(0, idMaxLength));
                }
                else
                {
                    line.Append(idString);
                    var spacesLength = idMaxLength - idString.Length;
                    for (int j = 0; j < spacesLength; j++)
                    {
                        line.Append(" ");
                    }
                }
                line.Append(" │");

                section.Add(line.ToString());
            }

            var bottom = BuildCollectionsSectionBottom();
            section.Add(bottom);
        }

        for (int i=0; i<ScreenHeight-2; i++)
        {
            string spaces = "";
            for (int j=0; j<_collectionsSectionWidth; j++)
            {
                spaces += " ";
            }
            section.Add(spaces);
        }

        var subList = section.GetRange(_collectionsSectionIndex, ScreenHeight-3);
        return subList;
    }

    private string BuildCollectionsSectionTop(string collectionName)
    {
        var top = new StringBuilder();
        var leftPiece = "┌─ ";
        var rightPiece = "┐";
        var maxWidthConnector = " ─";
        var space = " ";
        var bar = "─";

        int lengthWithoutName = 0;
        lengthWithoutName += rightPiece.Length;

        top.Append(leftPiece);
        lengthWithoutName += leftPiece.Length;
        
        if (collectionName.Length > _collectionsSectionNameMaxWidth)
        {
            top.Append(collectionName.Substring(0, _collectionsSectionNameMaxWidth));
            top.Append(maxWidthConnector);
        }
        else
        {
            top.Append(collectionName);
            top.Append(space);
            lengthWithoutName += collectionName.Length;
            lengthWithoutName += space.Length;

            var barsLength = _collectionsSectionWidth - lengthWithoutName;
            for (int i=0; i < barsLength; i++)
            {
                top.Append(bar);
            }
            
        }

        top.Append(rightPiece);

        return top.ToString();
    }

    private string BuildCollectionsSectionBottom()
    {
        return "└────────────────┘";
    }

    private List<string> BuildVersionInfoSection()
    {
        var section = new List<string>();
        int width = ScreenWidth - _collectionsSectionWidth;
        int innerWidth = width - 4;
        int height = ScreenHeight - 2;
        int innerHeight = height - 4;
        int textSectionsHeight = (innerHeight - _versionInfoSectionSetTextHeight) / 2;

        var collection = _project.CollectionsReadOnly[_collectionIndex];
        var version = collection.VersionsReadOnly[_versionIndex];

        section.Add(BuildVersionInfoTopOrBottom(width, true));

        section.Add(BuildVersionInfoInnerHeader("Prev:", width));
        var prev = version.Prev;
        string prevString = (prev == null) ? string.Empty : prev.CollectionName + " Version " + prev.Id.ToString()!;
        section.Add(BuildVersionInfoInnerHeader(prevString, width));
        section.Add(BuildVersionInfoInnerHeader(" ", width));

        section.Add(BuildVersionInfoInnerHeader("Changelog:", width));
        var changeLogStrings = BuildVersionInfoInnerText(version.ChangeLog, width, textSectionsHeight);
        foreach (var changeLogString in changeLogStrings)
        {
            section.Add(changeLogString);
        }
        section.Add(BuildVersionInfoInnerHeader(" ", width));

        section.Add(BuildVersionInfoInnerHeader("Next:", width));
        var nexts = version.Next;
        string nextsString = "";
        for (int i=0; i<nexts.Count-1; i++)
        {
            var next = nexts[i];
            nextsString += next.CollectionName + " Version " + next.Id.ToString() + ", ";
        }
        if (nexts.Count > 0)
        {
            var last = nexts[nexts.Count - 1];
            nextsString += last.CollectionName + " Version " + last.Id.ToString();
        }
        if (nextsString.Length > innerWidth) nextsString = nextsString.Substring(0, innerWidth);
        section.Add(BuildVersionInfoInnerHeader(nextsString, width));
        section.Add(BuildVersionInfoInnerHeader(" ", width));

        section.Add(BuildVersionInfoInnerHeader("Comments for Next:", width));
        var commentsForNextStrings = BuildVersionInfoInnerText(version.CommentsForNext, width, textSectionsHeight);
        foreach (var commentsForNextString in commentsForNextStrings)
        {
            section.Add(commentsForNextString);
        }
        section.Add(BuildVersionInfoInnerHeader(" ", width));

        section.Add(BuildVersionInfoTopOrBottom(width, false));

        if (section.Count < height)
        {
            section.Add(" ");
        }

        return section;
    }
    private string BuildVersionInfoTopOrBottom(int width, bool top)
    {
        StringBuilder line = new StringBuilder();

        string leftEdge  = top ? "┌" : "└";
        string rightEdge = top ? "┐" : "┘";

        line.Append(leftEdge);
        for (int i = 0; i < width-2; i++)
        {
            line.Append("─");
        }
        line.Append(rightEdge);

        return line.ToString();
    }
    private string BuildVersionInfoInnerHeader(string text, int width)
    {
        var textTrimmed = text;
        int maxTextWidth = width - 4;
        if (textTrimmed.Length > maxTextWidth)
        {
            textTrimmed.Substring(0, maxTextWidth);
        }

        var stringBuilder = new StringBuilder();

        stringBuilder.Append("│ ");
        stringBuilder.Append(textTrimmed);

        int spaces = width - stringBuilder.Length - 1;
        for (int i = 0; i < spaces; i++)
        {
            stringBuilder.Append(" ");
        }

        stringBuilder.Append("│");

        return stringBuilder.ToString();
    }
    private List<string> BuildVersionInfoInnerText(string text, int width, int height)
    {
        var list = new List<string>();
        int innerWidth = width - 4;

        string current = text;
        for (int i=0; i<height; i++)
        {
            string subString = "";

            try
            {
                subString = current.Substring(0, innerWidth);
                current = current.Substring(innerWidth);
            }
            catch
            {
                subString = current;
                current = "";

                int spaces = innerWidth - subString.Length;
                for (int j=0; j < spaces; j++)
                {
                    subString += " ";
                }
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("│ ");
            stringBuilder.Append(subString);
            stringBuilder.Append(" │");

            list.Add(stringBuilder.ToString());
        }

        return list;
    }
}
